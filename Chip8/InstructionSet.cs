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

		//	Method lookup table 
		//public Action<ushort> InstructionListDelegate;
		public Dictionary<int, Action<ushort>> InstructionList = new Dictionary<int, Action<ushort>>();

		public InstructionSet(ref Memory _memory, ref CPU _cpu)
		{
			memory = _memory;
			cpu = _cpu;

			//	Method lookup table 
			InstructionList.Add(0x0, this.SET_00xx);
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
			byte	rx		=	(byte)((opcode & 0x0F00u) >> 8);
			byte	data	=	(byte)(opcode & 0x00FFu);
			return	(rx, data);
		}
		public (byte, byte, byte) ParseRegisters(ref ushort opcode)
		{
			byte	rx	=	(byte)((opcode & 0x0F00) >> 8);
			byte	ry	=	(byte)((opcode & 0x00F0) >> 4);
			byte	op	=	(byte)(opcode & 0x000F);
			return (rx, ry, op);
		}
		public ushort ParseAddress(ref ushort opcode)
		{
			return (ushort)(opcode & 0x0FFFu); // min possible address is 0x200
			//return address;
		}
		public byte RandByte()    // returns a random byte 
		{
			var		randGen = new Random();
			byte	randomByte = (byte)randGen.Next(0, 255);
			return	randomByte;
		} 
		public byte ReadInput()
		{
			string input	= Console.ReadKey().KeyChar.ToString();    //	string representation of input char
			int key			= Convert.ToInt32(input);
			if (memory.keypad.Contains(key))
				return Convert.ToByte(key);

			return	ReadInput(); // 'wait' for a valid key input
		}
		/////////////////////////////////////////////////////////////////////////////////

		public void SET_00xx(ushort opcode)
		{
			
			(byte, byte) ParsedData = ParseData(ref opcode);
			byte op = (byte)(ParsedData.Item2);
			switch (op)
			{
				case 0x00E0:
					Array.Clear(memory.videoMemory, 0, 64*32);
					break;
				case 0x00EE: // stack POP
					cpu.PC = memory.stack.Pop();
					break;
				default:
					throw new System.ArgumentException("Invalid 00xx opcode");
			}
		}

		public void JMP_1nnn(ushort opcode) //	jump to nnn
		{
			cpu.PC								=	ParseAddress(ref opcode);
		}
		public void CALL_2nnn(ushort opcode) //	Store PC in stack then jump to nnn
		{
			memory.stack.Push(cpu.PC);
			cpu.PC								= ParseAddress(ref opcode);
		}
		public void SEQ_3xkk(ushort opcode)   // 0x3XKK --> 0x3000 + (x = register number) + (kk = value)
		{
			
			(byte, byte) parsedData				=	ParseData(ref opcode);	// compute once and store  in a (byte, byte)
			byte	rx							=	parsedData.Item1;			// Get register number
			byte	data						=	parsedData.Item2;			// fetch data
			if (cpu.registers[rx]	==	data)
				cpu.PC	+=	2;
		}
		public void SNE_4xkk(ushort opcode) // Skip next instruction if rx != kk.
		{
			
			(byte, byte)	parsedData			= ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte			rx					= parsedData.Item1;			// Get register number
			byte			data				= parsedData.Item2;			// fetch data
			if (cpu.registers[rx] != data)
				cpu.PC += 2;
		}
		public void SE_5xy0(ushort opcode)  // Skip next instruction if rx = ry.
		{
			
			(byte, byte, byte) parsedRegisters	= ParseRegisters(ref opcode);
			byte rx								= parsedRegisters.Item1;
			byte ry								= parsedRegisters.Item2;
			
			if (cpu.registers[rx] == cpu.registers[ry])
				cpu.PC += 2;
		}

		public void LD_6xkk(ushort opcode)  // Set rx = kk.
		{
			
			(byte, byte)	parsedData			= ParseData(ref opcode);    // compute once and store in a (byte, byte)
			byte			rx					= parsedData.Item1;         // Get register number
			byte			data				= parsedData.Item2;         // fetch data
			cpu.registers[rx]					= data;
		}
		public void ADD_7xkk(ushort opcode) // Set rx = rx + kk.
		{
			
			(byte, byte)	parsedData			= ParseData(ref opcode);    // compute once and store  in a (byte, byte)
			byte			rx					= parsedData.Item1;         // Get register number
			byte			data				= parsedData.Item2;         // fetch data
			cpu.registers[rx]					+= data;
		}

		public void Set_8000(ushort opcode)
		{
			
			(byte, byte, byte) parsedRegisters = ParseRegisters(ref opcode);
			byte rx = parsedRegisters.Item1;
			byte ry = parsedRegisters.Item2;
			byte op = parsedRegisters.Item3;
			switch (op)
			{
				case	0x0:         // Load ry into rx
					cpu.registers[rx] = cpu.registers[ry];
					break;

				case	0x1:        // AND
					cpu.registers[rx] &= cpu.registers[ry];
					break;

				case	0x2:       // OR
					cpu.registers[rx] |= cpu.registers[ry];
					break;

				case	0x3:      // XOR
					cpu.registers[rx] ^= cpu.registers[ry];
					break;

				case	0x4:      // summing 2 registers.
					byte sum = (byte)(cpu.registers[rx] + cpu.registers[ry]);
					if (sum > 255u) cpu.registers[cpu.Vf] = 1; else cpu.registers[cpu.Vf] = 0;
					cpu.registers[rx] = (byte)(sum & 0xFFu);
					break;

				case	0x5:     // subtracting ry from rx.
					cpu.registers[15] = (byte)(cpu.registers[rx] > cpu.registers[ry] ? 1 : 0);
					cpu.registers[rx] = (byte)((cpu.registers[rx] - cpu.registers[ry])); 
					break;

				case 0x6:
					cpu.registers[cpu.Vf]	= (byte)(cpu.registers[15] & 0x1);
					cpu.registers[rx]		= (byte)(cpu.registers[rx] >> 1);
					break;

				case 0x7:      // SUBN rx, ry         // Set rx = ry - rx, set VF = NOT borrow.
					cpu.registers[15] = (byte)(cpu.registers[ry] > cpu.registers[rx] ? 1 : 0);
					cpu.registers[rx] = (byte)((cpu.registers[ry] - cpu.registers[rx]));
					break;

				case 0xE:   
					//	If the most-significant bit of rx is 1, then VF is set to 1, otherwise to 0. Then rx is multiplied by 2.
					//	A left shift is performed(multiplication by 2), and the most significant bit is saved in Register VF.
					cpu.registers[cpu.Vf]	= (byte)(((cpu.registers[rx] & 0x80) == 0x80) ? 1 : 0);
					cpu.registers[rx]		= (byte)(cpu.registers[rx] << 1);
					break;
				default:
					throw new System.ArgumentException("Invalid 8xxx opcode");
			}

		}
		public void SNE_9xy0(ushort opcode) // 9xy0 - SNE rx, ry.  // Skip next instruction if rx != ry.
		{
			
			(byte, byte, byte) parsedRegisters = ParseRegisters(ref opcode);
			byte rx = parsedRegisters.Item1;
			byte ry = parsedRegisters.Item2;
			if (cpu.registers[rx] != cpu.registers[ry])
			{
				cpu.PC += 2;
			}
		}
		public void LD_Annn(ushort opcode)  //  set I = nnn
		{
			cpu.IReg						=	ParseAddress(ref opcode);
		}
		public void JP_Bnnn(ushort opcode)  //  Jump to location nnn + V0.
		{
			cpu.PC							=	(ushort)(cpu.registers[0] + ParseAddress(ref opcode));
		}
		public void RND_Cxkk(ushort opcode) //  Set rx = random byte AND kk.
		{
			
			(byte, byte) parsedData			=	ParseData(ref opcode);		// compute once and store in a (byte, byte)
			byte rx							=	parsedData.Item1;			// Get register number
			byte data						=	parsedData.Item2;			// fetch data

			cpu.registers[rx]				=	(byte)(RandByte() & data);
		}

		public unsafe void DRW_Dxyn(ushort opcode)
		//	Display n-byte sprite starting at memory location I at (rx, ry), set VF = collision.
		/*
		 *	We iterate over the sprite, row by row and column by column. 
		 *	We know there are eight columns because a sprite is guaranteed to be eight pixels wide.
		 *	If a sprite pixel is on then there may be a collision with what’s already being displayed,
		 *	so we check if our screen pixel in the same location is set. If so we must set the VF register to express collision.
		 *	Then we can just XOR the screen pixel with 0xFFFFFFFF to essentially XOR it with the sprite pixel (which we now know is on).
		 *	We can’t XOR directly because the sprite pixel is either 1 or 0 while our video pixel is either 0x00000000 or 0xFFFFFFFF.
		 */
		{

			try
			{
				const byte VIDEO_WIDTH = 64;
				const byte VIDEO_HEIGHT = 32;
				(byte, byte, byte) parsedRegisters = ParseRegisters(ref opcode);
				byte rx = parsedRegisters.Item1;
				byte ry = parsedRegisters.Item2;
				byte height = parsedRegisters.Item3;

				// Wrap if going beyond screen boundaries
				byte xPos = (byte)(cpu.registers[rx] % VIDEO_WIDTH);
				byte yPos = (byte)(cpu.registers[ry] % VIDEO_HEIGHT);

				cpu.registers[cpu.Vf] = 0;
				//uint *screenPixel;
				byte spritePixel;
				byte spriteByte;
				int index;
				
				
					for (byte row = 0; row < height; ++row) //	rows
					{
						//Console.WriteLine("CPU.IREG+row: {0:x}", cpu.IReg + row);
						spriteByte = memory.liveMem[cpu.IReg + row];

						for (byte col = 0; col < 8; ++col)  //	columns
						{
							spritePixel = (byte)(spriteByte & (0x80u >> col));
							index = (xPos + col) + (yPos + row) * VIDEO_WIDTH;
							if (index > 2047) continue;
							fixed (uint* screenPixel = &memory.videoMemory[index])
							{
								//Console.WriteLine("index: {0:x}", index);
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
			catch (Exception e)
			{
				Console.WriteLine(e.Message + ", DRAW instruction error");
			}
		}
		public void SKIP_Exxx(ushort opcode)
		{
			
			(byte, byte) parsedData			=	ParseData(ref opcode);		//	Compute once and store in a (byte, byte)
			byte rx							=	parsedData.Item1;			//	Get register number
			byte op							=	parsedData.Item2;			//	Fetch data
			byte key						=	cpu.registers[rx];          //	Fetch Value from selected register
			byte input						=	ReadInput();
			switch (op)
			{
				case 0x9E:		//Skip next instruction if key with the value of rx is pressed.
					if (input	==	key) cpu.PC	+=	2;
					break;
				case 0xA1:		//	Skip next instruction if key with the value of rx is not pressed.
					if (input	!=	key) cpu.PC	+=	2;
					break;
				default:
					throw new System.ArgumentException("Invalid Exxx opcode");
			}
		}
		public void LD_Fxxx(ushort opcode)	//	0xFxDD
		{
			
			(byte, byte)	parsedData		=	ParseData(ref opcode);		// compute once and store in a (byte, byte)
			byte			rx				=	parsedData.Item1;			// Get register number
			byte			op				=	parsedData.Item2;           // fetch data
			switch(op)
			{
				case	0x07:   //	Set rx = delay timer value.
					cpu.registers[rx]	=	cpu.delayTimer;
					break;
				case	0x0A:   //	Wait for a key press, store the value of the key in rx.
					byte	inputKEy			=	ReadInput();
					break;
				case	0x15:
					cpu.delayTimer		=	cpu.registers[rx];
					break;
				case	0x18:   //	Set sound timer = rx.
					cpu.soundTimer		=	cpu.registers[rx];
					break;
				case	0x1E:   //	Set I = I + rx.
					cpu.IReg			+=	cpu.registers[rx];
					break;
				case	0x29:   //	Set I = location of sprite for digit rx.
					cpu.IReg = (ushort)(5 * cpu.registers[rx]);
					break;

				case	0x33:   //	Store BCD representation of rx in memory locations I, I+1, and I+2.
					//byte value = cpu.registers[rx];
					memory.liveMem[cpu.IReg]		= (byte)(cpu.registers[rx] / 100);
					memory.liveMem[cpu.IReg + 1]	= (byte)((cpu.registers[rx] % 100) / 10);
					memory.liveMem[cpu.IReg + 2]	= (byte)(cpu.registers[rx] % 10);
					break;
				case	0x55:   //	Store registers V0 through rx in memory starting at location I.
					for (byte i = 0; i <= rx; ++i)
					{
						memory.liveMem[cpu.IReg + i] = cpu.registers[i];
					}
					break;
				case	0x65:   //	Read registers V0 through rx from memory starting at location I.
					for (byte i = 0; i <= rx; ++i)
					{
						cpu.registers[i] = memory.liveMem[cpu.IReg + i];
					}
					break;
				default:
					throw new System.ArgumentException("Invalid Fxxx opcode");

			}
		}
	}
}
