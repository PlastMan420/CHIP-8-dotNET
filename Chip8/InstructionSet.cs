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
        public InstructionSet(Memory _memory, CPU _cpu)
        {
            memory = _memory;
            cpu = _cpu;
        }

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
            byte Vx     = (byte)((opcode & 0x0F00u) >> 8);
            byte data   = (byte)(opcode & 0x00FFu);

            cpu.registers[Vx] += data;
        }

        public void FetchXY(ref byte Vx, ref byte Vy, ref ushort opcode) // Helper method
        {
            Vx = (byte)((opcode & 0x0F00u) >> 8);
            Vy = (byte)((opcode & 0x00F0u) >> 4);
        }
        public void LD_8xy0(ushort opcode)  // Set Vx = Vy.
        {
            byte Vx = 0,
                 Vy = 0;
            FetchXY(ref Vx, ref Vy, ref opcode);
            cpu.registers[Vx] = cpu.registers[Vy];
        }
        public void OR_8xy1(ushort opcode)  // Set Vx = Vx OR Vy.
        {
            byte Vx = 0,
                 Vy = 0;
            FetchXY(ref Vx, ref Vy, ref opcode);
            cpu.registers[Vx] |= cpu.registers[Vy];
        }
        public void AND_8xy2(ushort opcode) // Set Vx = Vx AND Vy.
        {
            byte Vx = 0,
                 Vy = 0;
            FetchXY(ref Vx, ref Vy, ref opcode);
            cpu.registers[Vx] &= cpu.registers[Vy];
        }
        public void XOR_8xy3(ushort opcode) // Set Vx = Vx XOR Vy.
        {
            byte Vx = 0,
                 Vy = 0;
            FetchXY(ref Vx, ref Vy, ref opcode);
            cpu.registers[Vx] ^= cpu.registers[Vy];
        }
    }
}
