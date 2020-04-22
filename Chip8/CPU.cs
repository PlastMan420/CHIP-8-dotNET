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
        public  enum constants : ushort
        {
            pcLength  =   2
        }
        // Registers
        public  byte[]          registers = new byte[16];    // x16 8-bit general purpose registers
        public  UInt16          IReg;                        // index register
        public  UInt16          PC;                          // program counter
        public  byte            SP;                          // stack pointer
        public  byte            delayTimer;
        public  byte            soundTimer;
        //Dictionary<UInt16, Delegate> opCode = new Dictionary<UInt16, Delegate>();
        public CPU() { }
    }
}
