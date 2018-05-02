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
        private const string DefaultSchema = "dbo";

        [TestMethod]
        public void GetContractMethodInformationFromContract_UsNoOverridingAttributes_EnsureContractMethodInformationContainsCorrectDetail()
        {
            // Arrange
            Mock<ILoggingProvider> loggingProvider =
                new Mock<ILoggingProvider>();

            IContractMethodInformationConverter contractMethodInformationConverter =
                new ContractMethodInformationConverter(
                    loggingProvider.Object);

            IEnumerable<ContractMethodInformation> expectedContractMethodInformations =
                new ContractMethodInformation[]
                {
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IVanillaContract).GetMethod(nameof(IVanillaContract.FirstStoredProcedure)),
                        Name = nameof(IVanillaContract.FirstStoredProcedure),
                        Schema = DefaultSchema,
                        Prefix = null,
                    },
                    new ContractMethodInformation()
                    {
                        MethodInfo = typeof(IVanillaContract).GetMethod(nameof(IVanillaContract.SecondStoredProcedure)),
                        Name = nameof(IVanillaContract.SecondStoredProcedure),
                        Schema = DefaultSchema,
                        Prefix = null,
                    }
                };
            IEnumerable<ContractMethodInformation> actualContractMethodInformations =
                null;

            // Act
            actualContractMethodInformations =
                contractMethodInformationConverter.GetContractMethodInformationFromContract<IVanillaContract>();

            // Assert
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
    }
}