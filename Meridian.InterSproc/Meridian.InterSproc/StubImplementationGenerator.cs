namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class StubImplementationGenerator : IStubImplementationGenerator
    {
        private const string StubImplementationClassName =
            "{0}StubImplementation";

        private readonly IStubCommonGenerator stubCommonGenerator;

        public StubImplementationGenerator(
            IStubCommonGenerator stubCommonGenerator)
        {
            this.stubCommonGenerator = stubCommonGenerator;
        }

        public CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            CodeTypeReference dataContextType,
            ContractMethodInformation[] contractMethodInformations,
            CodeMemberMethod[] dataContextMethods)
        {
            CodeTypeDeclaration toReturn = null;

            // Come up with the class name.
            string className = string.Format(
                StubImplementationClassName,
                databaseContractType.Name);

            // Declare class.
            toReturn = new CodeTypeDeclaration()
            {
                Name = className,
                IsClass = true,
                Attributes = MemberAttributes.Public
            };

            // Implements the database contract.
            toReturn.BaseTypes.Add(databaseContractType);

            // DataContext member declearation
            CodeMemberField dataContextMember = new CodeMemberField(
                dataContextType,
                "dataContext");

            toReturn.Members.Add(dataContextMember);

            // Constructor
            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };

            // Injectable settings provider, containing connection string.
            CodeParameterDeclarationExpression settingsProviderInject =
                new CodeParameterDeclarationExpression(
                    typeof(IStubImplementationSettingsProvider),
                    "stubImplementationSettingsProvider");

            constructor.Parameters.Add(settingsProviderInject);

            // Create a reference to the DataContext member...
            CodeThisReferenceExpression thisRef =
                new CodeThisReferenceExpression();
            CodeFieldReferenceExpression dataContextRef =
                new CodeFieldReferenceExpression(
                    thisRef,
                    dataContextMember.Name);

            // Make a reference to the "ConnStr" property of the injectable
            // settings provider.
            CodePropertyReferenceExpression settingsRefExpr =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(
                        settingsProviderInject.Name),
                    "ConnStr");

            // new DataContext()...
            CodeObjectCreateExpression createExpr =
                new CodeObjectCreateExpression(dataContextType, settingsRefExpr);

            // Into our class member.
            CodeAssignStatement assignStatement = new CodeAssignStatement(
                dataContextRef,
                createExpr);

            constructor.Statements.Add(assignStatement);

            toReturn.Members.Add(constructor);

            // Method implemnetations...
            CodeMemberMethod[] implementedMethods = contractMethodInformations
                .Select(x => this.ImplementContractMethod(x, dataContextMethods))
                .ToArray();

            toReturn.Members.AddRange(implementedMethods);

            return toReturn;
        }

        private CodeMemberMethod ImplementContractMethod(
            ContractMethodInformation contractMethodInformation,
            CodeMemberMethod[] dataContextMethods)
        {
            CodeMemberMethod toReturn = null;

            toReturn = new CodeMemberMethod()
            {
                Name = contractMethodInformation.MethodInfo.Name,
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };

            CodeParameterDeclarationExpression[] paramsToAdd =
                contractMethodInformation.MethodInfo
                    .GetParameters()
                    .Select(x => new CodeParameterDeclarationExpression(
                        x.ParameterType,
                        x.Name))
                    .ToArray();

            toReturn.Parameters.AddRange(paramsToAdd);

            Type returnType = contractMethodInformation.MethodInfo.ReturnType;

            CodeStatement[] body = this.stubCommonGenerator.GenerateMethodBody(
                returnType,
                x => this.BuildMethodBody(
                    x,
                    contractMethodInformation,
                    paramsToAdd,
                    dataContextMethods,
                    returnType));

            toReturn.Statements.AddRange(body);

            toReturn.ReturnType = new CodeTypeReference(returnType);

            return toReturn;
        }

        private void BuildMethodBody(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation,
            CodeParameterDeclarationExpression[] paramsToAdd,
            CodeMemberMethod[] dataContextMethods,
            Type returnType)
        {
            CodeFieldReferenceExpression dataContextRef =
                       new CodeFieldReferenceExpression(
                           new CodeThisReferenceExpression(),
                           "dataContext");

            CodeMethodReferenceExpression methodRef =
                new CodeMethodReferenceExpression(
                        dataContextRef,
                        contractMethodInformation.MethodInfo.Name);

            CodeVariableReferenceExpression[] paramsRefs =
                paramsToAdd
                    .Select(y => new CodeVariableReferenceExpression(y.Name))
                    .ToArray();

            // Invoke the data context method for this stub method.
            CodeMethodInvokeExpression dcMethodInvoke =
                new CodeMethodInvokeExpression(
                    methodRef,
                    paramsRefs);

            // Find the method that was generated previously.
            CodeMemberMethod dcMethod = dataContextMethods
                .Single(y => y.Name == contractMethodInformation.MethodInfo.Name);

            if (returnType != typeof(void))
            {
                CodeVariableDeclarationStatement iSingleResultCont =
                    new CodeVariableDeclarationStatement(
                        dcMethod.ReturnType,
                        $"single{dcMethod.Name}Result",
                        dcMethodInvoke);

                codeStatements.Add(iSingleResultCont);

                CodeSnippetStatement newLine =
                        new CodeSnippetStatement(string.Empty);

                CodeMethodReferenceExpression toArrayMethRef =
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression(iSingleResultCont.Name),
                        returnType.BaseType == typeof(Array) ? "ToArray" : "SingleOrDefault");

                CodeMethodInvokeExpression toArrayMethodInvoke =
                    new CodeMethodInvokeExpression(toArrayMethRef);

                CodeAssignStatement assignToReturn =
                    new CodeAssignStatement(
                        new CodeVariableReferenceExpression("toReturn"),
                        toArrayMethodInvoke);

                codeStatements.Add(newLine);
                codeStatements.Add(assignToReturn);
            }
            else
            {
                codeStatements.Add(new CodeExpressionStatement(dcMethodInvoke));
            }
        }
    }
}
