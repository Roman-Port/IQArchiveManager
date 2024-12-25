using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Server.Pre
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

        public void Process(byte[] symbols, int count, List<ulong> output)
        {
            for (int i = 0; i < count; i++)
            {
                // Shift in the bit
                shiftReg <<= 1;
                shiftReg &= 0x3FFFFFF;
                shiftReg |= (uint)(symbols[i] & 1);

                // Skip if we need to shift in new data
                if (--skip > 0) { continue; }

                // Calculate the syndrome and update sync status
                ushort syn = calcSyndrome(shiftReg);
                bool knownSyndrome = SYNDROMES.TryGetValue(syn, out BlockType synIt);
                sync = clamp(knownSyndrome ? ++sync : --sync, 0, 4);

                // If we're still no longer in sync, try to resync
                if (sync == 0) { continue; }

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
                bool isAvailable;
                uint block = correctErrors(shiftReg, type, out isAvailable);

                //Switch on type
                if (type == BlockType.BLOCK_TYPE_A)
                {
                    blocks[0] = block;
                } else if (type == BlockType.BLOCK_TYPE_B && lastType == BlockType.BLOCK_TYPE_A)
                {
                    blocks[1] = block;
                }
                else if ((type == BlockType.BLOCK_TYPE_C || type == BlockType.BLOCK_TYPE_CP) && lastType == BlockType.BLOCK_TYPE_B)
                {
                    blocks[2] = block;
                }
                else if (type == BlockType.BLOCK_TYPE_D && (lastType == BlockType.BLOCK_TYPE_C || lastType == BlockType.BLOCK_TYPE_CP))
                {
                    blocks[3] = block;
                    output.Add(SubmitFrame());
                }

                //Skip to next block
                lastType = type;
                skip = BLOCK_LEN;
            }
        }

        private ulong SubmitFrame()
        {
            //Format
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

            return output;
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

        private uint correctErrors(uint block, BlockType type, out bool recovered)
        {
            // Subtract the offset from block
            block ^= (uint)OFFSETS[type];
            uint outb = block;

            // Calculate syndrome of corrected block
            ushort syn = calcSyndrome(block);

            // Use the syndrome register to do error correction if errors are present
            byte errorFound = 0;
            if (syn != 0)
            {
                for (int i = DATA_LEN - 1; i >= 0; i--)
                {
                    // Check if the 5 leftmost bits are all zero
                    errorFound |= (byte)(((syn & 0b11111) == 0) ? 1 : 0);

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
