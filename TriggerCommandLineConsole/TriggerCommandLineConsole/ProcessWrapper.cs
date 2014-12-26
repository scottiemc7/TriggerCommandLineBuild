using System.Diagnostics;
using System.Text;

namespace TriggerCommandLineConsole
{
	class ProcessWrapper : IProcessWrapper
	{
		private Process _process;
        private readonly StringBuilder _outputBuilder = new StringBuilder();
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
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ErrorDataReceived += _process_OutputDataReceived;
		}

        public void SetOutputHandler(DataReceivedEventHandler handler)
        {
            _process.ErrorDataReceived += handler;
            _process.OutputDataReceived += handler;
        }

        void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _outputBuilder.AppendLine(e.Data);
        }

		public void Run()
		{
			_process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
			_process.WaitForExit();
            _process.Dispose();
            _process = null;
		}

		public string GetStandardErrorOutput()
		{
			//return _process.StandardError.ReadToEnd();
            return _outputBuilder.ToString();
		}
	}
}
