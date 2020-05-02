/*
 * Parsing and Executing 
 */

using System;
using System.IO;

namespace CHIP_8_dotNET.Chip8
{
    class Execution
    {
        private readonly Memory memory = new Memory();
        private readonly CPU cpu = new CPU();
        public Execution(string path) 
        {
            InstructionSet instructionSet = new InstructionSet(ref memory, ref cpu);
            memory.programMemory = File.ReadAllBytes(path); //  load program into memory
            memory.InitMemory();
            cpu.PC = memory.liveMem[200];
            Program();    //  start the program
        }
        
        public void Program()
        {
            while (true)
            {
                
            }

        }

    }
}
