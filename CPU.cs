/*
 * This is the CPU
 * -It defines the data structure of the CPU
 * -It looks up for methods inside 'InstructionSet' using the equivalent passed in opcode
 */

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace CHIP_8_dotNET
{
     class CPU
    {
        // Registers
        public  byte[] registers = new byte[16];
        public  UInt16 IReg;
        public  UInt16 PC;
        public  byte SP;

        // Instruction set
        // either do a big dictionary or a class + reflection
         Dictionary<UInt16, Delegate> opCode = new Dictionary<UInt16, Delegate>();
    }
}
