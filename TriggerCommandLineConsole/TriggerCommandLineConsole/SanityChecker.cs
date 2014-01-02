using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TriggerCommandLineConsole
{
	public class SanityChecker
	{
		const string ARGUMENTS = "check";

		readonly IProcessWrapperFactory _factory;
		readonly string _forgePath;
		readonly string _srcPath;

		/// <summary>
		/// Performs sanity checks on your Trigger.io source
		/// </summary>
		/// <param name="fac">A IProcessWrapper factory</param>
		/// <param name="forgePath">Path to Trigger.io command line, forge.exe</param>
		/// <param name="pathToSource">Path to Trigger.io source to validate</param>
		public SanityChecker(IProcessWrapperFactory fac, string forgePath, string pathToSource)
		{
			if (fac == null)
				throw new ArgumentNullException("fac");
			if (String.IsNullOrEmpty(forgePath))
				throw new ArgumentNullException("forgePath");
			if (!File.Exists(forgePath))
				throw new ArgumentException("forgePath");
			if (String.IsNullOrEmpty(pathToSource))
				throw new ArgumentNullException("pathToSource");
			if (!Directory.Exists(pathToSource))
				throw new ArgumentException("pathToSource");

			_factory = fac;
			_forgePath = forgePath;
			_srcPath = pathToSource;
		}

		public bool IsSane()
		{
			IProcessWrapper proc = _factory.CreateProcess(_forgePath, _srcPath, ARGUMENTS);
			proc.Run();

			LastResults = proc.GetStandardErrorOutput();

			//if any line of output from Forge starts with [WARNING] or [ERROR], something went wrong with check
			return !Regex.IsMatch(LastResults, @"^\[\s*WARNING|ERROR\]", RegexOptions.Multiline);
		}

		public string LastResults { get; set; }
	}
}
