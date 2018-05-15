namespace Meridian.InterSproc.Tests
{
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class DatabaseContractHashProviderTests
    {
        [TestMethod]
        public void GetContractHash_GetHashOfExampleContractMethodInformations_CheckHashIsCorrect()
        {
            // Arrange
            const string Schema = "sec";
            const string Prefix = "mysp_";

            IDatabaseContractHashProvider databaseContractHashProvider =
                this.GetDatabaseContractHashProviderInstance();
            IEnumerable<ContractMethodInformation> contractMethodInformations =
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

            const string expectedContractHash = "WMSHoaNZ0LxumDq07p9mQicpFJ8Llh5m28O2aTiXIgw=";
            string actualContractHash = null;

            // Act
            actualContractHash = databaseContractHashProvider
                .GetContractHash(contractMethodInformations);

            // Assert
            Assert.AreEqual(expectedContractHash, actualContractHash);
        }

        private IDatabaseContractHashProvider GetDatabaseContractHashProviderInstance()
        {
            IDatabaseContractHashProvider toReturn = null;

            ILoggingProvider loggingProvider = new LoggingProvider();

            toReturn = new DatabaseContractHashProvider(loggingProvider);

            return toReturn;
        }
    }
}