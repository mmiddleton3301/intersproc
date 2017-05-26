namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Microsoft.CSharp;

    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        private const string BaseStubNamespace =
            "Meridian.InterSproc.TemporaryStub";

        private const string StubImplementationClassName =
            "StubImplementation{0}";

        private readonly CSharpCodeProvider csharpCodeProvider;
        private readonly IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider;

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
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));

            codeCompileUnit.Namespaces.Add(codeNamespace);

            CodeTypeDeclaration interfaceImplementation =
                this.CreateInterfaceImplementation<DatabaseContractType>(
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

        private CodeTypeDeclaration CreateInterfaceImplementation<DatabaseContractType>(
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class
        {
            CodeTypeDeclaration toReturn = null;

            Type type = typeof(DatabaseContractType);

            toReturn = new CodeTypeDeclaration()
            {
                Name = string.Format(StubImplementationClassName, type.Name),
                IsClass = true,
                Attributes = MemberAttributes.Public
            };

            toReturn.BaseTypes.Add(type);

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
            if (returnType != typeof(void))
            {
                // Generate return type placeholder variables.
                CodePrimitiveExpression nullReference =
                    new CodePrimitiveExpression(null);

                CodeVariableDeclarationStatement returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        "toReturn",
                        nullReference);

                toReturn.Statements.Add(returnPlaceholderVariable);

                CodeVariableReferenceExpression returnVarRef =
                    new CodeVariableReferenceExpression(
                        returnPlaceholderVariable.Name);

                CodeMethodReturnStatement returnStatement =
                    new CodeMethodReturnStatement(returnVarRef);

                toReturn.Statements.Add(returnStatement);

                toReturn.ReturnType = new CodeTypeReference(returnType);
            }

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

            compilerParameters.ReferencedAssemblies
                .Add(hostAssemblyLocation.FullName);

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
                        // May wish to configure this...
                    };

                this.csharpCodeProvider.GenerateCodeFromCompileUnit(
                    codeCompileUnit,
                    fileStream,
                    codeGeneratorOptions);
            }
        }
    }
}