/*
 * Parsing and Executing 
 */

using System;
using System.IO;
using SDL2;

namespace CHIP_8_dotNET.Chip8
{
    class Execution
    {
        private Memory memory = new Memory();
        private CPU cpu = new CPU();
        ushort opcode;
        public Execution(string path) 
        {
            memory.programMemory = File.ReadAllBytes(path); //  load program into programMemory[]
            memory.InitMemory();

            cpu.PC          = 0x200;
            cpu.delayTimer  = 0;
            cpu.soundTimer  = 0;
            Program();    //  start the program
        }
        
        public void Program()
        {
            InstructionSet instructionSet = new InstructionSet(ref memory, ref cpu);
            opcode        = memory.liveMem[cpu.PC];

            IntPtr window   = SDL.SDL_CreateWindow("Chip-8.NET", 0, 0, 64 * 8, 32 * 8, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            var renderer    = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            SDL.SDL_Event sdlEvent;

            bool run = true;
            while (run)
            {
                Next();
                while (SDL.SDL_PollEvent(out sdlEvent) != 0)
                {
                    if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT) run = false;
                }

                /*
                  Dictionary<int, Action<ushort>>
                  int = Parse(). Action<ushort> is a delegate to void methods with 1 ushort parameter.
                */
                instructionSet.InstructionList[Parse()](opcode);  
            }

        }
        public void Next()  //  Next cycle. reads the upcoming 2 memory locations to form the next word.
                            //  and stores in index register.
        {
            opcode  = memory.liveMem[cpu.PC];
            opcode  *= 0x100;
            opcode  += memory.liveMem[cpu.PC+1];
            opcode  += 0x002;
            if (cpu.delayTimer > 0) --cpu.delayTimer;
            if (cpu.soundTimer > 0) --cpu.soundTimer;
        }

        public int Parse()  //  parses the leftmost byte of a word to determine the next instruction.
        {
            return (opcode & 0xF000) >> 12;

        }
    }
}
