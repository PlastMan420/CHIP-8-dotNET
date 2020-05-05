using System;
using CHIP_8_dotNET.Chip8;

namespace CHIP_8_dotNET
{
	class Program
	{
		static void Main(string[] args)
		{
			//  Program's location would be in args[], now we load it into main memory
			Execution emulator = new Execution(args[0]);
		}
	}
}
