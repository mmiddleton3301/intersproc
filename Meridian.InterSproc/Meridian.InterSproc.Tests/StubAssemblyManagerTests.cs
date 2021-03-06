﻿namespace Meridian.InterSproc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
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
                this.GetStubAssemblyManagerInstance(
                    executingDirectory,
                    executingAssembly,
                    setupMockAssemblyWrapperFactory =>
                    {
                        // Nothing for now.
                    },
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
                    },
                    mockStubAssemblyGenerator =>
                    {
                        // Nothing for now.
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

        [TestMethod]
        public void GenerateStubAssembly_AssemblyGeneratedWithSuccess_NameOfGeneratedAssemblyMatchesInputContractHash()
        {
            // Arrange
            const string Schema = "dbo";

            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            string expectedFilename = executingAssembly;
            string actualFilename = null;

            IStubAssemblyManager stubAssemblyManager =
                this.GetStubAssemblyManagerInstance(
                    executingDirectory,
                    executingAssembly,
                    mockAssemblyWrapperFactory =>
                    {
                        // Nothing for now.
                    },
                    mockDirectoryInfoWrapper =>
                    {
                        mockDirectoryInfoWrapper
                            .Setup(x => x.FullName)
                            .Returns(executingDirectory);
                    },
                    mockFileInfoWrapper =>
                    {
                        // Nothing for now.
                    },
                    mockStubAssemblyGenerator =>
                    {
                        Mock<IAssemblyWrapper> mockAssemblyWrapper =
                            new Mock<IAssemblyWrapper>();

                        mockAssemblyWrapper
                            .Setup(x => x.FullName)
                            .Returns(expectedFilename);

                        mockStubAssemblyGenerator
                            .Setup(x => x.Create<IVanillaContract>(
                                It.IsAny<IFileInfoWrapper>(),
                                It.IsAny<IEnumerable<ContractMethodInformation>>()))
                            .Returns(mockAssemblyWrapper.Object);

                    });

            string contractHashStr = $"{Guid.NewGuid()}";

            IAssemblyWrapper result = null;
            ContractMethodInformation[] contractMethodInformations =
            {
                new ContractMethodInformation()
                {
                    MethodInfo = typeof(IVanillaContract)
                        .GetMethod(nameof(IVanillaContract.FirstStoredProcedure)),
                    Name = nameof(IVanillaContract.FirstStoredProcedure),
                    Prefix = null,
                    Schema = Schema,
                },
                new ContractMethodInformation()
                {
                    MethodInfo = typeof(IVanillaContract)
                        .GetMethod(nameof(IVanillaContract.SecondStoredProcedure)),
                    Name = nameof(IVanillaContract.SecondStoredProcedure),
                    Prefix = null,
                    Schema = Schema,
                }
            };

            // Act
            result =
                stubAssemblyManager.GenerateStubAssembly<IVanillaContract>(
                    contractHashStr,
                    contractMethodInformations);

            // Assert
            actualFilename = result.FullName;

            Assert.AreEqual(expectedFilename, actualFilename);
        }

        [TestMethod]
        public void GetValidStubAssembly_SearchForAssemblyThatDoesExist_ReturnsFileMatchingInputHash()
        {
            // Arrange
            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            string contractHashStr = $"{Guid.NewGuid()}";

            string expectedAssemblyFilename =
                $@"{executingDirectory}\Temporary_{contractHashStr}.isa";
            string actualAssemblyFilename = null;

            IStubAssemblyManager stubAssemblyManager =
                this.GetStubAssemblyManagerInstance(
                    executingDirectory,
                    executingAssembly,
                    mockAssemblyWrapperFactory =>
                    {
                        Mock<IAssemblyWrapper> mockAssemblyWrapper =
                            new Mock<IAssemblyWrapper>();

                        mockAssemblyWrapper
                            .Setup(x => x.FullName)
                            .Returns(expectedAssemblyFilename);

                        mockAssemblyWrapperFactory
                            .Setup(x => x.LoadFile(It.IsAny<string>()))
                            .Returns(mockAssemblyWrapper.Object);
                    },
                    mockDirectoryInfoWrapper =>
                    {
                        mockDirectoryInfoWrapper
                            .Setup(x => x.FullName)
                            .Returns(executingDirectory);
                    },
                    mockFileInfoWrapper =>
                    {
                        mockFileInfoWrapper
                            .Setup(x => x.Exists)
                            .Returns(true);
                    },
                    mockStubAssemblyGenerator =>
                    {
                        // Nothing for now.
                    });

            IAssemblyWrapper assemblyWrapper = null;

            // Act
            assemblyWrapper = stubAssemblyManager
                .GetValidStubAssembly(contractHashStr);

            // Assert
            actualAssemblyFilename = assemblyWrapper.FullName;

            Assert.AreEqual(expectedAssemblyFilename, actualAssemblyFilename);
        }

        [TestMethod]
        public void GetValidStubAssembly_SearchForAssemblyThatDoesNotExist_ReturnsNull()
        {
            // Arrange
            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            IStubAssemblyManager stubAssemblyManager =
                this.GetStubAssemblyManagerInstance(
                    executingDirectory,
                    executingAssembly,
                    mockAssemblyWrapperFactory =>
                    {
                        // Nothing for now.
                    },
                    mockDirectoryInfoWrapper =>
                    {
                        mockDirectoryInfoWrapper
                            .Setup(x => x.FullName)
                            .Returns(executingDirectory);
                    },
                    mockFileInfoWrapper =>
                    {
                        // Nothing for now.
                    },
                    mockStubAssemblyGenerator =>
                    {
                        // Nothing for now.
                    });

            string contractHashStr = $"{Guid.NewGuid()}";
            IAssemblyWrapper assemblyWrapper = null;

            // Act
            assemblyWrapper = stubAssemblyManager
                .GetValidStubAssembly(contractHashStr);

            // Assert
            Assert.IsNull(assemblyWrapper);
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

        private IStubAssemblyManager GetStubAssemblyManagerInstance(
            string executingDirectory,
            string executingAssembly,
            Action<Mock<IAssemblyWrapperFactory>> setupMockAssemblyWrapperFactory,
            Action<Mock<IDirectoryInfoWrapper>> setupMockDirectoryInfoWrapper,
            Action<Mock<IFileInfoWrapper>> setupMockFileInfoWrapper,
            Action<Mock<IStubAssemblyGenerator>> setupMockStubAssemblyGenerator)
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
                mockStubAssemblyGenerator,
                setupMockAssemblyWrapperFactory,
                setupMockDirectoryInfoWrapper,
                setupMockFileInfoWrapper,
                setupMockStubAssemblyGenerator);

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
            Mock<IStubAssemblyGenerator> mockStubAssemblyGenerator,
            Action<Mock<IAssemblyWrapperFactory>> setupMockAssemblyWrapperFactory,
            Action<Mock<IDirectoryInfoWrapper>> setupMockDirectoryInfoWrapper,
            Action<Mock<IFileInfoWrapper>> setupMockFileInfoWrapper,
            Action<Mock<IStubAssemblyGenerator>> setupMockStubAssemblyGenerator)
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

            setupMockAssemblyWrapperFactory(mockAssemblyWrapperFactory);

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

            // mockStubAssemblyGenerator
            setupMockStubAssemblyGenerator(mockStubAssemblyGenerator);
        }
    }
}