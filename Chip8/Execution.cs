/*
 * Parsing and Executing 
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;

namespace CHIP_8_dotNET.Chip8
{
	class Execution
	{
		private Memory memory	= new Memory();
		private CPU cpu			= new CPU();
		ushort  opcode;
		public Execution(string path) 
		{
			memory.programMemory = File.ReadAllBytes(path); //  load program into programMemory[]
			memory.InitMemory();
			cpu.InitCPU();
			
			Program();    //  start the program
		}
		
		public void Program()
		{
			InstructionSet instructionSet = new InstructionSet(ref memory, ref cpu);

			if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
			{
				Console.WriteLine("SDL failed to init.");
				return;
			}

			const int videoScale = 15;
			const int pitch = 4 * 64;   //  sizeof(chip8.video[0]) * VIDEO_WIDTH;

			IntPtr window = SDL.SDL_CreateWindow("Chip-8 Interpreter", 128, 128, 64 * videoScale, 32 * videoScale, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

			if (window == IntPtr.Zero)
			{
				Console.WriteLine("SDL could not create a window.");
				return;
			}

			IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

			if (renderer == IntPtr.Zero)
			{
				Console.WriteLine("SDL could not create a valid renderer.");
				return;
			}
			
			Console.WriteLine("program starting");
			IntPtr sdlSurface, sdlTexture = IntPtr.Zero;
			SDL.SDL_Event sdlEvent;
			bool run = true;
			while (run)
			{

				try
				{
					Next();

					/*
						Dictionary<int, Action<ushort>>
						int = Parse(). Action<ushort> is a delegate to void methods with 1 ushort parameter.
					*/
					instructionSet.InstructionList[Parse()](opcode);

					while (SDL.SDL_PollEvent(out sdlEvent) != 0)
					{
						if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT)
						{
							run = false;
						}
					}
					var displayHandle = GCHandle.Alloc(memory.videoMemory, GCHandleType.Pinned);

					if (sdlTexture != IntPtr.Zero) SDL.SDL_DestroyTexture(sdlTexture);

					sdlSurface = SDL.SDL_CreateRGBSurfaceFrom(displayHandle.AddrOfPinnedObject(), 64, 32, 32, pitch, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
					sdlTexture = SDL.SDL_CreateTextureFromSurface(renderer, sdlSurface);

					displayHandle.Free();

					SDL.SDL_RenderClear(renderer);
					SDL.SDL_RenderCopy(renderer, sdlTexture, IntPtr.Zero, IntPtr.Zero);
					SDL.SDL_RenderPresent(renderer);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message + ", main loop error");
				}
			}
			SDL.SDL_DestroyRenderer(renderer);
			SDL.SDL_DestroyWindow(window);
			Console.WriteLine("program exited");
		}
		public void Next()  //  Fetches upcoming instruction for decoding.
		{
			//Console.WriteLine("cpu.pc: {0:x}", cpu.PC);
			try
			{
				opcode = memory.liveMem[cpu.PC];
				opcode = (ushort)(opcode << 8);
				opcode += memory.liveMem[cpu.PC + 1];
				cpu.PC += 2;
				Console.WriteLine("{0:x3}: {1:x4}", cpu.PC-2, opcode);
				if (cpu.delayTimer > 0) --cpu.delayTimer;
				if (cpu.soundTimer > 0) --cpu.soundTimer;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message + ", program decoding error");
			}
		}

		public int Parse()  //  parses the leftmost byte of a word to determine the next instruction.
		{
			return (opcode & 0xF000) >> 12;

		}

	}
}

