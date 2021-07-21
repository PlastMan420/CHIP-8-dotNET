/*
 * This defines the CPU as a structure.
 * Execusion.cs and InstructionSet.cs define the CPU's execusion.
 */
namespace CHIP_8_dotNET.Chip8
{
	class CPU
	{
		// Registers
		public byte[] registers = new byte[16];    // x16 8-bit general purpose registers
		public readonly int Vf = 15;                     // flag register index
		public ushort IReg;                        // index register
		public ushort PC;                          // program counter
		public byte delayTimer;
		public byte soundTimer;

		public CPU()
		{
		}

		public void InitCPU()
		{
			this.PC = 0x200;
			this.IReg = 0;
			this.delayTimer = 0;
			this.soundTimer = 0;
		}
	}
}

/* https://austinmorlan.com/posts/chip8_emulator/#chip-8-description
 * 8-bit Delay Timer
The CHIP-8 has a simple timer used for timing. If the timer value is zero, it stays zero. 
If it is loaded with a value, it will decrement at a rate of 60Hz.
Rather than making sure that the delay timer actually decrements at a rate of 60Hz, 
I just decrement it at whatever rate we have the cycle clock set to which has worked fine for all the games I’ve tested.


8-bit Sound Timer
The CHIP-8 also has another simple timer used for sound. Its behavior is the same (decrementing at 60Hz if non-zero), 
but a single tone will buzz when it’s non-zero. Programmers used this for simple sound emission.
While I do have a sound timer in my implementation, I opted to not bother with making the application actually emit any sound. 
See here for a way to generate a tone with SDL.
 */
