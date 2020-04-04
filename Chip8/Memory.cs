﻿/*
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

namespace CHIP_8_dotNET.Chip8
{
     class Memory
    {
        const   int systemSize                  = 0x4096;
        const   int fontSetSize                 = 80;
        const   int interpreterSize             = 0x0180;
        const   int programSize                 = 0x0C9D;
        const   int stackSize                   = 0x64;
        const   int videoMemorySize             = 0x00FF;
        const   int keypadSize                  = 16;

        public  byte[] liveMem                  = new byte[systemSize];
        public  byte[] interpreterMemory        = new byte[interpreterSize];
        public  byte[] programMemory            = new byte[programSize];
        public  byte[] stack                    = new byte[stackSize];
        public  uint[,] videoMemory             = new uint[64,32]; // It's uint32 so that we can use SDL2
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
        public byte[] keypad =
        {
            0x1, 0x2, 0x3, 
            0x4, 0x5, 0x6, 
            0x7, 0x8, 0x9, 
            0xA, 0xB, 0xC, 
            0xD, 0xE, 0xF
        };
        public Memory() { }
        public void InitProgram( )
        {
            throw new NotImplementedException();
        }
        public void InitMemory()
        {
            //liveMem = fontSet.Concat(interpreterMemory).Concat(programMemory)
            //    .Concat(stack).Concat(videoMemory).ToArray();
        }


    }
}
/*
 *  int[] front = { 1, 2, 3, 4 };
    int[] back = { 5, 6, 7, 8 };
    int[] combined = front.Concat(back).ToArray();
 */
