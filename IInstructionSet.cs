﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_dotNET
{
    interface IInstructionSet
    {
        void CLS();
        void RET();
        void JP_addr();
        void Call_addr();

    }
}
