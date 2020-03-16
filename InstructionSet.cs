using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET
{
    class InstructionSet : IInstructionSet
    {
        private readonly ICPU        cpu        = new ICPU();
        private readonly IMemory     memory     = new IMemory();
        InstructionSet(IMemory _memory, ICPU _cpu)
        {
            memory = _memory;
            cpu = _cpu;
        }
        public void Call_addr()
        {
            throw new NotImplementedException();
        }

        public void CLS()
        {   // Clear video buffer (clears display)
            memory.videoMemory = null;
        }

        public void JP_addr()
        {
            --cpu.SP;
            cpu.PC = memory.Stack[cpu.SP];
        }

        public void RET()
        {
            throw new NotImplementedException();
        }
    }
}
