/*
 * This defines how the memory looks like.
 */

 /*     Memory Map (in bytes) -------------->
  * | fonts 0x80 | interpreter 0x180 | program 0xC9B | stack 0x64 | Display Memory 0xFF |
  * 
  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHIP_8_dotNET
{
     class Memory : IMemory
    {
        const int systemSize                        = 0x4096;
        const int fontSetSize                       = 80;
        const int interpreterSize                   = 0x0180;
        const int programSize                       = 0x0C9D;
        //const int ETI660ProgramSize                 = 0x09FF;
        const int stackSize                         = 0x64;
        const int videoMemorySize                   = 0x00FF;

        public  byte[] liveMem                      = new byte[systemSize];

        public  byte[] interpreterMemory            = new byte[interpreterSize];
        public  byte[] programMemory                = new byte[programSize];
        //public  byte[] ETI660ProgramMemory          = new byte[ETI660ProgramSize];
        public  byte[] stack                        = new byte[stackSize];
        public  byte[] videoMemory                  = new byte[videoMemorySize];
        public  byte[] fontSet =
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
        public void InitProgram( )
        {
            throw new NotImplementedException();
        }
        public void InitMemory()
        {
            liveMem = fontSet.Concat(interpreterMemory).Concat(programMemory)
                .Concat(stack).Concat(videoMemory).ToArray();
        }


    }
}
/*
 *  int[] front = { 1, 2, 3, 4 };
    int[] back = { 5, 6, 7, 8 };
    int[] combined = front.Concat(back).ToArray();
 */
