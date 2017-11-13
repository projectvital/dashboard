using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects
{
    public class Warning : Exception
    {
        public Warning(string message)
            : base(message)
        {
        }
    }
}
