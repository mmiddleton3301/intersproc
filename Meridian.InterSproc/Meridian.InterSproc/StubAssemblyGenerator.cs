namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Microsoft.CSharp;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        private const string BaseStubNamespace = 
            "Meridian.InterSproc.TemporaryStub";

        private readonly CSharpCodeProvider csharpCodeProvider;
        private readonly IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider;
        private readonly IStubDatabaseContextGenerator stubDatabaseContextGenerator;
        private readonly IStubImplementationGenerator stubImplementationGenerator;
        
        public StubAssemblyGenerator(
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider,
            IStubDatabaseContextGenerator stubDatabaseContextGenerator,
            IStubImplementationGenerator stubImplementationGenerator)
        {
            this.csharpCodeProvider =
                new CSharpCodeProvider();
            this.stubAssemblyGeneratorSettingsProvider =
                stubAssemblyGeneratorSettingsProvider;
            this.stubDatabaseContextGenerator =
                stubDatabaseContextGenerator;
            this.stubImplementationGenerator =
                stubImplementationGenerator;
        }

        public Assembly Create<DatabaseContractType>(
            FileInfo destinationLocation,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            Type databaseContractType = typeof(DatabaseContractType);

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            // Generate the entire namespace and...
            CodeNamespace codeNamespace = this.GenerateEntireStubAssemblyDom(
                databaseContractType,
                contractMethodInformations);

            // ... add it to the CodeCompileUnit.
            codeCompileUnit.Namespaces.Add(codeNamespace);

            // Then, depending on the settings, output it to a code file.
            if (this.stubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)
            {
                this.GenerateCodeFile(destinationLocation, codeCompileUnit);
            }

            Assembly hostAssembly = databaseContractType.Assembly;

            // Then finally, compile.
            toReturn = this.CompileStubAssembly(
                destinationLocation,
                hostAssembly,
                codeCompileUnit);

            return toReturn;
        }

        private CodeNamespace GenerateEntireStubAssemblyDom(
            Type databaseContractType,
            ContractMethodInformation[] contractMethodInformations)
        {
            CodeNamespace toReturn = new CodeNamespace(BaseStubNamespace);

            // Add usings...
            // Add System.Linq...
            toReturn.Imports.Add(
                new CodeNamespaceImport(typeof(Enumerable).Namespace));

            // Start first with the custom data context.
            CodeTypeDeclaration customDataContext =
                this.stubDatabaseContextGenerator.CreateClass(
                    databaseContractType,
                    contractMethodInformations);
            toReturn.Types.Add(customDataContext);

            CodeMemberMethod[] dataContextMethods =
                customDataContext.Members
                    .Cast<CodeTypeMember>()
                    .Where(x => x is CodeMemberMethod)
                    .Select(x => x as CodeMemberMethod)
                    .ToArray();

            // Then the actual interface implementation.
            CodeTypeDeclaration interfaceImplementation =
                this.stubImplementationGenerator.CreateClass(
                    databaseContractType,
                    new CodeTypeReference(customDataContext.Name),
                    contractMethodInformations,
                    dataContextMethods);
            toReturn.Types.Add(interfaceImplementation);

            return toReturn;
        }

        private Assembly CompileStubAssembly(
            FileInfo destinationLocation,
            Assembly hostAssembly,
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
                .Add(hostAssembly.Location);

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