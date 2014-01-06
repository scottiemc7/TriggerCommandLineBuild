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
	class Program
	{
		static void Main(string[] args)
		{
			ProgressBeginBuild();

			var options = new Options();
			if (!CommandLine.Parser.Default.ParseArguments(args, options))
			{
				ProgressEndBuild(false, "Argument failure");
				return;
			}

			if (String.IsNullOrEmpty(options.SrcPath) || !Directory.Exists(options.SrcPath))
			{
				Console.WriteLine("Path to source is invalid");
				ProgressEndBuild(false, "Argument failure");
				return;
			}//end if

			if (String.IsNullOrEmpty(options.PackagePath) || !Directory.Exists(options.PackagePath))
			{
				Console.WriteLine("Package path is invalid");
				ProgressEndBuild(false, "Argument failure");
				return;
			}//end if

			String fullForgePath = options.ForgePath.TrimEnd('\\') + "\\forge.exe";
			if (String.IsNullOrEmpty(options.ForgePath) || !File.Exists(fullForgePath))
			{
				Console.WriteLine("Path to forge.exe is invalid");
				ProgressEndBuild(false, "Argument failure");
				return;
			}//end if

			if (!options.Android && !options.iOS)
			{
				Console.WriteLine("At least one platform (Android or iOS) needs to be selected");
				ProgressEndBuild(false, "Argument failure");
				return;
			}//end if

			if (options.Android && (String.IsNullOrEmpty(options.AndroidKeystorePass) ||
									String.IsNullOrEmpty(options.AndroidKeystorePath) ||
									!File.Exists(options.AndroidKeystorePath) ||
									String.IsNullOrEmpty(options.AndroidKeyAlias) ||
									String.IsNullOrEmpty(options.AndroidKeyPass) ||
									String.IsNullOrEmpty(options.AndroidSDKPath) ||
									!Directory.Exists(options.AndroidSDKPath)))
			{
				Console.WriteLine("At least one Android required argument is missing or file could not be found");
				ProgressEndBuild(false, "Argument failure");
				return;
			}//end if

			if (options.iOS && (String.IsNullOrEmpty(options.iOSCertificatePass) ||
								String.IsNullOrEmpty(options.iOSCertificatePath) ||
								!File.Exists(options.iOSCertificatePath) ||
								String.IsNullOrEmpty(options.iOSProfilePath) ||
								!File.Exists(options.iOSProfilePath)))
			{
				Console.WriteLine("At least one iOS required argument is missing or file could not be found");
				ProgressEndBuild(false, "Argument failure");
				return;
			}//end if

			options.PackagePath = options.PackagePath.TrimEnd('\\');

			//send debug to console.out
#if DEBUG
			TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
			Debug.Listeners.Add(myWriter);
#endif

			IProcessWrapperFactory fac = new ProcessWrapperFactory();
			CommandLineBuilder builder = new CommandLineBuilder(fac, fullForgePath, options.SrcPath);
			IOSResources iosRes = new IOSResources() { CertificatePath = options.iOSCertificatePath, CertificatePassword = options.iOSCertificatePass, ProfilePath = options.iOSProfilePath };
			AndroidResources andRes = new AndroidResources() { KeyAlias = options.AndroidKeyAlias, KeyPassword = options.AndroidKeyPass, KeystorePassword = options.AndroidKeystorePass, KeystorePath = options.AndroidKeystorePath, SDKPath = options.AndroidSDKPath };

			//attempt to build before we package or run sanity check
			if (options.iOS)
			{
				ProgressMessage("Begin iOS build");
				using (DirectoryMover m = new DirectoryMover(options.SrcPath, options.iOSIgnore))
				{
					bool ret = builder.BuildiOS(iosRes);
					Console.WriteLine(builder.LastBuildOutput);
					if (!ret)
					{
						ProgressEndBuild(false, "iOS build failed");
						return;
					}//end if
				}//end using
			}//end if

			if (options.Android)
			{
				ProgressMessage("Begin Android build");
				using (DirectoryMover m = new DirectoryMover(options.SrcPath, options.AndroidIgnore))
				{
					bool ret = builder.BuildAndroid(andRes);
					Console.WriteLine(builder.LastBuildOutput);
					if (!ret)
					{
						ProgressEndBuild(false, "Android build failed");
						return;
					}//end if
				}//end using
			}//end if

			//sanity check
			if (options.SanityCheck)
			{
				ProgressMessage("Performing sanity check");
				SanityChecker c = new SanityChecker(fac, fullForgePath, options.SrcPath);
				bool isSane = c.IsSane();
				Console.WriteLine(c.LastResults);
				if (!isSane)
				{
					ProgressEndBuild(false, "Sanity check failed");
					return;
				}
				else
					ProgressMessage("Sanity check complete");
			}//end sanity check

			//ios packaging
			if (options.iOS)
			{
				ProgressMessage("Begin iOS packaging");
				string path = builder.PackageiOS(iosRes);
				Console.WriteLine(builder.LastBuildOutput);

				if (String.IsNullOrEmpty(path))
				{
					ProgressEndBuild(false, "iOS packaging failed");
					return;
				}
				else
				{
					string movePath = String.Format("{0}\\ios.ipa", options.PackagePath);
					if(File.Exists(movePath))
						File.Delete(movePath);
					File.Move(path, movePath);
					Console.WriteLine(String.Format("iOS package created at {0}", movePath));
				}//end if
			}//end ios
			
			//android packaging
			if (options.Android)
			{
				ProgressMessage("Begin Android packaging");
				string path = builder.PackageAndroid(andRes);
				Console.WriteLine(builder.LastBuildOutput);
				if (String.IsNullOrEmpty(path))
				{
					ProgressEndBuild(false, "Android packaging failed");
					return;
				}
				else
				{
					string movePath = String.Format("{0}\\android.apk", options.PackagePath);
					if(File.Exists(movePath))
						File.Delete(movePath);
					File.Move(path, movePath);
					Console.WriteLine(String.Format("Android package created at {0}", movePath));
				}//end if

				
			}//end android

			ProgressEndBuild(true, "Build complete");

#if DEBUG
			Console.ReadLine();
#endif
		}

		private static void ProgressBeginBuild()
		{
			Console.WriteLine("##teamcity[progressStart '']");
		}

		private static void ProgressEndBuild(bool success, string msg)
		{
			Console.WriteLine(String.Format("##teamcity[progressFinish 'Build Finished With {0}']", success ? "SUCCESS" : "FAILURE"));
			Console.WriteLine(String.Format("##teamcity[buildStatus status='{1}' text='{0}']", msg, success ? "SUCCESS" : "FAILURE"));
		}

		private static void ProgressMessage(string msg)
		{
			Console.WriteLine(String.Format("##teamcity[progressMessage '{0}']", msg));
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}
