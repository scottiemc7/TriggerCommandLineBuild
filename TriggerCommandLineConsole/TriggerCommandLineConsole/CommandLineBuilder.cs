using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TriggerCommandLineConsole
{
	public class CommandLineBuilder
	{
		const string FAILPATTERN = @"^\[\s*ERROR\]";
		const string PATHPATTERNIOS = @"^\[\s*INFO\]\s*created\s*IPA:\s*(?<path>[^\r\n]+)";
		const string PATHPATTERNANDROID = @"^\[\s*INFO\]\s*created\s*APK:\s*(?<path>[^\r\n]+)";

		readonly IProcessWrapperFactory _factory;
		readonly string _forgePath;
		readonly string _srcPath;

		public CommandLineBuilder(IProcessWrapperFactory fac, string forgePath, string pathToSource)
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

		public bool BuildAndroid(AndroidResources resources)
		{
			LastBuildOutput = null;

			ValidateAndroidResources(resources);

			string args = String.Format("build android --username=\"{0}\" --password=\"{1}\" --android.sdk \"{2}\" --android.profile.keystore \"{3}\" --android.profile.keyalias \"{4}\" --android.profile.storepass \"{5}\" --android.profile.keypass \"{6}\"", resources.UserName, resources.Password, resources.SDKPath, resources.KeystorePath, resources.KeyAlias, resources.KeystorePassword, resources.KeyPassword);

			//build app
			Debug.WriteLine(String.Format("Calling forge.exe {0}", args));
			IProcessWrapper buildProc = _factory.CreateProcess(_forgePath, _srcPath, args);
			buildProc.Run();
			LastBuildOutput = buildProc.GetStandardErrorOutput();

			//if any line of output from Forge starts with [ERROR], something went wrong with build
			return !Regex.IsMatch(LastBuildOutput, FAILPATTERN, RegexOptions.Multiline);
		}

		public string PackageAndroid(AndroidResources resources)
		{
			LastBuildOutput = null;

			ValidateAndroidResources(resources);

			string args = String.Format("package android --username=\"{0}\" --password=\"{1}\" --android.sdk \"{2}\" --android.profile.keystore \"{3}\" --android.profile.keyalias \"{4}\" --android.profile.storepass \"{5}\" --android.profile.keypass \"{6}\"", resources.UserName, resources.Password, resources.SDKPath, resources.KeystorePath, resources.KeyAlias, resources.KeystorePassword, resources.KeyPassword);
			Debug.WriteLine(String.Format("Calling forge.exe {0}", args));

			IProcessWrapper proc = _factory.CreateProcess(_forgePath, _srcPath, args);
			proc.Run();
			LastBuildOutput = proc.GetStandardErrorOutput();

			//if any line of output from Forge starts with [ERROR], something went wrong with build
			if (!Regex.IsMatch(LastBuildOutput, FAILPATTERN, RegexOptions.Multiline))
			{
				//last line of output is [INFO] "path"
				string path = Regex.Match(LastBuildOutput, PATHPATTERNANDROID, RegexOptions.Multiline).Groups["path"].Value;
				Debug.WriteLine(String.Format("Android package path: {0}", path));
				return path;
			}
			else
			{
				Debug.WriteLine("Build failure");
				return null;
			}//end if
		}

		private void ValidateAndroidResources(AndroidResources resources)
		{
			if (String.IsNullOrEmpty(resources.UserName) || String.IsNullOrEmpty(resources.Password))
				throw new ArgumentNullException("Username and password required", "resources");
			if (!Directory.Exists(resources.SDKPath))
				throw new ArgumentException("Unknown sdk path", "resources.SDKPath");
			if (!File.Exists(resources.KeystorePath))
				throw new ArgumentException("Unknown keystore path", "resources.KeystorePath");
		}

		private void ValidateiOSResources(IOSResources resources)
		{
			if (String.IsNullOrEmpty(resources.UserName) || String.IsNullOrEmpty(resources.Password))
				throw new ArgumentNullException("Username and password required", "resources");
			if (!File.Exists(resources.ProfilePath))
				throw new ArgumentException("Unknown profile path", "resources.ProfilePath");
			if (!File.Exists(resources.CertificatePath))
				throw new ArgumentException("Unknown certificate path", "resources.CertificatePath");
		}

		public bool BuildiOS(IOSResources resources)
		{
			LastBuildOutput = null;

			ValidateiOSResources(resources);

			string args = String.Format("build ios --username=\"{0}\" --password=\"{1}\"  --ios.profile.provisioning_profile \"{2}\" --ios.profile.developer_certificate_path \"{3}\" --ios.profile.developer_certificate_password \"{4}\"", resources.UserName, resources.Password, resources.ProfilePath, resources.CertificatePath, resources.CertificatePassword);

			//build app
			Debug.WriteLine(String.Format("Calling forge.exe {0}", args));
			IProcessWrapper buildProc = _factory.CreateProcess(_forgePath, _srcPath, args);
			buildProc.Run();
			LastBuildOutput = buildProc.GetStandardErrorOutput();

			//if any line of output from Forge starts with [ERROR], something went wrong with build
			return !Regex.IsMatch(LastBuildOutput, FAILPATTERN, RegexOptions.Multiline);
		}

		public string PackageiOS(IOSResources resources)
		{
			LastBuildOutput = null;

			ValidateiOSResources(resources);

			string args = String.Format("package ios --username=\"{0}\" --password=\"{1}\" --ios.profile.provisioning_profile \"{2}\" --ios.profile.developer_certificate_path \"{3}\" --ios.profile.developer_certificate_password \"{4}\"", resources.UserName, resources.Password, resources.ProfilePath, resources.CertificatePath, resources.CertificatePassword);
			Debug.WriteLine(String.Format("Calling forge.exe {0}", args));

			IProcessWrapper proc = _factory.CreateProcess(_forgePath, _srcPath, args);
			proc.Run();
			LastBuildOutput = proc.GetStandardErrorOutput();

			//if any line of output from Forge starts with [ERROR], something went wrong with build
			if (!Regex.IsMatch(LastBuildOutput, FAILPATTERN, RegexOptions.Multiline))
			{
				//last line of output is [INFO] "path"
				string path = Regex.Match(LastBuildOutput, PATHPATTERNIOS, RegexOptions.Multiline).Groups["path"].Value;
				Debug.WriteLine(String.Format("iOS package path: {0}", path));
				return path;
			}
			else
			{
				Debug.WriteLine("Build failure");
				return null;
			}//end if
		}

		public string LastBuildOutput { get; set; }
	}

	public struct AndroidResources
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string SDKPath { get; set; }
		public string KeystorePath { get; set; }
		public string KeyAlias { get; set; }
		public string KeyPassword { get; set; }
		public string KeystorePassword { get; set; }
	}

	public struct IOSResources
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string CertificatePath { get; set; }
		public string ProfilePath { get; set; }
		public string CertificatePassword { get; set; }
	}
}
