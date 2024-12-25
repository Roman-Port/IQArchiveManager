using IQArchiveManager.Common.IO.RDS;
using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Client.RDS
{
    // Modified + ported version of SDR++ RDS decoder
    class RdsBlockDecoder
    {
        public RdsBlockDecoder()
        {

        }

        private uint shiftReg = 0;
        private int sync = 0;
        private int skip = 0;
        private BlockType lastType = BlockType.BLOCK_TYPE_A;
        private uint[] blocks = new uint[4];
        private RdsFlags flags;
        private bool submitted = false; // Reset at the start of every frame
        private bool corrected = false; // Reset at the start of every frame; True if any frames were corrected

        private enum BlockType
        {
            BLOCK_TYPE_A,
            BLOCK_TYPE_B,
            BLOCK_TYPE_C,
            BLOCK_TYPE_CP,
            BLOCK_TYPE_D,
            _BLOCK_TYPE_COUNT
        };

        private static readonly Dictionary<ushort, BlockType> SYNDROMES = new Dictionary<ushort, BlockType>() {
            { 0b1111011000, BlockType.BLOCK_TYPE_A  },
            { 0b1111010100, BlockType.BLOCK_TYPE_B  },
            { 0b1001011100, BlockType.BLOCK_TYPE_C  },
            { 0b1111001100, BlockType.BLOCK_TYPE_CP },
            { 0b1001011000, BlockType.BLOCK_TYPE_D  }
        };

        private static readonly Dictionary<BlockType, ushort> OFFSETS = new Dictionary<BlockType, ushort>() {
            { BlockType.BLOCK_TYPE_A,  0b0011111100 },
            { BlockType.BLOCK_TYPE_B,  0b0110011000 },
            { BlockType.BLOCK_TYPE_C,  0b0101101000 },
            { BlockType.BLOCK_TYPE_CP, 0b1101010000 },
            { BlockType.BLOCK_TYPE_D,  0b0110110100 }
        };

        private const ushort LFSR_POLY = 0b0110111001;
        private const ushort IN_POLY = 0b1100011011;

        private const int BLOCK_LEN = 26;
        private const int DATA_LEN = 16;
        private const int POLY_LEN = 10;

        public RdsPacket? Process(byte symbol, uint timestamp)
        {
            // Shift in the bit
            shiftReg <<= 1;
            shiftReg &= 0x3FFFFFF;
            shiftReg |= (uint)(symbol & 1);

            // Skip if we need to shift in new data
            if (--skip > 0) { return null; }

            // Calculate the syndrome and update sync status
            ushort syn = calcSyndrome(shiftReg);
            bool knownSyndrome = SYNDROMES.TryGetValue(syn, out BlockType synIt);
            if (!knownSyndrome)
                SetOutputFlag(RdsFlags.CONTIGUOUS, false);
            sync = clamp(knownSyndrome ? ++sync : --sync, 0, 4);

            // If we're still no longer in sync, try to resync
            if (sync == 0) { return null; }

            // Figure out which block we've got
            BlockType type;
            if (knownSyndrome)
            {
                type = SYNDROMES[syn];
            }
            else
            {
                type = (BlockType)(((int)lastType + 1) % (int)BlockType._BLOCK_TYPE_COUNT);
            }

            // Save block while correcting errors (NOT YET) <- idk why the "not yet is here", TODO: find why
            uint block = correctErrors(shiftReg, type, out bool isAvailable, out bool correctionApplied);

            //Check if we need to reset the state
            if (type == BlockType.BLOCK_TYPE_A)
            {
                submitted = false;
                corrected = false;
            }

            //Set corrected flag
            corrected = corrected || correctionApplied;

            //Switch on type
            RdsPacket? result = null;
            if (type == BlockType.BLOCK_TYPE_A)
            {
                blocks[0] = block;
                SetOutputFlag(RdsFlags.BLOCK_A_VALID, isAvailable);
            }
            else if (type == BlockType.BLOCK_TYPE_B && lastType == BlockType.BLOCK_TYPE_A && !submitted)
            {
                blocks[1] = block;
                SetOutputFlag(RdsFlags.BLOCK_B_VALID, isAvailable);
            }
            else if ((type == BlockType.BLOCK_TYPE_C || type == BlockType.BLOCK_TYPE_CP) && lastType == BlockType.BLOCK_TYPE_B && !submitted)
            {
                blocks[2] = block;
                SetOutputFlag(RdsFlags.BLOCK_C_VALID, isAvailable);
            }
            else if (type == BlockType.BLOCK_TYPE_D && (lastType == BlockType.BLOCK_TYPE_C || lastType == BlockType.BLOCK_TYPE_CP) && !submitted)
            {
                blocks[3] = block;
                SetOutputFlag(RdsFlags.BLOCK_D_VALID, isAvailable);

                //Submit completed frame
                SetOutputFlag(RdsFlags.CORRECTED, corrected);
                result = SubmitFrame(timestamp);

                //Set state machine
                SetOutputFlag(RdsFlags.CONTIGUOUS, true);
                submitted = true;
            }
            else
            {
                //Out of order
                SetOutputFlag(RdsFlags.CONTIGUOUS, false);
            }

            //Skip to next block
            lastType = type;
            skip = BLOCK_LEN;

            return result;
        }

        private void SetOutputFlag(RdsFlags flag, bool set)
        {
            if (set)
                flags = (RdsFlags)((int)flags | (1 << (int)flag));
            else
                flags = (RdsFlags)((int)flags & ~(1 << (int)flag));
        }

        private RdsPacket SubmitFrame(uint timestamp)
        {
            //Format the frame
            ulong output = 0;
            ushort block;
            int flipped;
            for (int i = 0; i < 4; i++)
            {
                //Get only data bits
                block = (ushort)(blocks[i] >> 10);

                //Flip all the bits of this block for compatibility reasons
                flipped = 0;
                int bitOffset = 0;
                for (int j = 15; j >= 0; j--)
                    flipped |= ((block >> bitOffset++) & 1) << j;

                //Push in
                output |= (ulong)flipped << (i * 16);
            }

            return new RdsPacket
            {
                timestamp = timestamp,
                flags = flags,
                frame = output
            };
        }

        private int clamp(int input, int min, int max)
        {
            if (input < min)
                return min;
            if (input > max)
                return max;
            return input;
        }

        private ushort calcSyndrome(uint block)
        {
            ushort syn = 0;

            // Calculate the syndrome using a LFSR
            for (int i = BLOCK_LEN - 1; i >= 0; i--)
            {
                // Shift the syndrome and keep the output
                byte outBit = (byte)((syn >> (POLY_LEN - 1)) & 1);
                syn = (ushort)((syn << 1) & 0b1111111111);

                // Apply LFSR polynomial
                syn ^= (ushort)(LFSR_POLY * outBit);

                // Apply input polynomial.
                syn ^= (ushort)(IN_POLY * ((block >> i) & 1));
            }

            return syn;
        }

        private uint correctErrors(uint block, BlockType type, out bool recovered, out bool correctionApplied)
        {
            // Subtract the offset from block
            block ^= (uint)OFFSETS[type];
            uint outb = block;

            // Calculate syndrome of corrected block
            ushort syn = calcSyndrome(block);

            // Use the syndrome register to do error correction if errors are present
            byte errorFound = 0;
            correctionApplied = false;
            if (syn != 0)
            {
                for (int i = DATA_LEN - 1; i >= 0; i--)
                {
                    // Check if the 5 leftmost bits are all zero
                    errorFound |= (byte)(((syn & 0b11111) == 0) ? 1 : 0);
                    correctionApplied = correctionApplied || errorFound > 0;

                    // Write output
                    byte outBit = (byte)((syn >> (POLY_LEN - 1)) & 1);
                    outb ^= (uint)(errorFound & outBit) << (i + POLY_LEN);

                    // Shift syndrome
                    syn = (ushort)((syn << 1) & 0b1111111111);
                    syn ^= (ushort)(LFSR_POLY * outBit * ((!(errorFound != 0)) ? 1 : 0));
                }
            }
            recovered = (syn & 0b11111) == 0;

            return outb;
        }

    }
}
