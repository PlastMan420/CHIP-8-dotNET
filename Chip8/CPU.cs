/*
 * This defines the CPU as a structure.
 * Execusion.cs and InstructionSet.cs define the CPU's execusion.
 */

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace CHIP_8_dotNET.Chip8
{
     class CPU
    {
        // Registers
        public  byte[]          registers = new byte[16];    // x16 8-bit general purpose registers
        public  int             Vf = 15;
        public  ushort          IReg;                        // index register
        public  ushort          PC;                          // program counter
        public  byte            SP;                          // stack pointer
        public  byte            delayTimer;
        public  byte            soundTimer;

        
        

        public CPU() 
        {
        }
    }
}
