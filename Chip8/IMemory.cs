using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET
{
    interface IMemory
    {
        byte[] programMemory { get; set; }
        public void InitProgram();
        public void InitMemory();

    }
}
