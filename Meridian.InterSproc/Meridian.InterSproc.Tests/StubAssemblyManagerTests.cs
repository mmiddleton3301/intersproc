namespace Meridian.InterSproc.Tests
{
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class StubAssemblyManagerTests
    {
        [TestMethod]
        public void UnnamedTest1()
        {
            // Arrange
            IStubAssemblyManager stubAssemblyManager =
                this.GetStubAssemblyGeneratorInstance();

            // Act
            stubAssemblyManager.CleanupTemporaryAssemblies();

            // Assert
        }

        public IStubAssemblyManager GetStubAssemblyGeneratorInstance()
        {
            IStubAssemblyManager toReturn = null;

            ILoggingProvider loggingProvider = new LoggingProvider();

            IFileInfoWrapperFactory fileInfoWrapperFactory =
                new FileInfoWrapperFactory();
            Mock<IStubAssemblyGenerator> mockStubAssemblyGenerator =
                new Mock<IStubAssemblyGenerator>();

            toReturn = new StubAssemblyManager(
                fileInfoWrapperFactory,
                loggingProvider,
                mockStubAssemblyGenerator.Object);

            return toReturn;
        }
    }
}