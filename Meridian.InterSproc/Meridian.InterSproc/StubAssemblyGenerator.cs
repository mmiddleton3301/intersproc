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
            
            codeCompileUnit.Namespaces.Add(codeNamespace);

            Type type = typeof(DatabaseContractType);

            CodeTypeDeclaration customDataContext =
                this.CreateCustomDataContext(type);

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

        private CodeTypeDeclaration CreateCustomDataContext(Type type)
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
            constructor.Parameters.Add(
                new CodeParameterDeclarationExpression(
                    typeof(IStubImplementationSettingsProvider),
                    "stubImplementationSettingsProvider"));

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
            if (returnType != typeof(void))
            {
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

                CodeVariableDeclarationStatement returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        "toReturn",
                        initialisationValue);

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
                        BracingStyle = "C"
                    };

                this.csharpCodeProvider.GenerateCodeFromCompileUnit(
                    codeCompileUnit,
                    fileStream,
                    codeGeneratorOptions);
            }
        }
    }
}