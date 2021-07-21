/*
 * This defines how the memory looks like.
 */

/*     Memory Map (in bytes) -------------->
 * | fonts 0x80 | interpreter 0x180 | program 0xC9B | stack 0x60 | Display Memory 0xFF |
 *  0                           1FF  200
 */

using System.Collections.Generic;

namespace CHIP_8_dotNET.Chip8
{
	class Memory
	{
		const int systemSize = 0x1000; // 4KB
		const int fontSetSize = 80;
		const int keypadSize = 16;
		const int interpreterSize = 0x200 - fontSetSize;    //	0x1FF - 0x80 = 0x01B0 - 0x50 = 0x160
		const int programSize = 0xCA0;    //	Starts at location 0x200 -> 0xEA0 + stack starting at 0xEA0 
										  //const   int stackSize                   = 0x060;		//	96 bytes of stack
		const int stackSize = 16;
		const int videoSize = 64 * 32;     //	256 bytes 64x32 bits 0xF00 -> 0xFFF

		public byte[] liveMem = new byte[systemSize];
		public Stack<ushort> stack = new Stack<ushort>();
		public byte[] programMemory = new byte[programSize];
		public uint[] videoMemory = new uint[videoSize]; // It's uint32 so that we can use SDL2
		public byte[] fontSet =
		{   0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
			0x20, 0x60, 0x20, 0x20, 0x70, // 1
			0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
			0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
			0x90, 0x90, 0xF0, 0x10, 0x10, // 4
			0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
			0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
			0xF0, 0x10, 0x20, 0x40, 0x40, // 7
			0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
			0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
			0xF0, 0x90, 0xF0, 0x90, 0x90, // A
			0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
			0xF0, 0x80, 0x80, 0x80, 0xF0, // C
			0xE0, 0x90, 0x90, 0x90, 0xE0, // D
			0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
			0xF0, 0x80, 0xF0, 0x80, 0x80  // F
		};
		public Dictionary<char, byte> keypad = new Dictionary<char, byte>()
		{
			{'0', 0 },
			{'1', 1 },
			{'2', 2 },
			{'3', 3 },
			{'4', 4 },
			{'5', 5 },
			{'6', 6 },
			{'7', 7 },
			{'8', 8 },
			{'9', 9 },
			{'a', 0x0A },
			{'b', 0x0B },
			{'c', 0x0C },
			{'d', 0x0D },
			{'e', 0x0E },
			{'f', 0x0F }
		};
		public Memory()
		{
		}
		public void InitMemory()
		{
			fontSet.CopyTo(liveMem, 0);
			programMemory.CopyTo(liveMem, 0x200);
			//for(int i = 0; i < liveMem.Length; i++)
			//Console.WriteLine("{0:x3}: {1:x}", i, liveMem[i]);
			//Console.WriteLine(liveMem.Length);
		}

	}
}