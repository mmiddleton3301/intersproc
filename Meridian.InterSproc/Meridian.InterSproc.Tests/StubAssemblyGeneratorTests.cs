namespace Meridian.InterSproc.Tests
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class StubAssemblyGeneratorTests
    {
        [TestMethod]
        public void Create_AllDependenciesPresentCompilationSuccess_ReturnsExpectedGeneratedStubAssembly()
        {
            // Arrange
            const string Schema = "dbo";

            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            string expectedStubDestinationLocation =
                $@"{executingDirectory}\Temporary_{Guid.NewGuid()}.isa";
            string actualStubDestinationLocation = null;

            IStubAssemblyGenerator stubAssemblyGenerator =
                this.GetStubAssemblyGeneratorInstance(
                    mockAssemblyWrapperFactory =>
                    {
                        Mock<IAssemblyWrapper> createResult =
                            new Mock<IAssemblyWrapper>();

                        createResult
                            .Setup(x => x.Location)
                            .Returns(executingAssembly);

                        mockAssemblyWrapperFactory
                            .Setup(x => x.Create(It.IsAny<Assembly>()))
                            .Returns(createResult.Object);

                        Func<string, IAssemblyWrapper> returnsCallback =
                            x =>
                            {
                                Mock<IAssemblyWrapper> mockCreatedStubAssembly =
                                    new Mock<IAssemblyWrapper>();

                                mockCreatedStubAssembly
                                    .Setup(y => y.FullName)
                                    .Returns(Path.GetFileNameWithoutExtension(x));

                                mockCreatedStubAssembly
                                    .Setup(y => y.Location)
                                    .Returns(expectedStubDestinationLocation);

                                return mockCreatedStubAssembly.Object;
                            };

                        mockAssemblyWrapperFactory
                            .Setup(x => x.LoadFile(It.IsAny<string>()))
                            .Returns(returnsCallback);
                    },
                    mockCSharpCompilationWrapperFactory =>
                    {
                        Mock<IEmitResultWrapper> emitResult =
                            new Mock<IEmitResultWrapper>();

                        emitResult
                            .Setup(x => x.Success)
                            .Returns(true);

                        Mock<ICSharpCompilationWrapper> createResult =
                            new Mock<ICSharpCompilationWrapper>();

                        createResult
                            .Setup(x => x.Emit(It.IsAny<Stream>()))
                            .Returns(emitResult.Object);

                        mockCSharpCompilationWrapperFactory
                            .Setup(x => x.Create(
                                It.IsAny<IFileInfoWrapper>(),
                                It.IsAny<SyntaxTree>(),
                                It.IsAny<IEnumerable<IMetadataReferenceWrapper>>()))
                            .Returns(createResult.Object);
                    },
                    mockFileInfoWrapperFactory =>
                    {
                        Mock<IFileInfoWrapper> createResult =
                            new Mock<IFileInfoWrapper>();

                        createResult
                            .Setup(x => x.Create())
                            .Returns(new MemoryStream());

                        mockFileInfoWrapperFactory
                            .Setup(x => x.Create(It.IsAny<string>()))
                            .Returns(createResult.Object);
                    },
                    mockMetadataReferenceWrapperFactory =>
                    {
                        Func<string, IMetadataReferenceWrapper> createCallback =
                        path =>
                        {
                            Mock<IMetadataReferenceWrapper> mdrWrapper =
                                new Mock<IMetadataReferenceWrapper>();

                            mdrWrapper
                                .Setup(x => x.Display)
                                .Returns(path);

                            return mdrWrapper.Object;
                        };

                        mockMetadataReferenceWrapperFactory
                            .Setup(x => x.Create(It.IsAny<string>()))
                            .Returns(createCallback);
                    },
                    mockStubAssemblyGeneratorSettingsProvider =>
                    {
                        mockStubAssemblyGeneratorSettingsProvider
                            .Setup(x => x.GenerateAssemblyCodeFile)
                            .Returns(true);
                    },
                    mockStubDomGenerator =>
                    {
                        CodeNamespace generateEntireStubAssemblyDomResult =
                            new CodeNamespace();

                        mockStubDomGenerator
                            .Setup(x => x.GenerateEntireStubAssemblyDom(
                                It.IsAny<Type>(),
                                It.IsAny<IEnumerable<ContractMethodInformation>>()))
                            .Returns(generateEntireStubAssemblyDomResult);
                    });

            // destinationLocation argument
            Mock<IFileInfoWrapper> destinationLocation =
                new Mock<IFileInfoWrapper>();

            destinationLocation
                .Setup(x => x.FullName)
                .Returns(expectedStubDestinationLocation);

            MemoryStream memoryStream = new MemoryStream();

            destinationLocation
                .Setup(x => x.Create())
                .Returns(memoryStream);

            // contractMethodInformations argument
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
                },
            };

            // result
            IAssemblyWrapper result = null;

            // Act
            result = stubAssemblyGenerator.Create<IVanillaContract>(
                destinationLocation.Object,
                contractMethodInformations);

            // Assert
            actualStubDestinationLocation = result.Location;

            Assert.AreEqual(
                expectedStubDestinationLocation,
                actualStubDestinationLocation);
        }

        private IStubAssemblyGenerator GetStubAssemblyGeneratorInstance(
            Action<Mock<IAssemblyWrapperFactory>> setupMockAssemblyWrapperFactory,
            Action<Mock<ICSharpCompilationWrapperFactory>> setupMockCSharpCompilationWrapperFactory,
            Action<Mock<IFileInfoWrapperFactory>> setupMockFileInfoWrapperFactory,
            Action<Mock<IMetadataReferenceWrapperFactory>> setupMockMetadataReferenceWrapperFactory,
            Action<Mock<IStubAssemblyGeneratorSettingsProvider>> setupMockStubAssemblyGeneratorSettingsProvider,
            Action<Mock<IStubDomGenerator>> setupMockStubDomGenerator)
        {
            IStubAssemblyGenerator toReturn = null;

            Mock<IAssemblyWrapperFactory> mockAssemblyWrapperFactory =
                new Mock<IAssemblyWrapperFactory>();
            Mock<ICSharpCompilationWrapperFactory> mockCSharpCompilationWrapperFactory =
                new Mock<ICSharpCompilationWrapperFactory>();
            Mock<IFileInfoWrapperFactory> mockFileInfoWrapperFactory =
                new Mock<IFileInfoWrapperFactory>();
            LoggingProvider loggingProvider =
                new LoggingProvider();
            Mock<IMetadataReferenceWrapperFactory> mockMetadataReferenceWrapperFactory =
                new Mock<IMetadataReferenceWrapperFactory>();
            Mock<IStubAssemblyGeneratorSettingsProvider> mockStubAssemblyGeneratorSettingsProvider =
                new Mock<IStubAssemblyGeneratorSettingsProvider>();
            Mock<IStubDomGenerator> mockStubDomGenerator =
                new Mock<IStubDomGenerator>();

            // Setup mocks
            setupMockAssemblyWrapperFactory(mockAssemblyWrapperFactory);
            setupMockCSharpCompilationWrapperFactory(
                mockCSharpCompilationWrapperFactory);
            setupMockFileInfoWrapperFactory(mockFileInfoWrapperFactory);
            setupMockMetadataReferenceWrapperFactory(
                mockMetadataReferenceWrapperFactory);
            setupMockStubAssemblyGeneratorSettingsProvider(
                mockStubAssemblyGeneratorSettingsProvider);
            setupMockStubDomGenerator(mockStubDomGenerator);

            toReturn = new StubAssemblyGenerator(
                mockAssemblyWrapperFactory.Object,
                mockCSharpCompilationWrapperFactory.Object,
                mockFileInfoWrapperFactory.Object,
                loggingProvider,
                mockMetadataReferenceWrapperFactory.Object,
                mockStubAssemblyGeneratorSettingsProvider.Object,
                mockStubDomGenerator.Object);

            return toReturn;
        }
    }
}