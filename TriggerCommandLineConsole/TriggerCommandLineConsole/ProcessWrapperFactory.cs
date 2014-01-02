
namespace TriggerCommandLineConsole
{
	class ProcessWrapperFactory : IProcessWrapperFactory
	{
		public IProcessWrapper CreateProcess(string path, string workingDirectory, string arguments)
		{
			return new ProcessWrapper(path, workingDirectory, arguments);
		}
	}
}
