using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Moq;
using TriggerCommandLineConsole;

namespace TriggerCommandLineConsoleTest
{
	[TestClass]
	public class CommandLineBuilderTests
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
		string _iosCertPath;
		string _iosProfilePath;
		string _andKeystorePath;

		[TestInitialize]
		public void Init()
		{
			_sanityDir = Path.Combine(TestContext.TestDeploymentDir, "sanityDir");
			_pathThatExists1 = Path.Combine(_sanityDir, "testDir1");
			_fakeForgePath = String.Format("{0}\\forge.exe", _pathThatExists1);
			_pathThatExists2 = Path.Combine(_sanityDir, "testDir2");
			_iosCertPath = String.Format("{0}\\cert.p12", _pathThatExists1);
			_iosProfilePath = String.Format("{0}\\profile.mobileprovision", _pathThatExists1);
			_andKeystorePath = String.Format("{0}\\keystore.keystore", _pathThatExists1);

			Directory.CreateDirectory(_sanityDir);
			Directory.CreateDirectory(_pathThatExists1);
			Directory.CreateDirectory(_pathThatExists2);
			File.Create(_fakeForgePath).Close();
			File.Create(_iosCertPath).Close();
			File.Create(_iosProfilePath).Close();
			File.Create(_andKeystorePath).Close();
		}

		[TestCleanup]
		public void Cleanup()
		{
			Directory.Delete(_sanityDir, true);
		}

		[TestMethod]
		public void BuildiOSFails_If_ErrorOutputContainsWarningOrError()
		{
			MockRepository repo = new MockRepository(MockBehavior.Loose);
			Mock<IProcessWrapperFactory> mockProcessFactory = repo.Create<IProcessWrapperFactory>();
			Mock<IProcessWrapper> mockProcess = repo.Create<IProcessWrapper>();

			mockProcessFactory.Setup(a => a.CreateProcess(It.Is<String>(s => s == _fakeForgePath), It.Is<String>(s => s == _pathThatExists1), It.IsAny<String>())).Returns(mockProcess.Object);
			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.GOODBUILD);

			IOSResources iosRes = new IOSResources() { CertificatePassword = "fakepass", CertificatePath = _iosCertPath, ProfilePath = _iosProfilePath };
			CommandLineBuilder builder = new CommandLineBuilder(mockProcessFactory.Object, _fakeForgePath, _pathThatExists1);

			Assert.IsTrue(builder.BuildiOS(iosRes));

			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.BADBUILD);
						
			Assert.IsFalse(builder.BuildiOS(iosRes));
		}

		[TestMethod]
		public void PackageiOSFails_If_ErrorOutputContainsWarningOrError()
		{
			MockRepository repo = new MockRepository(MockBehavior.Loose);
			Mock<IProcessWrapperFactory> mockProcessFactory = repo.Create<IProcessWrapperFactory>();
			Mock<IProcessWrapper> mockProcess = repo.Create<IProcessWrapper>();

			mockProcessFactory.Setup(a => a.CreateProcess(It.Is<String>(s => s == _fakeForgePath), It.Is<String>(s => s == _pathThatExists1), It.IsAny<String>())).Returns(mockProcess.Object);
			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.GOODIOSPACKAGE);

			IOSResources iosRes = new IOSResources() { CertificatePassword = "fakepass", CertificatePath = _iosCertPath, ProfilePath = _iosProfilePath };
			CommandLineBuilder builder = new CommandLineBuilder(mockProcessFactory.Object, _fakeForgePath, _pathThatExists1);

			Assert.IsNotNull(builder.PackageiOS(iosRes));

			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.BADIOSPACKAGE);

			Assert.IsNull(builder.PackageiOS(iosRes));
		}

		[TestMethod]
		public void BuildAndroidFails_If_ErrorOutputContainsWarningOrError()
		{
			MockRepository repo = new MockRepository(MockBehavior.Loose);
			Mock<IProcessWrapperFactory> mockProcessFactory = repo.Create<IProcessWrapperFactory>();
			Mock<IProcessWrapper> mockProcess = repo.Create<IProcessWrapper>();

			mockProcessFactory.Setup(a => a.CreateProcess(It.Is<String>(s => s == _fakeForgePath), It.Is<String>(s => s == _pathThatExists1), It.IsAny<String>())).Returns(mockProcess.Object);
			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.GOODBUILD);

			AndroidResources andRes = new AndroidResources() { SDKPath = _pathThatExists2, KeystorePath = _andKeystorePath, KeyAlias = "keyalias", KeyPassword = "keypass", KeystorePassword = "keystorepass" };
			CommandLineBuilder builder = new CommandLineBuilder(mockProcessFactory.Object, _fakeForgePath, _pathThatExists1);

			Assert.IsTrue(builder.BuildAndroid(andRes));

			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.BADBUILD);

			Assert.IsFalse(builder.BuildAndroid(andRes));
		}

		[TestMethod]
		public void PackageAndroidFails_If_ErrorOutputContainsWarningOrError()
		{
			MockRepository repo = new MockRepository(MockBehavior.Loose);
			Mock<IProcessWrapperFactory> mockProcessFactory = repo.Create<IProcessWrapperFactory>();
			Mock<IProcessWrapper> mockProcess = repo.Create<IProcessWrapper>();

			mockProcessFactory.Setup(a => a.CreateProcess(It.Is<String>(s => s == _fakeForgePath), It.Is<String>(s => s == _pathThatExists1), It.IsAny<String>())).Returns(mockProcess.Object);
			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.GOODANDPACKAGE);

			AndroidResources andRes = new AndroidResources() { SDKPath = _pathThatExists2, KeystorePath = _andKeystorePath, KeyAlias = "keyalias", KeyPassword = "keypass", KeystorePassword = "keystorepass" };
			CommandLineBuilder builder = new CommandLineBuilder(mockProcessFactory.Object, _fakeForgePath, _pathThatExists1);

			Assert.IsNotNull(builder.PackageAndroid(andRes));

			mockProcess.Setup(p => p.GetStandardErrorOutput()).Returns(TriggerCommandLineConsoleTest.Properties.Resources.BADANDPACKAGE);

			Assert.IsNull(builder.PackageAndroid(andRes));
		}
	}
}
