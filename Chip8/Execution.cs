using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET.Chip8
{
    class Execution
    {
        public Execution() 
        {
            Memory memory = new Memory();
            CPU cpu = new CPU();
            InstructionSet instructionSet = new InstructionSet(memory, cpu);
        }
        


    }
}
