namespace Meridian.InterSproc.Tests
{
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.CodeDom;

    [TestClass]
    public class StubMethodGeneratorTests
    {
        [TestMethod]
        public void CreateMethod_BuildImplementationForVoidMethod_EnsureMethodNameMatches()
        {
            // Arrange
            const string Schema = "dbo";

            IStubMethodGenerator stubMethodGenerator =
                this.GetStubMethodGeneratorInstance();

            ContractMethodInformation contractMethodInformation =
                new ContractMethodInformation()
                {
                    MethodInfo = typeof(IVanillaContract)
                        .GetMethod(nameof(IVanillaContract.SecondStoredProcedure)),
                    Name = nameof(IVanillaContract.SecondStoredProcedure),
                    Prefix = null,
                    Schema = Schema,
                };

            CodeMemberMethod result = null;

            string expectedMethodName =
                nameof(IVanillaContract.SecondStoredProcedure);
            string actualMethodName = null;

            // Act
            result = stubMethodGenerator.CreateMethod(
                contractMethodInformation);

            // Assert
            actualMethodName = result.Name;

            Assert.AreEqual(expectedMethodName, actualMethodName);
        }

        private IStubMethodGenerator GetStubMethodGeneratorInstance()
        {
            IStubMethodGenerator toReturn = null;

            ILoggingProvider loggingProvider = new LoggingProvider();
            CodeMemberField connectionStringMember =
                new CodeMemberField(
                    typeof(string),
                    "connectionString");

            toReturn = new StubMethodGenerator(
                loggingProvider,
                connectionStringMember);

            return toReturn;
        }
    }
}