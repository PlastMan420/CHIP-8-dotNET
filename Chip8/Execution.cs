/*
 * Parsing and Executing 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET.Chip8
{
    class Execution
    {
        private readonly Memory memory = new Memory();
        private readonly CPU cpu = new CPU();
        public Execution() 
        {
            
            InstructionSet instructionSet = new InstructionSet(ref memory, ref cpu);
            cycle();
        }
        
        public void cycle()
        {
            ushort counter = 200;
            ushort operation = 0;
            cpu.PC = memory.programMemory[0];
            while (true)
            {
                
            }

        }
        public ushort parse()
        {
            return (ushort)(cpu.PC & 0xF000u); 
        }
    }
}
