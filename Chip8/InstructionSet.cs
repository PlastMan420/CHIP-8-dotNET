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
		public void FetchData(ref byte Vx, ref byte data, ref ushort opcode)
		{
			Vx = (byte)((opcode & 0x0F00u) >> 8);
			data = (byte)(opcode & 0x00FFu);
		}
		public void FetchXY(ref byte Vx, ref byte Vy, ref ushort opcode) // Helper method
		{
			Vx = (byte)((opcode & 0x0F00u) >> 8);
			Vy = (byte)((opcode & 0x00F0u) >> 4);
		}
		public byte randByte()    // returns a random byte 
		{
			var randGen = new Random();
			byte randomByte = (byte)randGen.Next(0, 255);
			return randomByte;
		} 
		/////////////////////////////////////////////////////////////////////////////////
		public void CLS() // 0x00E0
		{   // Clear video buffer (clears display)
			memory.videoMemory = null;
		}
		public void RET() // 0x00EE
		{
			--cpu.SP;
			cpu.PC = memory.stack[cpu.SP];
		}

		public void JMP(ushort opcode) //op: 0x1nnn --> 1000 + a 12 bit address
		{
			ushort address = (ushort)(opcode & 0x0FFFu); // Casting to prevent Int promotions
			cpu.PC = address;
		}
		public void CALL(ushort opcode) // 0x2nnn --> 2000 + a 12 bit address
		{
			ushort address = (ushort)(opcode & 0x0FFFu);

			memory.stack[cpu.SP] = (byte)cpu.PC;
			++cpu.SP;
			cpu.PC = address;
		}
		public void SEQ_3xkk(ushort opcode)   // 0x3XKK --> 0x3000 + (x = register number) + (kk = value)
		{
			byte Vx     = (byte)((opcode & 0x0F00u) >> 8); // Get register number
			byte data   = (byte)(opcode & 0x00FF);    // fetch data
			if (cpu.registers[Vx] == data)
				cpu.PC += 2;
		}
		public void SNE_4xkk(ushort opcode) // Skip next instruction if Vx != kk.
		{
			byte Vx     = (byte)((opcode & 0x0F00u) >> 8); // Get register number
			byte data   = (byte)(opcode & 0x00FFu);
			if (cpu.registers[Vx] != data)
				cpu.PC += 2;
		}
		public void SE_5xy0(ushort opcode)  // Skip next instruction if Vx = Vy.
		{
			byte Vx = (byte)((opcode & 0x0F00u) >> 8);
			byte Vy = (byte)((opcode & 0x00F0u) >> 4);
			if (cpu.registers[Vx] == cpu.registers[Vy])
				cpu.PC += 2;
		}

		public void LD_6xkk(ushort opcode)  // Set Vx = kk.
		{
			byte Vx     = (byte)((opcode & 0x0F00u) >> 8);
			byte data   = (byte)(opcode & 0x00FFu);

			cpu.registers[Vx] = data;
		}
		public void ADD_7xkk(ushort opcode) // Set Vx = Vx + kk.
		{
			byte Vx     = 0,
				 data   = 0;
			FetchData(ref Vx, ref data, ref opcode);
			cpu.registers[Vx] += data;
		}



		/// <summary>
		/// Opcode set 8xxx. they include logical and arithmetic operations
		/// </summary>
		/// <param name="opcode"></param>
		public void set_8000(ushort opcode)
		{
			byte Vx = 0,
				 Vy = 0;
			FetchXY(ref Vx, ref Vy, ref opcode);
			int op = opcode & 0x000F;
			switch (op)
			{
				case 0:         // Load Vy into Vx
					cpu.registers[Vx] = cpu.registers[Vy];
					break;

				case 1:        // AND
					cpu.registers[Vx] &= cpu.registers[Vy];
					break;

				case 2:       // OR
					cpu.registers[Vx] |= cpu.registers[Vy];
					break;

				case 3:      // XOR
					cpu.registers[Vx] ^= cpu.registers[Vy];
					break;

				case 4:      // summing 2 registers.
					int sum = (cpu.registers[Vx] + cpu.registers[Vy]);
					if (sum > 255u) cpu.registers[15] = 1; else cpu.registers[15] = 0;
					cpu.registers[Vx] = (byte)(sum & 0xFFu);
					break;

				case 5:     // subtracting Vy from Vx.
					if (cpu.registers[Vx] > cpu.registers[Vy]) cpu.registers[15] = 1; else cpu.registers[15] = 0;
					cpu.registers[Vx] -= cpu.registers[Vy];
					break;

				case 6:
					cpu.registers[15] = (byte)(cpu.registers[15] & 0x1u);
					cpu.registers[Vx] >>= 1;
					break;

				case 7:      // SUBN Vx, Vy         // Set Vx = Vy - Vx, set VF = NOT borrow.
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
			byte Vx = 0,
				 Vy = 0;
			FetchXY(ref Vx, ref Vy, ref opcode);
		}
		public void LD_Annn(ushort opcode)  //  set I = nnn
		{
			ushort address = (ushort)(opcode & 0x0FFFu);
			cpu.IReg = address;
		}
		public void JP_Bnnn(ushort opcode)  //  Jump to location nnn + V0.
		{
			ushort address = (ushort)(opcode & 0x0FFFu);
			cpu.PC = (ushort)(cpu.registers[0] + address);
		}
		public void RND_Cxkk(ushort opcode) //  Set Vx = random byte AND kk.
		{
			byte Vx = (byte)((opcode & 0x0F00u) >> 8);
			byte data = (byte)((opcode & 0x00FFu));

			cpu.registers[Vx] = (byte)(randByte() & data);
		}

		public void DRW_Dxyn(ushort opcode)
		{
			// TODO
		}
		public void SKIP_Exxx(ushort opcode)
		{
			byte Vx = (byte)((opcode & 0x0F00u) >> 8);
			byte op = (byte)((opcode & 0x00FFu));
			byte key = cpu.registers[Vx];
			switch (op)
			{
				case 0x9E:      //	Skip next instruction if key with the value of Vx is pressed.

			}
		}
	}
}
