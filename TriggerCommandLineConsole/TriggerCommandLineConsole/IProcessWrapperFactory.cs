using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerCommandLineConsole
{
	public interface IProcessWrapperFactory
	{
		IProcessWrapper CreateProcess(string path, string workingDirectory, string arguments);
	}
}
