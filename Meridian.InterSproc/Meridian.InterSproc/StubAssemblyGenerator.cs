namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Microsoft.CSharp;
    using System.Data.Linq.Mapping;

    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        private const string BaseStubNamespace =
            "Meridian.InterSproc.TemporaryStub";

        private const string StubImplementationClassName =
            "{0}StubImplementation";

        private const string StubImplementationDataContextName =
            "{0}DataContext";

        private readonly CSharpCodeProvider csharpCodeProvider;
        private readonly IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider;

        private CodeMemberMethod[] dataContextMethods;

        public StubAssemblyGenerator(
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider)
        {
            this.csharpCodeProvider = new CSharpCodeProvider();
            this.stubAssemblyGeneratorSettingsProvider =
                stubAssemblyGeneratorSettingsProvider;
        }

        public Assembly Create<DatabaseContractType>(
            FileInfo destinationLocation,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(BaseStubNamespace);
            codeNamespace.Imports.Add(
                new CodeNamespaceImport(typeof(Enumerable).Namespace));

            codeCompileUnit.Namespaces.Add(codeNamespace);

            Type type = typeof(DatabaseContractType);

            CodeTypeDeclaration customDataContext =
                this.CreateCustomDataContext(
                    type,
                    contractMethodInformations);

            codeNamespace.Types.Add(customDataContext);

            CodeTypeDeclaration interfaceImplementation =
                this.CreateInterfaceImplementation(
                    type,
                    new CodeTypeReference(customDataContext.Name),
                    contractMethodInformations);

            codeNamespace.Types.Add(interfaceImplementation);

            if (this.stubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)
            {
                this.GenerateCodeFile(destinationLocation, codeCompileUnit);
            }

            Assembly hostAssembly = typeof(DatabaseContractType).Assembly;
            FileInfo hostAssemblyLocation =
                new FileInfo(hostAssembly.Location);

            toReturn = this.CompileStubAssembly(
                destinationLocation,
                hostAssemblyLocation,
                codeCompileUnit);

            return toReturn;
        }

        private CodeTypeDeclaration CreateCustomDataContext(
            Type type,
            ContractMethodInformation[] contractMethodInformations)
        {
            CodeTypeDeclaration toReturn = new CodeTypeDeclaration()
            {
                Name = string.Format(StubImplementationDataContextName, type.Name),
                IsClass = true,
                TypeAttributes = TypeAttributes.NotPublic
            };

            Type dataContextType = typeof(DataContext);

            toReturn.BaseTypes.Add(dataContextType);

            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.FamilyAndAssembly
            };
            constructor.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(string), "connStr"));
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("connStr"));

            toReturn.Members.Add(constructor);

            this.dataContextMethods = contractMethodInformations
                .Select(this.CreateDataContextMethod)
                .ToArray();

            toReturn.Members.AddRange(this.dataContextMethods);

            return toReturn;
        }

        private CodeMemberMethod CreateDataContextMethod(
            ContractMethodInformation contractMethodInformation)
        {
            CodeMemberMethod toReturn = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.FamilyAndAssembly
            };

            CodeTypeReference functionAttrType =
                new CodeTypeReference(typeof(FunctionAttribute));
            CodeAttributeArgument isComposibleArg =
                new CodeAttributeArgument(
                    "IsComposable",
                    new CodePrimitiveExpression(false));
            CodeAttributeArgument nameArgument =
                new CodeAttributeArgument(
                    "Name",
                    new CodePrimitiveExpression(
                        $"{contractMethodInformation.Schema}." +
                        $"{contractMethodInformation.Prefix}{contractMethodInformation.Name}"));

            CodeAttributeDeclaration methodAttr = new CodeAttributeDeclaration(
                functionAttrType,
                isComposibleArg,
                nameArgument);

            toReturn.CustomAttributes.Add(methodAttr);

            toReturn.Name = contractMethodInformation.MethodInfo.Name;

            ParameterInfo[] methodParamInfos = contractMethodInformation
                .MethodInfo
                .GetParameters();

            CodeParameterDeclarationExpression[] methodParams =
                methodParamInfos
                    .Select(x => new CodeParameterDeclarationExpression(
                        x.ParameterType,
                        x.Name))
                    .ToArray();

            toReturn.Parameters.AddRange(methodParams);

            Type dataContextMethodReturnType = null;
            if (contractMethodInformation.MethodInfo.ReturnType != typeof(void))
            {
                Type innerType = contractMethodInformation.MethodInfo.ReturnType;

                if (innerType.BaseType == typeof(Array))
                {
                    innerType = innerType.GetElementType();
                }

                dataContextMethodReturnType = typeof(ISingleResult<>);
                dataContextMethodReturnType = dataContextMethodReturnType
                    .MakeGenericType(innerType);
            }
            else
            {
                dataContextMethodReturnType = typeof(int);
            }

            CodeStatement[] body =
                this.GenerateMethodBody(
                    dataContextMethodReturnType,
                    x =>
                    {
                        Type methodInfoType = typeof(MethodInfo);

                        // MethodInfo.GetCurrentMethod()
                        CodeMethodInvokeExpression getCurrentMethodRef =
                            new CodeMethodInvokeExpression(
                                new CodeTypeReferenceExpression(methodInfoType),
                                "GetCurrentMethod");

                        // (MethodInfo)MethodInfo.GetCurrentMethod()
                        CodeCastExpression castExpr = new CodeCastExpression(
                            methodInfoType,
                            getCurrentMethodRef);

                        // MethodInfo mi = (MethodInfo)MethodInfo.GetCurrentMethod()
                        CodeVariableDeclarationStatement methodInfo =
                            new CodeVariableDeclarationStatement(
                                methodInfoType,
                                "mi",
                                castExpr);

                        x.Add(methodInfo);

                        // New line.
                        CodeSnippetStatement newLine = new CodeSnippetStatement(string.Empty);

                        x.Add(newLine);

                        // object[] { param1, param2, etc }
                        CodeTypeReference objectType =
                            new CodeTypeReference(typeof(object));

                        CodeVariableReferenceExpression[] paramRefs =
                            methodParams
                                .Select(y => new CodeVariableReferenceExpression(y.Name))
                                .ToArray();

                        CodeArrayCreateExpression objectArrayCreate =
                            new CodeArrayCreateExpression(
                                objectType,
                                paramRefs);

                        // object[] methodParams = object[] { param1, param2, etc }
                        CodeVariableDeclarationStatement objectArrayDecl =
                            new CodeVariableDeclarationStatement(
                                typeof(object[]),
                                "methodParams",
                                objectArrayCreate);

                        x.Add(objectArrayDecl);

                        // Another new line.
                        x.Add(newLine);

                        // this.ExecuteMethodCall(this, mi, methodParams)
                        CodeThisReferenceExpression thisRef =
                            new CodeThisReferenceExpression();

                        CodeMethodReferenceExpression executeMethodCallRef =
                            new CodeMethodReferenceExpression(
                                thisRef,
                                "ExecuteMethodCall");

                        CodeMethodInvokeExpression executeMethodCall =
                            new CodeMethodInvokeExpression(
                                executeMethodCallRef,
                                thisRef,
                                new CodeVariableReferenceExpression(methodInfo.Name),
                                new CodeVariableReferenceExpression(objectArrayDecl.Name));

                        // IExecuteResult result = this.ExecuteMethodCall(this, mi, methodParams)
                        CodeVariableDeclarationStatement resultExecution =
                            new CodeVariableDeclarationStatement(
                                typeof(IExecuteResult),
                                "result",
                                executeMethodCall);

                        x.Add(resultExecution);

                        x.Add(newLine);

                        // result.ReturnValue
                        CodePropertyReferenceExpression returnValuePropRef =
                            new CodePropertyReferenceExpression(
                                new CodeVariableReferenceExpression(resultExecution.Name),
                                "ReturnValue");

                        // (ReturnType)result.ReturnValue
                        CodeCastExpression resultCastExpr =
                            new CodeCastExpression(
                                dataContextMethodReturnType,
                                returnValuePropRef);

                        // toReturn = (ReturnType)result.ReturnValue
                        CodeAssignStatement returnVarAssign =
                            new CodeAssignStatement(
                                new CodeVariableReferenceExpression("toReturn"),
                                resultCastExpr);

                        x.Add(returnVarAssign);
                    });

            toReturn.Statements.AddRange(body);

            toReturn.ReturnType =
                new CodeTypeReference(dataContextMethodReturnType);

            return toReturn;
        }

        private CodeTypeDeclaration CreateInterfaceImplementation(
            Type type,
            CodeTypeReference dataContextType,
            ContractMethodInformation[] contractMethodInformations)
        {
            CodeTypeDeclaration toReturn = null;

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
           
            CodeStatement[] body = this.GenerateMethodBody(
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

        private CodeStatement[] GenerateMethodBody(
            Type returnType,
            Action<List<CodeStatement>> bodyBuilderAction)
        {
            CodeStatement[] toReturn = null;

            List<CodeStatement> lines =
                new List<CodeStatement>();

            // Generate return type placeholder variables.
            CodeExpression initialisationValue = null;

            if (returnType.IsValueType)
            {
                initialisationValue = new CodeDefaultValueExpression(
                    new CodeTypeReference(returnType));
            }
            else
            {
                // Class type initialises to null.
                initialisationValue = new CodePrimitiveExpression(null);
            }

            CodeSnippetStatement emptyLine =
                    new CodeSnippetStatement(string.Empty);

            CodeVariableDeclarationStatement returnPlaceholderVariable = null;

            if (returnType != typeof(void))
            {
                returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        "toReturn",
                        initialisationValue);

                lines.Add(returnPlaceholderVariable);
                lines.Add(emptyLine);
            }

            bodyBuilderAction(lines);

            if (returnType != typeof(void))
            {
                lines.Add(emptyLine);

                CodeVariableReferenceExpression returnVarRef =
                    new CodeVariableReferenceExpression(
                        returnPlaceholderVariable.Name);

                CodeMethodReturnStatement returnStatement =
                    new CodeMethodReturnStatement(returnVarRef);

                lines.Add(returnStatement);
            }

            toReturn = lines.ToArray();

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

        private Assembly CompileStubAssembly(
            FileInfo destinationLocation,
            FileInfo hostAssemblyLocation,
            CodeCompileUnit codeCompileUnit)
        {
            Assembly toReturn = null;

            CompilerParameters compilerParameters = new CompilerParameters()
            {
                OutputAssembly = destinationLocation.FullName
            };

            Assembly interSprocAssembly = this.GetType().Assembly;

            // Add a reference to the InterSproc assembly.
            compilerParameters.ReferencedAssemblies.Add(
                interSprocAssembly.Location);

            // Add a reference to the host assembly.
            compilerParameters.ReferencedAssemblies
                .Add(hostAssemblyLocation.FullName);

            // Then the .net assemblies...
            compilerParameters.ReferencedAssemblies
                .Add(typeof(System.Data.IDbConnection).Assembly.Location);
            compilerParameters.ReferencedAssemblies
                .Add(typeof(DataContext).Assembly.Location);
            compilerParameters.ReferencedAssemblies
                .Add(typeof(Enumerable).Assembly.Location);

            CompilerResults compilerResults = this.csharpCodeProvider
                .CompileAssemblyFromDom(
                    compilerParameters,
                    codeCompileUnit);

            if (compilerResults.Errors.Count > 0)
            {
                CompilerError[] compilerErrors = compilerResults.Errors
                    .Cast<CompilerError>()
                    .ToArray();

                throw new StubGenerationException(compilerErrors);
            }
            else
            {
                toReturn = compilerResults.CompiledAssembly;
            }

            return toReturn;
        }

        private void GenerateCodeFile(
            FileInfo destinationLocation,
            CodeCompileUnit codeCompileUnit)
        {
            FileInfo codeFileLoc =
                new FileInfo($"{destinationLocation.FullName}.cs");

            using (StreamWriter fileStream = codeFileLoc.CreateText())
            {
                CodeGeneratorOptions codeGeneratorOptions =
                    new CodeGeneratorOptions()
                    {
                        BracingStyle = "C",
                        BlankLinesBetweenMembers = true
                    };

                this.csharpCodeProvider.GenerateCodeFromCompileUnit(
                    codeCompileUnit,
                    fileStream,
                    codeGeneratorOptions);
            }
        }
    }
}