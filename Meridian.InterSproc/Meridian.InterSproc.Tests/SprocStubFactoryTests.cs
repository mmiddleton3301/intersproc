namespace Meridian.InterSproc.Tests
{
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class SprocStubFactoryTests
    {
        [TestMethod]
        public void CreateStub_UseCachedAssembliesFetchesCachedAssembly_ProducesInterSprocStubInstance()
        {
            // Arrange
            const string Schema = "dbo";

            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            ISprocStubFactory sprocStubFactory =
                this.GetSprocStubFactoryInstance(
                    mockSprocStubFactorySettingsProvider =>
                    {
                        mockSprocStubFactorySettingsProvider
                            .Setup(x => x.UseCachedStubAssemblies)
                            .Returns(true);
                    },
                    mockContractMethodInformationConverter =>
                    {
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

                        mockContractMethodInformationConverter
                            .Setup(x => x.GetContractMethodInformationFromContract<IVanillaContract>())
                            .Returns(contractMethodInformations);
                    },
                    mockDatabaseContractHashProvider =>
                    {
                        mockDatabaseContractHashProvider
                            .Setup(x => x.GetContractHash(It.IsAny<IEnumerable<ContractMethodInformation>>()))
                            .Returns($"{Guid.NewGuid()}");
                    },
                    mockStubAssemblyManager => {
                        Mock<IAssemblyWrapper> mockAssemblyWrapper =
                            new Mock<IAssemblyWrapper>();

                        mockAssemblyWrapper
                            .Setup(x => x.Location)
                            .Returns(executingAssembly);

                        mockStubAssemblyManager
                            .Setup(x => x.GetValidStubAssembly(
                                It.IsAny<string>()))
                            .Returns(mockAssemblyWrapper.Object);
                    },
                    mockInstanceProvider =>
                    {
                        Mock<IVanillaContract> mockVanillaContractStub =
                            new Mock<IVanillaContract>();

                        mockInstanceProvider
                            .Setup(x => x.GetInstance<IVanillaContract>(
                                It.IsAny<IAssemblyWrapper>(),
                                It.IsAny<string>()))
                            .Returns(mockVanillaContractStub.Object);
                    });

            string connStr =
                "Server=SCCENDB47.somecorp.local;" +
                "Initial Catalog=HRDB;" +
                "Integrated Security=True;";

            IVanillaContract result = null;

            // Act
            result = sprocStubFactory.CreateStub<IVanillaContract>(connStr);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateStub_DontUseCachedAssembliesGenerateNewAssembly_ProducesInterSprocStubInstance()
        {
            // Arrange
            const string Schema = "dbo";

            string executingDirectory = @"X:\somecorp-hrapp-web";
            string executingAssembly =
                $@"{executingDirectory}\SomeCorp.HrApp.Web.dll";

            ISprocStubFactory sprocStubFactory =
                this.GetSprocStubFactoryInstance(
                    mockSprocStubFactorySettingsProvider =>
                    {
                        // Does nothing.
                    },
                    mockContractMethodInformationConverter =>
                    {
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

                        mockContractMethodInformationConverter
                            .Setup(x => x.GetContractMethodInformationFromContract<IVanillaContract>())
                            .Returns(contractMethodInformations);
                    },
                    mockDatabaseContractHashProvider =>
                    {
                        mockDatabaseContractHashProvider
                            .Setup(x => x.GetContractHash(It.IsAny<IEnumerable<ContractMethodInformation>>()))
                            .Returns($"{Guid.NewGuid()}");
                    },
                    mockStubAssemblyManager => {
                        Mock<IAssemblyWrapper> mockAssemblyWrapper =
                            new Mock<IAssemblyWrapper>();

                        mockAssemblyWrapper
                            .Setup(x => x.Location)
                            .Returns(executingAssembly);

                        mockStubAssemblyManager
                            .Setup(x => x.GenerateStubAssembly<IVanillaContract>(
                                It.IsAny<string>(),
                                It.IsAny<IEnumerable<ContractMethodInformation>>()))
                            .Returns(mockAssemblyWrapper.Object);
                    },
                    mockInstanceProvider =>
                    {
                        Mock<IVanillaContract> mockVanillaContractStub =
                            new Mock<IVanillaContract>();

                        mockInstanceProvider
                            .Setup(x => x.GetInstance<IVanillaContract>(
                                It.IsAny<IAssemblyWrapper>(),
                                It.IsAny<string>()))
                            .Returns(mockVanillaContractStub.Object);
                    });

            string connStr =
                "Server=SCCENDB47.somecorp.local;" +
                "Initial Catalog=HRDB;" +
                "Integrated Security=True;";

            IVanillaContract result = null;

            // Act
            result = sprocStubFactory.CreateStub<IVanillaContract>(connStr);

            // Assert
            Assert.IsNotNull(result);
        }

        private ISprocStubFactory GetSprocStubFactoryInstance(
            Action<Mock<ISprocStubFactorySettingsProvider>> setupMockSprocStubFactorySettingsProvider,
            Action<Mock<IContractMethodInformationConverter>> setupMockContractMethodInformationConverter,
            Action<Mock<IDatabaseContractHashProvider>> setupMockDatabaseContractHashProvider,
            Action<Mock<IStubAssemblyManager>> setupMockStubAssemblyManager,
            Action<Mock<IStubInstanceProvider>> setupMockInstanceProvider)
        {
            ISprocStubFactory toReturn = null;

            Mock<IContractMethodInformationConverter> mockContractMethodInformationConverter =
                new Mock<IContractMethodInformationConverter>();
            Mock<IDatabaseContractHashProvider> mockDatabaseContractHashProvider =
                new Mock<IDatabaseContractHashProvider>();
            ILoggingProvider loggingProvider = new LoggingProvider();
            Mock<ISprocStubFactorySettingsProvider> mockSprocStubFactorySettingsProvider =
                new Mock<ISprocStubFactorySettingsProvider>();
            Mock<IStubAssemblyManager> mockStubAssemblyManager =
                new Mock<IStubAssemblyManager>();
            Mock<IStubInstanceProvider> mockStubInstanceProvider =
                new Mock<IStubInstanceProvider>();

            // Setup mocks
            setupMockSprocStubFactorySettingsProvider(
                mockSprocStubFactorySettingsProvider);
            setupMockContractMethodInformationConverter(
                mockContractMethodInformationConverter);
            setupMockDatabaseContractHashProvider(
                mockDatabaseContractHashProvider);
            setupMockStubAssemblyManager(mockStubAssemblyManager);
            setupMockInstanceProvider(mockStubInstanceProvider);

            toReturn = new SprocStubFactory(
                mockContractMethodInformationConverter.Object,
                mockDatabaseContractHashProvider.Object,
                loggingProvider,
                mockSprocStubFactorySettingsProvider.Object,
                mockStubAssemblyManager.Object,
                mockStubInstanceProvider.Object);

            return toReturn;
        }
    }
}