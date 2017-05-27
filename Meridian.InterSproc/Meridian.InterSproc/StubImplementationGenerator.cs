namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class StubImplementationGenerator : IStubImplementationGenerator
    {
        private const string StubImplementationClassName =
            "{0}StubImplementation";

        private readonly IStubCommonGenerator stubCommonGenerator;

        private CodeMemberMethod[] dataContextMethods;

        public StubImplementationGenerator(
            IStubCommonGenerator stubCommonGenerator)
        {
            this.stubCommonGenerator = stubCommonGenerator;
        }

        public CodeTypeDeclaration CreateClass(
            Type type,
            CodeTypeReference dataContextType,
            ContractMethodInformation[] contractMethodInformations,
            CodeMemberMethod[] dataContextMethods)
        {
            CodeTypeDeclaration toReturn = null;

            this.dataContextMethods = dataContextMethods;

            toReturn = new CodeTypeDeclaration()
            {
                Name = string.Format(StubImplementationClassName, type.Name),
                IsClass = true,
                Attributes = MemberAttributes.Public
            };

            toReturn.BaseTypes.Add(type);

            CodeMemberField dataContextMember = new CodeMemberField(
                dataContextType,
                "dataContext");

            toReturn.Members.Add(dataContextMember);

            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };

            CodeParameterDeclarationExpression settingsProviderInject =
                new CodeParameterDeclarationExpression(
                    typeof(IStubImplementationSettingsProvider),
                    "stubImplementationSettingsProvider");
            constructor.Parameters.Add(settingsProviderInject);

            CodeThisReferenceExpression thisRef =
                new CodeThisReferenceExpression();
            CodeFieldReferenceExpression dataContextRef =
                new CodeFieldReferenceExpression(
                    thisRef,
                    dataContextMember.Name);

            CodePropertyReferenceExpression settingsRefExpr =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(settingsProviderInject.Name),
                    "ConnStr");

            CodeObjectCreateExpression createExpr =
                new CodeObjectCreateExpression(dataContextType, settingsRefExpr);

            CodeAssignStatement assignStatement = new CodeAssignStatement(
                dataContextRef,
                createExpr);

            constructor.Statements.Add(assignStatement);

            toReturn.Members.Add(constructor);

            CodeMemberMethod[] implementedMethods = contractMethodInformations
                .Select(this.ImplementContractMethod)
                .ToArray();

            toReturn.Members.AddRange(implementedMethods);

            return toReturn;
        }

        private CodeParameterDeclarationExpression[] CreateImplementationParameters(
            MethodInfo methodInfo)
        {
            CodeParameterDeclarationExpression[] toReturn =
                methodInfo
                    .GetParameters()
                    .Select(x => new CodeParameterDeclarationExpression(
                        x.ParameterType,
                        x.Name))
                    .ToArray();

            return toReturn;
        }

        private CodeMemberMethod ImplementContractMethod(
            ContractMethodInformation contractMethodInformation)
        {
            CodeMemberMethod toReturn = null;

            toReturn = new CodeMemberMethod()
            {
                Name = contractMethodInformation.MethodInfo.Name,
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };

            CodeParameterDeclarationExpression[] paramsToAdd =
                this.CreateImplementationParameters(
                    contractMethodInformation.MethodInfo);

            toReturn.Parameters.AddRange(paramsToAdd);

            Type returnType = contractMethodInformation.MethodInfo.ReturnType;

            CodeStatement[] body = this.stubCommonGenerator.GenerateMethodBody(
                returnType,
                x =>
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
                    CodeMemberMethod dcMethod = this.dataContextMethods
                        .Single(y => y.Name == contractMethodInformation.MethodInfo.Name);

                    if (returnType != typeof(void))
                    {
                        CodeVariableDeclarationStatement iSingleResultCont =
                            new CodeVariableDeclarationStatement(
                                dcMethod.ReturnType,
                                $"single{dcMethod.Name}Result",
                                dcMethodInvoke);

                        x.Add(iSingleResultCont);

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

                        x.Add(newLine);
                        x.Add(assignToReturn);
                    }
                    else
                    {
                        x.Add(new CodeExpressionStatement(dcMethodInvoke));
                    }
                });

            toReturn.Statements.AddRange(body);

            toReturn.ReturnType = new CodeTypeReference(returnType);

            return toReturn;
        }
    }
}
