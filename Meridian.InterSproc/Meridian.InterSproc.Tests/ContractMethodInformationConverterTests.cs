namespace Meridian.InterSproc.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContractMethodInformationConverterTests
    {
        [TestMethod]
        public void GetContractMethodInformationFromContract_UseClassWideOverrideAndMethodOverridesAttributes_EnsureContractInformationCorrect()
        {
            // Arrange
            const string Schema = "sec";
            const string Prefix = "mysp_";

            IContractMethodInformationConverter contractMethodInformationConverter =
                this.GetContractMethodInformationConverterInstance();

            IEnumerable<ContractMethodInformation> expectedContractMethodInformations =
                new ContractMethodInformation[]
                {
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IInterfaceWideOverrideAndMethodOverrideContract)
                            .GetMethod(nameof(IInterfaceWideOverrideAndMethodOverrideContract.FirstStoredProcedure)),
                        Name = nameof(IInterfaceWideOverrideAndMethodOverrideContract.FirstStoredProcedure),
                        Schema = Schema,
                        Prefix = Prefix,
                    },
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IInterfaceWideOverrideAndMethodOverrideContract)
                            .GetMethod(nameof(IInterfaceWideOverrideAndMethodOverrideContract.SecondStoredProcedure)),
                        Name = nameof(IInterfaceWideOverrideAndMethodOverrideContract.SecondStoredProcedure),
                        Schema = Schema,
                        Prefix = Prefix,
                    },
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IInterfaceWideOverrideAndMethodOverrideContract)
                            .GetMethod(nameof(IInterfaceWideOverrideAndMethodOverrideContract.OutsideOfSchemaStoredProcedure)),
                        Name = nameof(IInterfaceWideOverrideAndMethodOverrideContract.OutsideOfSchemaStoredProcedure),
                        Schema = "dbo",
                        Prefix = string.Empty,
                    },
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IInterfaceWideOverrideAndMethodOverrideContract)
                            .GetMethod(nameof(IInterfaceWideOverrideAndMethodOverrideContract.NameOverriddenStoredProcedure)),
                        Name = "ThirdStoredProcedure",
                        Schema = Schema,
                        Prefix = Prefix,
                    }
                };
            IEnumerable<ContractMethodInformation> actualContractMethodInformations =
                null;

            // Act
            actualContractMethodInformations =
                contractMethodInformationConverter.GetContractMethodInformationFromContract<IInterfaceWideOverrideAndMethodOverrideContract>();

            // Assert
            this.CompareTwoContractMethodInformationCollections(
                expectedContractMethodInformations,
                actualContractMethodInformations);
        }

        [TestMethod]
        public void GetContractMethodInformationFromContract_UseClassWideOverrideAttributeOnly_EnsureContractInformationCorrect()
        {
            // Arrange
            const string Schema = "hr";
            const string Prefix = "usp_";

            IContractMethodInformationConverter contractMethodInformationConverter =
                this.GetContractMethodInformationConverterInstance();

            IEnumerable<ContractMethodInformation> expectedContractMethodInformations =
                new ContractMethodInformation[]
                {
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IInterfaceWideOverrideOnlyContract)
                            .GetMethod(nameof(IInterfaceWideOverrideOnlyContract.FirstStoredProcedure)),
                        Name = nameof(IInterfaceWideOverrideOnlyContract.FirstStoredProcedure),
                        Schema = Schema,
                        Prefix = Prefix,
                    },
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IInterfaceWideOverrideOnlyContract)
                            .GetMethod(nameof(IInterfaceWideOverrideOnlyContract.SecondStoredProcedure)),
                        Name = nameof(IInterfaceWideOverrideOnlyContract.SecondStoredProcedure),
                        Schema = Schema,
                        Prefix = Prefix,
                    }
                };
            IEnumerable<ContractMethodInformation> actualContractMethodInformations =
                null;

            // Act
            actualContractMethodInformations =
                contractMethodInformationConverter.GetContractMethodInformationFromContract<IInterfaceWideOverrideOnlyContract>();

            // Assert
            this.CompareTwoContractMethodInformationCollections(
                expectedContractMethodInformations,
                actualContractMethodInformations);
        }

        [TestMethod]
        public void GetContractMethodInformationFromContract_UseNoOverridingAttributes_EnsureContractMethodInformationCorrect()
        {
            // Arrange
            const string Schema = "dbo";
            const string Prefix = null;

            IContractMethodInformationConverter contractMethodInformationConverter =
                this.GetContractMethodInformationConverterInstance();

            IEnumerable<ContractMethodInformation> expectedContractMethodInformations =
                new ContractMethodInformation[]
                {
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IVanillaContract)
                            .GetMethod(nameof(IVanillaContract.FirstStoredProcedure)),
                        Name = nameof(IVanillaContract.FirstStoredProcedure),
                        Schema = Schema,
                        Prefix = Prefix,
                    },
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IVanillaContract)
                            .GetMethod(nameof(IVanillaContract.SecondStoredProcedure)),
                        Name = nameof(IVanillaContract.SecondStoredProcedure),
                        Schema = Schema,
                        Prefix = Prefix,
                    }
                };
            IEnumerable<ContractMethodInformation> actualContractMethodInformations =
                null;

            // Act
            actualContractMethodInformations =
                contractMethodInformationConverter.GetContractMethodInformationFromContract<IVanillaContract>();

            // Assert
            this.CompareTwoContractMethodInformationCollections(
                expectedContractMethodInformations,
                actualContractMethodInformations);
        }

        private void CompareTwoContractMethodInformationCollections(
            IEnumerable<ContractMethodInformation> expectedContractMethodInformations,
            IEnumerable<ContractMethodInformation> actualContractMethodInformations)
        {
            // Convert the two arrays to string equivilants, then compare.
            string[] expectedContractMethodInformationsToString = expectedContractMethodInformations
                .Select(x => x.ToString())
                .ToArray();
            string[] actualContractMethodInformationsToString = actualContractMethodInformations
                .Select(x => x.ToString())
                .ToArray();

            CollectionAssert.AreEqual(
                expectedContractMethodInformationsToString,
                actualContractMethodInformationsToString);
        }

        private IContractMethodInformationConverter GetContractMethodInformationConverterInstance()
        {
            IContractMethodInformationConverter toReturn = null;

            ILoggingProvider loggingProvider = new LoggingProvider();

            toReturn = new ContractMethodInformationConverter(loggingProvider);

            return toReturn;
        }
    }
}