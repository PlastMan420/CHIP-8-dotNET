/*  
 *  This is the instruction set
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET.Chip8
{
	class InstructionSet
	{
		private readonly CPU cpu = new CPU();
		private readonly Memory memory = new Memory();
		//public byte Vx, Vy, data;
		
		public InstructionSet(ref Memory _memory, ref CPU _cpu)
		{
			memory = _memory;
			cpu = _cpu;
		}

		/// <summary>
		/// Helper methods
		/// </summary>
		public void ParseData(ref byte Vx, ref byte data, ref ushort opcode)
		{
			Vx		= (byte)((opcode & 0x0F00u) >> 8);
			data	= (byte) (opcode & 0x00FFu);
		}
		public (byte, byte) ParseData(ref ushort opcode)
		{
			byte	Vx		=	(byte)((opcode & 0x0F00u) >> 8);
			byte	data	=	(byte)(opcode & 0x00FFu);
			return	(Vx, data);
		}
		public void ParseRegisters(ref byte Vx, ref byte Vy, ref ushort opcode) // Helper method
		{
			Vx		=	(byte)((opcode & 0x0F00u) >> 8);
			Vy		=	(byte)((opcode & 0x00F0u) >> 4);
		}
		public (byte, byte) ParseRegisters(ref ushort opcode)
		{
			byte	Vx	=	(byte)((opcode & 0x0F00u) >> 8);
			byte	Vy	=	(byte)((opcode & 0x00F0u) >> 4);
			return (Vx, Vy);
		}
		public ushort ParseAddress(ref ushort opcode)
		{
			ushort address = (ushort)(opcode & 0x0FFFu); // min possible address is 0x200
			return address;
		}
		public byte RandByte()    // returns a random byte 
		{
			var		randGen = new Random();
			byte	randomByte = (byte)randGen.Next(0, 255);
			return	randomByte;
		} 
		/////////////////////////////////////////////////////////////////////////////////

		public void CLS() // 0x00E0
		{   // Clear video buffer (clears display)
			memory.videoMemory = null;
		}
		public void RET() // 0x00EE
		{
			--cpu.SP;
			cpu.PC	= memory.stack[cpu.SP];
		}

		public void JMP(ushort opcode) //op: 0x1nnn --> 1000 + a 12 bit address
		{
			ushort address	=	ParseAddress(ref opcode); // Casting to prevent Int promotions
			cpu.PC			=	address;
		}
		public void CALL(ushort opcode) // 0x2nnn --> 2000 + a 12 bit address
		{
			ushort address			=	ParseAddress(ref opcode);

			memory.stack[cpu.SP]	=	(byte)cpu.PC;
			++cpu.SP;
			cpu.PC					=	address;
		}
		public void SEQ_3xkk(ushort opcode)   // 0x3XKK --> 0x3000 + (x = register number) + (kk = value)
		{
			(byte, byte) parsedData = ParseData(ref opcode);	// compute once and store  in a (byte, byte)
			byte Vx					= parsedData.Item1;			// Get register number
			byte data				= parsedData.Item2;			// fetch data
			if (cpu.registers[Vx]	==	data)
				cpu.PC	+=	2;
		}
		public void SNE_4xkk(ushort opcode) // Skip next instruction if Vx != kk.
		{
			(byte, byte) parsedData = ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte Vx					= parsedData.Item1;			// Get register number
			byte data				= parsedData.Item2;			// fetch data
			if (cpu.registers[Vx] != data)
				cpu.PC += 2;
		}
		public void SE_5xy0(ushort opcode)  // Skip next instruction if Vx = Vy.
		{
			(byte, byte) parsedRegisters	=	ParseRegisters(ref opcode);
			byte Vx							=	parsedRegisters.Item1;
			byte Vy							=	parsedRegisters.Item2;
			if (cpu.registers[Vx] == cpu.registers[Vy])
				cpu.PC += 2;
		}

		public void LD_6xkk(ushort opcode)  // Set Vx = kk.
		{
			(byte, byte) parsedData = ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte Vx					= parsedData.Item1;         // Get register number
			byte data				= parsedData.Item2;         // fetch data
			cpu.registers[Vx] = data;
		}
		public void ADD_7xkk(ushort opcode) // Set Vx = Vx + kk.
		{
			(byte, byte) parsedData =	ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte Vx					=	parsedData.Item1;         // Get register number
			byte data				=	parsedData.Item2;         // fetch data
			cpu.registers[Vx] += data;
		}



		/// <summary>
		/// Opcode set 8xxx. they include logical and arithmetic operations
		/// </summary>
		/// <param name="opcode"></param>
		public void Set_8000(ushort opcode)
		{
			(byte, byte) parsedRegisters	=	ParseRegisters(ref opcode);
			byte Vx							=	parsedRegisters.Item1;
			byte Vy							=	parsedRegisters.Item2;
			byte op							=	(byte)(opcode & 0x000F);
			switch (op)
			{
				case 0x0:         // Load Vy into Vx
					cpu.registers[Vx] = cpu.registers[Vy];
					break;

				case 0x1:        // AND
					cpu.registers[Vx] &= cpu.registers[Vy];
					break;

				case 0x2:       // OR
					cpu.registers[Vx] |= cpu.registers[Vy];
					break;

				case 0x3:      // XOR
					cpu.registers[Vx] ^= cpu.registers[Vy];
					break;

				case 0x4:      // summing 2 registers.
					int sum = (cpu.registers[Vx] + cpu.registers[Vy]);
					if (sum > 255u) cpu.registers[15] = 1; else cpu.registers[15] = 0;
					cpu.registers[Vx] = (byte)(sum & 0xFFu);
					break;

				case 0x5:     // subtracting Vy from Vx.
					if (cpu.registers[Vx] > cpu.registers[Vy]) cpu.registers[15] = 1; else cpu.registers[15] = 0;
					cpu.registers[Vx] -= cpu.registers[Vy];
					break;

				case 0x6:
					cpu.registers[15] = (byte)(cpu.registers[15] & 0x1u);
					cpu.registers[Vx] >>= 1;
					break;

				case 0x7:      // SUBN Vx, Vy         // Set Vx = Vy - Vx, set VF = NOT borrow.
					if (cpu.registers[Vy] > cpu.registers[Vx]) cpu.registers[15] = 1; else cpu.registers[15] = 0;
					cpu.registers[Vx] = (byte)(cpu.registers[Vy] - cpu.registers[Vx]);
					break;

				case 0xE:   // SHL Vx {, Vy}   // Set Vx = Vx SHL 1. 
					cpu.registers[15] = (byte)((cpu.registers[Vx] & 0x80u) >> 7);
					cpu.registers[Vx] <<= 1;
					break;
			}
				
		}
		public void SNE_9xy0(ushort opcode) // 9xy0 - SNE Vx, Vy.  // Skip next instruction if Vx != Vy.
		{
			(byte, byte) parsedRegisters	=	ParseRegisters(ref opcode);
			byte Vx							=	parsedRegisters.Item1;
			byte Vy							=	parsedRegisters.Item2;
		}
		public void LD_Annn(ushort opcode)  //  set I = nnn
		{
			cpu.IReg						=	ParseAddress(ref opcode);
		}
		public void JP_Bnnn(ushort opcode)  //  Jump to location nnn + V0.
		{
			cpu.PC							=	(ushort)(cpu.registers[0] + ParseAddress(ref opcode));
		}
		public void RND_Cxkk(ushort opcode) //  Set Vx = random byte AND kk.
		{
			(byte, byte) parsedData = ParseData(ref opcode);				// compute once and store in a (byte, byte)
			byte Vx							=	parsedData.Item1;			// Get register number
			byte data						=	parsedData.Item2;			// fetch data

			cpu.registers[Vx]				=	(byte)(RandByte() & data);
		}

		public void DRW_Dxyn(ushort opcode)
		{
			// TODO
		}
		public void SKIP_Exxx(ushort opcode)
		{
			(byte, byte) parsedData			=	ParseData(ref opcode);		//	Compute once and store in a (byte, byte)
			byte Vx							=	parsedData.Item1;			//	Get register number
			byte op							=	parsedData.Item2;			//	Fetch data
			byte key						=	cpu.registers[Vx];			//	Fetch Value from selected register
			ConsoleKeyInfo input = Console.ReadKey();
			switch (op)
			{
				case 0x9E:		//Skip next instruction if key with the value of Vx is pressed.
					if (input.KeyChar	==	key) cpu.PC	+=	2;
					break;
				case 0xA1:		//	Skip next instruction if key with the value of Vx is not pressed.
					if (input.KeyChar	==	key) cpu.PC	+=	2;
					break;
			}
		}
		public void LD_Fxxx(ushort opcode) 
		{
			(byte, byte) parsedData			=	ParseData(ref opcode);		// compute once and store in a (byte, byte)
			byte Vx							=	parsedData.Item1;			// Get register number
			byte data						=	parsedData.Item2;			// fetch data
		}
	}
}
