/*  
 *  This is the instruction set
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CHIP_8_dotNET.Chip8
{
	class InstructionSet
	{
		private readonly CPU cpu = new CPU();
		private readonly Memory memory = new Memory();

		public Action<ushort> InstructionListDelegate;
		public Dictionary<int, Action<ushort>> InstructionList = new Dictionary<int, Action<ushort>>();


		public InstructionSet(ref Memory _memory, ref CPU _cpu)
		{
			memory = _memory;
			cpu = _cpu;
			InstructionList.Add(0, this.CLS_RET_00xx);
			InstructionList.Add(0x1, this.JMP_1nnn);
			InstructionList.Add(0x2, this.CALL_2nnn);
			InstructionList.Add(0x3, this.SEQ_3xkk);
			InstructionList.Add(0x4, this.SNE_4xkk);
			InstructionList.Add(0x5, this.SE_5xy0);
			InstructionList.Add(0x6, this.LD_6xkk);
			InstructionList.Add(0x7, this.ADD_7xkk);
			InstructionList.Add(0x8, this.Set_8000);
			InstructionList.Add(0x9, this.SNE_9xy0);
			InstructionList.Add(0xA, this.LD_Annn);
			InstructionList.Add(0xB, this.JP_Bnnn);
			InstructionList.Add(0xC, this.RND_Cxkk);
			InstructionList.Add(0xD, this.DRW_Dxyn);
			InstructionList.Add(0xE, this.SKIP_Exxx);
			InstructionList.Add(0xF, this.LD_Fxxx);
		}



		/// <summary>
		/// Helper methods
		/// </summary>
		public (byte, byte) ParseData(ref ushort opcode)
		{
			byte	Vx		=	(byte)((opcode & 0x0F00u) >> 8);
			byte	data	=	(byte)(opcode & 0x00FFu);
			return	(Vx, data);
		}
		public (byte, byte, byte) ParseRegisters(ref ushort opcode)
		{
			byte	Vx	=	(byte)((opcode & 0x0F00u) >> 8);
			byte	Vy	=	(byte)((opcode & 0x00F0u) >> 4);
			byte	op	=	(byte)(opcode & 0x00F0u);
			return (Vx, Vy, op);
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
		public byte ReadInput()
		{
			int	input	=	Convert.ToInt32(Console.ReadKey());
			if(memory.keypad.Contains(input))	
				return	Convert.ToByte(input);
			
			return	ReadInput();
		}
		/////////////////////////////////////////////////////////////////////////////////

		public void CLS_RET_00xx(ushort opcode)
		{
			(byte, byte) ParsedData = ParseData(ref opcode);
			byte op = (byte)(ParsedData.Item2 & 0x00FF);
			switch (op)
			{
				case 0x00E0:
					memory.videoMemory = null;
					break;
				case 0x00EE:
					--cpu.SP;
					cpu.PC = memory.stack[cpu.SP];
					break;
			}
		}

		public void JMP_1nnn(ushort opcode) //op: 0x1nnn --> 1000 + a 12 bit address
		{
			ushort address						=	ParseAddress(ref opcode); // Casting to prevent Int promotions
			cpu.PC								=	address;
		}
		public void CALL_2nnn(ushort opcode) // 0x2nnn --> 2000 + a 12 bit address
		{
			ushort address						=	ParseAddress(ref opcode);

			memory.stack[cpu.SP]				=	(byte)cpu.PC;
			++cpu.SP;	
			cpu.PC								=	address;
		}
		public void SEQ_3xkk(ushort opcode)   // 0x3XKK --> 0x3000 + (x = register number) + (kk = value)
		{
			(byte, byte) parsedData				=	ParseData(ref opcode);	// compute once and store  in a (byte, byte)
			byte	Vx							=	parsedData.Item1;			// Get register number
			byte	data						=	parsedData.Item2;			// fetch data
			if (cpu.registers[Vx]	==	data)
				cpu.PC	+=	2;
		}
		public void SNE_4xkk(ushort opcode) // Skip next instruction if Vx != kk.
		{
			(byte, byte)	parsedData			=	ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte			Vx					=	parsedData.Item1;			// Get register number
			byte			 data				=	parsedData.Item2;			// fetch data
			if (cpu.registers[Vx] != data)
				cpu.PC += 2;
		}
		public void SE_5xy0(ushort opcode)  // Skip next instruction if Vx = Vy.
		{
			(byte, byte, byte) parsedRegisters	= ParseRegisters(ref opcode);
			byte Vx								= parsedRegisters.Item1;
			byte Vy								= parsedRegisters.Item2;
			byte op								= parsedRegisters.Item3;
			if (cpu.registers[Vx] == cpu.registers[Vy])
				cpu.PC += 2;
		}

		public void LD_6xkk(ushort opcode)  // Set Vx = kk.
		{
			(byte, byte)	parsedData			=	ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte			Vx					=	parsedData.Item1;         // Get register number
			byte			data				=	parsedData.Item2;         // fetch data
			cpu.registers[Vx] = data;
		}
		public void ADD_7xkk(ushort opcode) // Set Vx = Vx + kk.
		{
			(byte, byte)	parsedData			=	ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte			Vx					=	parsedData.Item1;         // Get register number
			byte			data				=	parsedData.Item2;         // fetch data
			cpu.registers[Vx] += data;
		}



		/// <summary>
		/// Opcode set 8xxx. they include logical and arithmetic operations
		/// </summary>
		/// <param name="opcode"></param>
		public void Set_8000(ushort opcode)
		{
			(byte, byte, byte) parsedRegisters = ParseRegisters(ref opcode);
			byte Vx = parsedRegisters.Item1;
			byte Vy = parsedRegisters.Item2;
			byte op = parsedRegisters.Item3;
			switch (op)
			{
				case	0x0:         // Load Vy into Vx
					cpu.registers[Vx] = cpu.registers[Vy];
					break;

				case	0x1:        // AND
					cpu.registers[Vx] &= cpu.registers[Vy];
					break;

				case	0x2:       // OR
					cpu.registers[Vx] |= cpu.registers[Vy];
					break;

				case	0x3:      // XOR
					cpu.registers[Vx] ^= cpu.registers[Vy];
					break;

				case	0x4:      // summing 2 registers.
					int sum = (cpu.registers[Vx] + cpu.registers[Vy]);
					if (sum > 255u) cpu.registers[15] = 1; else cpu.registers[15] = 0;
					cpu.registers[Vx] = (byte)(sum & 0xFFu);
					break;

				case	0x5:     // subtracting Vy from Vx.
					if (cpu.registers[Vx] > cpu.registers[Vy]) cpu.registers[15]	=	1; else cpu.registers[15] = 0;
					cpu.registers[Vx]	-= cpu.registers[Vy];
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
			(byte, byte, byte) parsedRegisters = ParseRegisters(ref opcode);
			byte Vx = parsedRegisters.Item1;
			byte Vy = parsedRegisters.Item2;
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
			(byte, byte) parsedData			=	ParseData(ref opcode);		// compute once and store in a (byte, byte)
			byte Vx							=	parsedData.Item1;			// Get register number
			byte data						=	parsedData.Item2;			// fetch data

			cpu.registers[Vx]				=	(byte)(RandByte() & data);
		}

		public unsafe void DRW_Dxyn(ushort opcode)
		//	Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
		/*
		 *	We iterate over the sprite, row by row and column by column. 
		 *	We know there are eight columns because a sprite is guaranteed to be eight pixels wide.
		 *	If a sprite pixel is on then there may be a collision with what’s already being displayed,
		 *	so we check if our screen pixel in the same location is set. If so we must set the VF register to express collision.
		 *	Then we can just XOR the screen pixel with 0xFFFFFFFF to essentially XOR it with the sprite pixel (which we now know is on).
		 *	We can’t XOR directly because the sprite pixel is either 1 or 0 while our video pixel is either 0x00000000 or 0xFFFFFFFF.
		 */
		{
			const byte VIDEO_WIDTH	= 64;
			const byte VIDEO_HEIGHT	= 32;
			(byte, byte, byte) parsedRegisters = ParseRegisters(ref opcode);
			byte Vx		= parsedRegisters.Item1;
			byte Vy		= parsedRegisters.Item2;
			byte height = parsedRegisters.Item3;

			// Wrap if going beyond screen boundaries
			byte xPos = (byte)(cpu.registers[Vx] % VIDEO_WIDTH);
			byte yPos = (byte)(cpu.registers[Vy] % VIDEO_HEIGHT);

			cpu.registers[cpu.Vf] = 0;
			//uint *screenPixel;
			byte spritePixel;
			byte spriteByte;

			for (byte row = 0; row < height; ++row)
			{
				spriteByte = memory.liveMem[cpu.IReg + row];

				for (byte col = 0; col < 8; ++col)
				{
					spritePixel = (byte)(spriteByte & (0x80u >> col));
					fixed (uint *screenPixel = &memory.videoMemory[(yPos + row) * VIDEO_WIDTH + (xPos + col)]) 
					{
						// Sprite pixel is on
						if (spritePixel > 0)
						{
							// Screen pixel also on - collision
							if (*screenPixel == 0xFFFFFFFF)
							{
								cpu.registers[15] = 1;
							}

							// Effectively XOR with the sprite pixel
							*screenPixel ^= 0xFFFFFFFF;
						}
					}
				}
			}
		}
		public void SKIP_Exxx(ushort opcode)
		{
			(byte, byte) parsedData			=	ParseData(ref opcode);		//	Compute once and store in a (byte, byte)
			byte Vx							=	parsedData.Item1;			//	Get register number
			byte op							=	parsedData.Item2;			//	Fetch data
			byte key						=	cpu.registers[Vx];          //	Fetch Value from selected register
			byte input						=	ReadInput();
			switch (op)
			{
				case 0x9E:		//Skip next instruction if key with the value of Vx is pressed.
					if (input	==	key) cpu.PC	+=	2;
					break;
				case 0xA1:		//	Skip next instruction if key with the value of Vx is not pressed.
					if (input	==	key) cpu.PC	+=	2;
					break;
			}
		}
		public void LD_Fxxx(ushort opcode)	//	0xFxDD
		{
			(byte, byte)	parsedData		=	ParseData(ref opcode);		// compute once and store in a (byte, byte)
			byte			Vx				=	parsedData.Item1;			// Get register number
			byte			op				=	parsedData.Item2;           // fetch data
			switch(op)
			{
				case	0x07:   //	Set Vx = delay timer value.
					cpu.registers[Vx]	=	cpu.delayTimer;
					break;
				case	0x0A:   //	Wait for a key press, store the value of the key in Vx.
					byte	key			=	ReadInput();
					break;
				case	0x15:
					cpu.delayTimer		=	cpu.registers[Vx];
					break;
				case	0x18:   //	Set sound timer = Vx.
					cpu.soundTimer		=	cpu.registers[Vx];
					break;
				case	0x1E:   //	Set I = I + Vx.
					cpu.IReg			+=	cpu.registers[Vx];
					break;
				case	0x29:   //	Set I = location of sprite for digit Vx.
					byte digit = cpu.registers[Vx];
					cpu.IReg = memory.fontSet[(byte)5 * digit];
					break;

				case	0x33:   //	Store BCD representation of Vx in memory locations I, I+1, and I+2.
					byte value = cpu.registers[Vx];
					// Ones-place
					memory.liveMem[cpu.IReg + 2] = (byte)(value % 10);
					value /= 10;

					// Tens-place
					memory.liveMem[cpu.IReg + 1] = (byte)(value % 10);
					value /= 10;

					// Hundreds-place
					memory.liveMem[cpu.IReg] = (byte)(value % 10);
					break;
				case	0x55:   //	Store registers V0 through Vx in memory starting at location I.
					for (byte i = 0; i <= Vx; ++i)
					{
						memory.liveMem[cpu.IReg + i] = cpu.registers[i];
					}
					break;
				case	0x65:   //	Read registers V0 through Vx from memory starting at location I.
					for (byte i = 0; i <= Vx; ++i)
					{
						cpu.registers[i] = memory.liveMem[cpu.IReg + i];
					}
					break;

			}
		}
	}
}
