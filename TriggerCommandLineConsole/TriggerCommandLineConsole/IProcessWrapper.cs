using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerCommandLineConsole
{
	public interface IProcessWrapper
	{
		void Run();
		String GetStandardErrorOutput();
        void SetOutputHandler(DataReceivedEventHandler handler);
	}
}
