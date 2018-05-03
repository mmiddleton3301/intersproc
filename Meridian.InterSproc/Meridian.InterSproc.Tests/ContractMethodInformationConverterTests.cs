namespace Meridian.InterSproc.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ContractMethodInformationConverterTests
    {
        private IContractMethodInformationConverter GetContractMethodInformationConverterInstance()
        {
            IContractMethodInformationConverter toReturn = null;

            Mock<ILoggingProvider> mockLoggingProvider =
                new Mock<ILoggingProvider>();

            toReturn = new ContractMethodInformationConverter(
                mockLoggingProvider.Object);

            return toReturn;
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
    }
}