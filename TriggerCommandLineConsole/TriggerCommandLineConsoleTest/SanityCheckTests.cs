using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriggerCommandLineConsole;
using System.IO;
using Moq;

namespace TriggerCommandLineConsoleTest
{
	[TestClass]
	public class SanityCheckTests
	{
		public TestContext TestContext
		{
			get;
			set;
		}

		string _sanityDir;
		string _pathThatExists1;
		string _fakeForgePath;
		string _pathThatExists2;

		[TestInitialize]
		public void Init()
		{
			_sanityDir = Path.Combine(TestContext.TestDeploymentDir, "sanityDir");
			_pathThatExists1 = Path.Combine(_sanityDir, "testDir1");
			_fakeForgePath = String.Format("{0}\\forge.exe", _pathThatExists1);
			_pathThatExists2 = Path.Combine(_sanityDir, "testDir2");			

			Directory.CreateDirectory(_sanityDir);
			Directory.CreateDirectory(_pathThatExists1);
			Directory.CreateDirectory(_pathThatExists2);
			File.Create(_fakeForgePath).Close();
		}

		[TestCleanup]
		public void Cleanup()
		{
			Directory.Delete(_sanityDir, true);
		}

		[TestMethod]
		public void IsSaneCheckFailsIfInvalidArguments()
		{
			string pathThatDoesntExist = Path.Combine(_sanityDir, "testDir3");

			MockRepository repo = new MockRepository(MockBehavior.Loose);
			Mock<IProcessWrapperFactory> mockProcessFactory = repo.Create<IProcessWrapperFactory>();

			//passing in null values should fail
			try
			{
				SanityChecker check = new SanityChecker(null, _fakeForgePath, _pathThatExists2);
				Assert.Fail();
			}
			catch (ArgumentNullException) { }

			try
			{
				SanityChecker check = new SanityChecker(mockProcessFactory.Object, null, _pathThatExists2);
				Assert.Fail();
			}
			catch (ArgumentNullException) { }

			try
			{
				SanityChecker check = new SanityChecker(mockProcessFactory.Object, _fakeForgePath, null);
				Assert.Fail();
			}
			catch (ArgumentNullException) { }

			//passing in a dir instead of a path to a file should fail
			try
			{
				SanityChecker check = new SanityChecker(mockProcessFactory.Object, _pathThatExists1, _pathThatExists2);
				Assert.Fail();
			}
			catch (ArgumentException) { }

			//passing in a non existant dir should fail
			try
			{
				SanityChecker check = new SanityChecker(mockProcessFactory.Object, _fakeForgePath, pathThatDoesntExist);
				Assert.Fail();
			}
			catch (ArgumentException) { }
		}

		[TestMethod]
		public void IsSaneCheckFails_If_ErrorOutputContainsWarningOrError()
		{
			MockRepository repo = new MockRepository(MockBehavior.Loose);
			Mock<IProcessWrapperFactory> mockProcessFactory = repo.Create<IProcessWrapperFactory>();
			Mock<IProcessWrapper> mockProcess = repo.Create<IProcessWrapper>();

			mockProcessFactory.Setup(a => a.CreateProcess(It.Is<String>(s => s == _fakeForgePath), It.Is<String>(s => s == _pathThatExists1), It.IsAny<String>())).Returns(mockProcess.Object);
			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.GOODSANITYCHECK);

			SanityChecker check = new SanityChecker(mockProcessFactory.Object, _fakeForgePath, _pathThatExists1);
			Assert.IsTrue(check.IsSane());

			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.BADSANITYCHECK);

			check = new SanityChecker(mockProcessFactory.Object, _fakeForgePath, _pathThatExists1);
			Assert.IsFalse(check.IsSane());
		}
	}
}
