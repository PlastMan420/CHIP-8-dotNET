/*
 * This is the CPU
 */

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace CHIP_8_dotNET
{
    static class CHIP8
    {
        // Registers
        public static byte[] registers = new byte[16];
        public static UInt16 IReg;
        public static UInt16 PC;
        public static byte SP;

        // Instruction set
        // either do a big dictionary or a class + reflection
        static Dictionary<UInt16, Delegate> opCode = new Dictionary<UInt16, Delegate>();

        
        void Exec()
        {

        }

    }
}
