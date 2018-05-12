namespace Meridian.InterSproc.Tests
{
    using System;
    using System.CodeDom;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;
    using Meridian.InterSproc.Tests.Infrastructure;
    using Meridian.InterSproc.Tests.Infrastructure.ExampleContracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class StubClassGeneratorTests
    {
        [TestMethod]
        public void CreateClass_ClassIsGeneratedMinusMethods_CreatedClassHasCorrectName()
        {
            // Arrange
            const string Schema = "dbo";

            IStubClassGenerator stubClassGenerator =
                this.GetStubClassGeneratorInstance(
                    mockStubMethodGeneratorFactory =>
                    {
                        Mock<IStubMethodGenerator> mockStubMethodGenerator =
                            new Mock<IStubMethodGenerator>();

                        // Just return an empty/blank CodeMemberMethod.
                        mockStubMethodGenerator
                            .Setup(x => x.CreateMethod(
                                It.IsAny<ContractMethodInformation>()))
                            .Returns(new CodeMemberMethod());

                        mockStubMethodGeneratorFactory
                            .Setup(x => x.Create(It.IsAny<CodeMemberField>()))
                            .Returns(mockStubMethodGenerator.Object);
                    });

            Type databaseContractType = typeof(IVanillaContract);
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

            CodeTypeDeclaration result = null;

            string expectedClassName =
                $"{nameof(IVanillaContract)}StubImplementation";
            string actualClassName = null;

            // Act
            result = stubClassGenerator.CreateClass(
                databaseContractType,
                contractMethodInformations);

            // Assert
            actualClassName = result.Name;

            Assert.AreEqual(expectedClassName, actualClassName);
        }

        private IStubClassGenerator GetStubClassGeneratorInstance(
            Action<Mock<IStubMethodGeneratorFactory>> setupMockStubMethodGeneratorFactory)
        {
            IStubClassGenerator toReturn = null;

            ILoggingProvider loggingProvider = new LoggingProvider();

            Mock<IStubMethodGeneratorFactory> mockStubMethodGeneratorFactory =
                new Mock<IStubMethodGeneratorFactory>();

            // Setup mocks
            setupMockStubMethodGeneratorFactory(
                mockStubMethodGeneratorFactory);

            toReturn = new StubClassGenerator(
                loggingProvider,
                mockStubMethodGeneratorFactory.Object);

            return toReturn;
        }
    }
}