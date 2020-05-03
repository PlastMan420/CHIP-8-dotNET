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
        
        public Execution(string path) 
        {
            
            memory.programMemory = File.ReadAllBytes(path); //  load program into programMemory[]
            memory.InitMemory();
            cpu.PC = 0x200;
            Program();    //  start the program
        }
        
        public void Program()
        {
            InstructionSet instructionSet = new InstructionSet(ref memory, ref cpu);
            cpu.IReg = memory.liveMem[cpu.PC];
            while (true)
            {
                Next();

                //  Dictionary<int, Action<ushort>>
                //  int = Parse(). Action<ushort> is a delegate to void methods with 1 ushort parameter.
                instructionSet.InstructionList[Parse()](cpu.IReg);  
            }

        }
        public void Next()  //  Next cycle. reads the upcoming 2 memory locations to form the next word.
                            //  and stores in index register.
        {
            cpu.IReg    = memory.liveMem[cpu.PC];
            cpu.IReg    *= 0x100;
            cpu.IReg    += memory.liveMem[cpu.PC+1];
            cpu.PC      += 0x002;
            if (cpu.delayTimer > 0) --cpu.delayTimer;
            if (cpu.soundTimer > 0) --cpu.soundTimer;
        }

        public int Parse()  //  parses the leftmost byte of a word to determine the next instruction.
        {
            return (cpu.IReg & 0xF000) >> 12;

        }
    }
}
