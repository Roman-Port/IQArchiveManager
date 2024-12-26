using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS.DSPs
{
    class RdsDspOriginal : IRdsDsp
    {
        public bool IsSynced
        {
            get => isSynced;
            private set
            {
                //Reset respective values depending on the state
                if (value)
                {
                    badBlocks = 0;
                    blocks = 0;
                    blockBits = 0;
                    groupAssemblyRunning = false;
                }
                else
                {
                    presync = false;
                }

                //Update value and dispatch event
                isSynced = value;
            }
        }

        private bool isSynced;
        private long bits;
        private long presyncOffsetBits;
        private long bitsBuffer;
        private int blockBits;
        private int badBlocks;
        private int blocks;
        private int goodBlocks;
        private int[] group = new int[4];
        private bool presync;
        private bool groupAssemblyRunning;
        private int lastOffset;
        private int blockIndex;

        private readonly static int[] OFFSET_POS = { 0, 1, 2, 3, 2 };
        private readonly static int[] OFFSET_WORD = { 252, 408, 360, 436, 848 };
        private readonly static int[] SYNDROME = { 383, 14, 303, 663, 748 };

        public List<RdsPacket> Load(Stream stream)
        {
            //Create reader on it
            RdsDeserializer reader = new RdsDeserializer(stream);

            //Read out bits and push into bit decoder
            long timestamp;
            byte bit;
            RdsPacket? packet;
            List<RdsPacket> output = new List<RdsPacket>();
            while (reader.ReadBit(out timestamp, out bit))
            {
                //Decode
                packet = Process(bit, timestamp);

                //If got a result, push into decoder
                if (packet != null && (packet.Value.flags & RdsFlags.CORRECTED) != RdsFlags.CORRECTED)
                {
                    output.Add(packet.Value);
                }
            }

            return output;
        }

        public RdsPacket? Process(byte bit, long timestamp)
        {
            //Push bit to the buffer
            bitsBuffer = (bitsBuffer << 1) | bit;

            //Attempt to sync
            RdsPacket? packet = null;
            if (isSynced)
                packet = ProcessSynced(timestamp);
            else
                ProcessUnsynced();

            //Update counter
            bits++;

            return packet;
        }

        private RdsPacket? ProcessSynced(long timestamp)
        {
            RdsPacket? packet = null;
            /* wait until 26 bits enter the buffer */
            if (blockBits < 25)
            {
                blockBits++;
            }
            else
            {
                //Calculate word
                int dataword = (int)((bitsBuffer >> 10) & 0xffff);

                //Check CRC
                bool crcOk = CheckBlockCrc(dataword);
                if (!crcOk)
                    badBlocks++;

                //If this was decoded OK and this is the first block, we know to begin assembly
                if (blockIndex == 0 && crcOk)
                {
                    groupAssemblyRunning = true;
                    goodBlocks = 1;
                }

                //Begin assembling group
                if (groupAssemblyRunning)
                {
                    //Read in parts
                    if (!crcOk)
                    {
                        groupAssemblyRunning = false;
                    }
                    else
                    {
                        group[blockIndex] = dataword;
                        goodBlocks++;
                    }

                    //If we get all blocks successfully, submit the frame
                    if (goodBlocks == 5)
                    {
                        packet = new RdsPacket
                        {
                            timestamp = timestamp,
                            flags = RdsFlags.BLOCK_A_VALID | RdsFlags.BLOCK_B_VALID | RdsFlags.BLOCK_C_VALID | RdsFlags.BLOCK_D_VALID,
                            a = (ushort)(group[0] & 0xFFFF),
                            b = (ushort)(group[1] & 0xFFFF),
                            c = (ushort)(group[2] & 0xFFFF),
                            d = (ushort)(group[3] & 0xFFFF)
                        };
                    }
                }

                //Update state
                blockBits = 0;
                blockIndex = (blockIndex + 1) % 4;
                blocks++;

                //Reset state if needed
                if (blocks == 50)
                {
                    if (badBlocks > 35)
                        IsSynced = false;
                    blocks = 0;
                    badBlocks = 0;
                }
            }
            return packet;
        }

        private void ProcessUnsynced()
        {
            //Calculate
            int decodedSyndrome = CalculateSyndrome(bitsBuffer, 26);

            //Try each group so we can detect which one we're looking at
            for (int j = 0; j < 5; j++)
            {
                //Check if it matches
                if (decodedSyndrome == SYNDROME[j])
                {
                    //Matches! We now know what block we are located on
                    if (!presync)
                    {
                        //Enter presync
                        lastOffset = j;
                        presyncOffsetBits = bits;
                        presync = true;
                    }
                    else
                    {
                        //Calculate bit distance
                        long bitDistance = bits - presyncOffsetBits;

                        //Calculate block distance
                        int blockDistance;
                        if (OFFSET_POS[lastOffset] >= OFFSET_POS[j])
                            blockDistance = OFFSET_POS[j] + 4 - OFFSET_POS[lastOffset];
                        else
                            blockDistance = OFFSET_POS[j] - OFFSET_POS[lastOffset];

                        //Detect sync
                        if ((blockDistance * 26) != bitDistance)
                        {
                            presync = false;
                        }
                        else
                        {
                            //We are now synced!
                            blockIndex = (j + 1) % 4;
                            IsSynced = true;
                        }
                    }
                    break;
                }
            }
        }

        private bool CheckBlockCrc(int dataword)
        {
            //Calculate
            int calculatedCrc = CalculateSyndrome(dataword, 16);
            long checkword = (int)(bitsBuffer & 0x3ff);

            //Check
            if (blockIndex == 2)
            {
                long actualCrc = checkword ^ OFFSET_WORD[blockIndex];
                if (actualCrc == calculatedCrc)
                {
                    return true;
                }
                else
                {
                    actualCrc = checkword ^ OFFSET_WORD[4];
                    return actualCrc == calculatedCrc;
                }
            }
            else
            {
                long actualCrc = checkword ^ OFFSET_WORD[blockIndex];
                return actualCrc == calculatedCrc;
            }
        }

        private int CalculateSyndrome(long block, int length)
        {
            long x = 0;
            for (int i = length; i > 0; i--)
            {
                x = (x << 1) | ((block >> (i - 1)) & 0x01);
                if ((x & (1 << 10)) != 0)
                    x = x ^ 0x5B9;
            }
            for (int i = 10; i > 0; i--)
            {
                x = x << 1;
                if ((x & (1 << 10)) != 0)
                    x = x ^ 0x5B9;
            }
            return (int)(x & ((1 << 10) - 1));
        }
    }
}
