using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerCommandLineConsole
{
	class DirectoryMover : IDisposable
	{
		readonly List<string> _originalDirs = new List<string>();
		readonly List<string> _tempDirs = new List<string>();
		public DirectoryMover(string rootDir, string dirs)
		{
			if (String.IsNullOrEmpty(rootDir) || String.IsNullOrEmpty(dirs))
				return;

			rootDir = rootDir.TrimEnd('\\');

			//copy every directory to temp storage
			foreach (string dir in dirs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
			{
				string	strippedDir = dir.TrimStart('\\').TrimEnd('\\'),
						fullPath = String.Format("{0}\\{1}", rootDir, strippedDir),
						tempPath = String.Format("{0}\\{1}\\{2}", Path.GetTempPath(), Guid.NewGuid(), strippedDir);

				_originalDirs.Add(fullPath);
				_tempDirs.Add(tempPath);
				DirectoryCopy(fullPath, tempPath, true);
			}//end foreach

			//delete all of the original dirs only after everything has been copied to temp storage
			foreach (string dir in _originalDirs)
				Directory.Delete(dir, true);
		}

		public void Dispose()
		{
			//move every directory back
			for (int i = 0; i < _originalDirs.Count; i++)
			{
				DirectoryCopy(_tempDirs[i], _originalDirs[i], true);
				Directory.Delete(_tempDirs[i], true);
			}//end for
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
