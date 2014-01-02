using System.Diagnostics;

namespace TriggerCommandLineConsole
{
	class ProcessWrapper : IProcessWrapper
	{
		private readonly Process _process;
		public ProcessWrapper(string path, string workingDirectory, string arguments)
		{
			ProcessStartInfo si = new ProcessStartInfo();
			si.CreateNoWindow = true;
			si.FileName = path;
			si.WorkingDirectory = workingDirectory;
			si.Arguments = arguments;
			si.RedirectStandardError = true;
			si.RedirectStandardOutput = true;
			si.RedirectStandardInput = true;
			si.UseShellExecute = false;

			_process = new Process();
			_process.StartInfo = si;
		}

		public void Run()
		{
			_process.Start();
			_process.WaitForExit();
		}

		public string GetStandardErrorOutput()
		{
			return _process.StandardError.ReadToEnd();
		}
	}
}
