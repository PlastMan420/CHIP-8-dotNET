using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET
{
    class InstructionSet : IInstructionSet
    {
        public void Call_addr()
        {
            throw new NotImplementedException();
        }

        public void CLS()
        {   // Clear video buffer (clears display)
            Memory.videoMemory = null;
        }

        public void JP_addr()
        {
            --CHIP8.SP;
        }

        public void RET()
        {
            throw new NotImplementedException();
        }
    }
}
