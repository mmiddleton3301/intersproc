namespace Meridian.InterSproc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class StubAssemblyManagerTests
    {
        [TestMethod]
        public void CleanupTemporaryAssemblies_ThreeAssembliesWithSourcePresentForCleanup_EnsureAllAreDeleted()
        {
            // Arrange
            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            List<string> deletedFiles = new List<string>();

            Action<string> deleteMethodInvoked =
                x =>
                {
                    deletedFiles.Add(x);
                };

            string[] assembliesToDeleteStr =
            {
                $"Temporary_{Guid.NewGuid()}.isa",
                $"Temporary_{Guid.NewGuid()}.isa",
                $"Temporary_{Guid.NewGuid()}.isa"
            };

            IStubAssemblyManager stubAssemblyManager =
                this.GetStubAssemblyGeneratorInstance(
                    executingDirectory,
                    executingAssembly,
                    mockDirectoryInfoWrapper =>
                    {
                        mockDirectoryInfoWrapper
                            .Setup(x => x.FullName)
                            .Returns(executingDirectory);

                        IFileInfoWrapper[] assembliesToDelete =
                            assembliesToDeleteStr
                                .Select(x => $"{executingDirectory}\\{x}")
                                .Select(x => this.CreateMockInfoWrapperForAssembly(x, deleteMethodInvoked))
                                .ToArray();

                        mockDirectoryInfoWrapper
                            .Setup(x => x.GetFiles(It.IsAny<string>()))
                            .Returns(assembliesToDelete);
                    },
                    mockFileInfoWrapper =>
                    {
                        mockFileInfoWrapper
                            .Setup(x => x.Exists)
                            .Returns(true);

                        mockFileInfoWrapper
                            .Setup(x => x.Delete())
                            .Callback(() =>
                            {
                                deleteMethodInvoked(
                                    mockFileInfoWrapper.Object.FullName);
                            });
                    });

            string[] assembliesToDeleteFullPathStr =
                assembliesToDeleteStr
                .Select(x => $"{executingDirectory}\\{x}")
                .ToArray();

            string[] expectedDeletedFiles =
                assembliesToDeleteFullPathStr
                    .Concat(assembliesToDeleteFullPathStr.Select(y => $"{y}.cs"))
                    .OrderBy(x => x)
                    .ToArray();
            string[] actualDeletedFiles = null;

            // Act
            stubAssemblyManager.CleanupTemporaryAssemblies();

            // Assert
            actualDeletedFiles = deletedFiles
                .OrderBy(x => x)
                .ToArray();

            CollectionAssert.AreEqual(
                expectedDeletedFiles,
                actualDeletedFiles);
        }

        private IFileInfoWrapper CreateMockInfoWrapperForAssembly(
            string assemblyPath,
            Action<string> deleteInvoked)
        {
            IFileInfoWrapper toReturn = null;

            Mock<IFileInfoWrapper> mockFileInfoWrapper =
                new Mock<IFileInfoWrapper>();

            mockFileInfoWrapper
                .Setup(x => x.FullName)
                .Returns(assemblyPath);
            mockFileInfoWrapper
                .Setup(x => x.Delete())
                .Callback(() =>
                {
                    deleteInvoked(assemblyPath);
                });

            toReturn = mockFileInfoWrapper.Object;

            return toReturn;
        }

        public IStubAssemblyManager GetStubAssemblyGeneratorInstance(
            string executingDirectory,
            string executingAssembly,
            Action<Mock<IDirectoryInfoWrapper>> setupMockDirectoryInfoWrapper,
            Action<Mock<IFileInfoWrapper>> setupMockFileInfoWrapper)
        {
            IStubAssemblyManager toReturn = null;

            ILoggingProvider loggingProvider = new LoggingProvider();

            // Injected mocks
            Mock<IAssemblyWrapperFactory> mockAssemblyWrapperFactory =
                new Mock<IAssemblyWrapperFactory>();
            Mock<IDirectoryInfoWrapperFactory> mockDirectoryInfoWrapperFactory =
                new Mock<IDirectoryInfoWrapperFactory>();
            Mock<IFileInfoWrapperFactory> mockFileInfoWrapperFactory =
                new Mock<IFileInfoWrapperFactory>();
            Mock<IStubAssemblyGenerator> mockStubAssemblyGenerator =
                new Mock<IStubAssemblyGenerator>();

            this.SetupConstructorBehaviours(
                executingDirectory,
                executingAssembly,
                mockAssemblyWrapperFactory,
                mockDirectoryInfoWrapperFactory,
                mockFileInfoWrapperFactory,
                setupMockDirectoryInfoWrapper,
                setupMockFileInfoWrapper);

            // Inject and prepare instance
            toReturn = new StubAssemblyManager(
                mockAssemblyWrapperFactory.Object,
                mockDirectoryInfoWrapperFactory.Object,
                mockFileInfoWrapperFactory.Object,
                loggingProvider,
                mockStubAssemblyGenerator.Object);

            return toReturn;
        }

        private void SetupConstructorBehaviours(
            string executingDirectory,
            string executingAssembly,
            Mock<IAssemblyWrapperFactory> mockAssemblyWrapperFactory,
            Mock<IDirectoryInfoWrapperFactory> mockDirectoryInfoWrapperFactory,
            Mock<IFileInfoWrapperFactory> mockFileInfoWrapperFactory,
            Action<Mock<IDirectoryInfoWrapper>> setupMockDirectoryInfoWrapper,
            Action<Mock<IFileInfoWrapper>> setupMockFileInfoWrapper)
        {
            // mockAssemblyWrapperFactory
            Mock<IAssemblyWrapper> mockAssemblyWrapper =
                new Mock<IAssemblyWrapper>();

            mockAssemblyWrapper
                .Setup(x => x.Location)
                .Returns(executingAssembly);

            mockAssemblyWrapperFactory
                .Setup(x => x.GetExecutingAssembly())
                .Returns(mockAssemblyWrapper.Object);

            // mockDirectoryInfoWrapperFactory
            Mock<IDirectoryInfoWrapper> mockDirectoryInfoWrapper =
                new Mock<IDirectoryInfoWrapper>();

            setupMockDirectoryInfoWrapper(mockDirectoryInfoWrapper);

            mockDirectoryInfoWrapperFactory
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(mockDirectoryInfoWrapper.Object);

            // mockFileInfoWrapperFactory
            Func<string, IFileInfoWrapper> valueFunction =
                x =>
                {
                    Mock<IFileInfoWrapper> mockFileInfoWrapper =
                        new Mock<IFileInfoWrapper>();

                    mockFileInfoWrapper
                        .Setup(y => y.ParentDirectoryPath)
                        .Returns(executingDirectory);

                    mockFileInfoWrapper
                        .Setup(y => y.FullName)
                        .Returns(x);

                    setupMockFileInfoWrapper(mockFileInfoWrapper);

                    return mockFileInfoWrapper.Object;
                };

            mockFileInfoWrapperFactory
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(valueFunction);
        }
    }
}