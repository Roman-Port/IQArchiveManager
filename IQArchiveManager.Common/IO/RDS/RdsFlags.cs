using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public enum RdsFlags
    {
        BLOCK_A_VALID = (1 << 0), // Set if A block is valid
        BLOCK_B_VALID = (1 << 1), // Set if B block is valid
        BLOCK_C_VALID = (1 << 2), // Set if C block is valid
        BLOCK_D_VALID = (1 << 3), // Set if D block is valid
        CONTIGUOUS    = (1 << 4), // Set if sync was retained from the last frame
        CORRECTED     = (1 << 5), // Set if a block was modified from a correction
        RESERVED      = (1 << 6), // Unused
        END_OF_CHUNK  = (1 << 7)  // Set if this is the last frame in the packet
    }
}
