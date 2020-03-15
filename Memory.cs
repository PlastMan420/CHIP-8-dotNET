using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET
{
    static class Memory
    {
        public static byte[] memory = new byte[4096];

        const int interpreterSize                         = 0x0200;
        const int programSize                             = 0x0CFF;
        const int ETI660ProgramSize                       = 0x09FF;
        const int videoMemorySize                         = 0x00FF;
        //const int fontSetSize                           = 80;

        public static byte[] interpreterMemory            = new byte[interpreterSize];
        public static byte[] programMemory                = new byte[programSize];
        public static byte[] ETI660ProgramMemory          = new byte[ETI660ProgramSize];
        public static byte[] videoMemory                  = new byte[videoMemorySize];
        //byte[] fontSet                                  = new byte[fontSetSize];

        // Fonts are part of interpreter memory
        public static byte[] fontSet =
        {
                0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
	            0x20, 0x60, 0x20, 0x20, 0x70, // 1
	            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
	            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
	            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
	            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
	            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
	            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
	            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
	            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
	            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
	            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
	            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
	            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
	            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
	            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };
        
    }
}
