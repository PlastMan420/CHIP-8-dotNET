/*
 * Parsing and Executing 
 */

using System;
using System.IO;

namespace CHIP_8_dotNET.Chip8
{
    class Execution
    {
        private Memory memory = new Memory();
        private CPU cpu = new CPU();
        //private readonly InstructionSet instructionSet = new InstructionSet(ref Memory, ref cpu);
        public Execution(string path) 
        {
            
            memory.programMemory = File.ReadAllBytes(path); //  load program into programMemory[]
            memory.InitMemory();
            cpu.PC = 0x200;
            Program();    //  start the program
        }
        
        public void initCPU()
        {
            
        }
        public void Program()
        {
            cpu.IReg = memory.liveMem[cpu.PC];
            while (true)
            {
                Next();

            }

        }
        public void Next() 
        {
            cpu.IReg    = memory.liveMem[cpu.PC];
            ++cpu.PC;
            cpu.IReg    *= 0x100;
            cpu.IReg    += memory.liveMem[cpu.PC];
            ++cpu.PC;
        }

        public void Parse()
        {

        }
    }
}
