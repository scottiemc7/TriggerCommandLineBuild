using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriggerCommandLineConsole
{
    public interface IJSONValueSwapper
    {
        void Swap(string key, dynamic newValue);
    }
}
