// ----------------------------------------------------------------------------
// <copyright file="StubAssemblyGenerator.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Data;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Microsoft.CSharp;

    /// <summary>
    /// Implements <see cref="IStubAssemblyGenerator" />. 
    /// </summary>
    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        /// <summary>
        /// The base namespace for stub implementations.
        /// </summary>
        private const string BaseStubNamespace = 
            "Meridian.InterSproc.TemporaryStub";

        /// <summary>
        /// An instance of <see cref="CSharpCodeProvider" />, used by the
        /// generator to both compile and produce <c>.cs</c> files.
        /// </summary>
        private readonly CSharpCodeProvider csharpCodeProvider;

        /// <summary>
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// An instance of
        /// <see cref="IStubAssemblyGeneratorSettingsProvider" />. 
        /// </summary>
        private readonly IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider;

        /// <summary>
        /// An instance of <see cref="IStubDatabaseContextGenerator" />. 
        /// </summary>
        private readonly IStubDatabaseContextGenerator stubDatabaseContextGenerator;

        /// <summary>
        /// An instance of <see cref="IStubImplementationGenerator" />. 
        /// </summary>
        private readonly IStubImplementationGenerator stubImplementationGenerator;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubAssemblyGenerator" /> class. 
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </param>
        /// <param name="stubAssemblyGeneratorSettingsProvider">
        /// An instance of
        /// <see cref="IStubAssemblyGeneratorSettingsProvider" />. 
        /// </param>
        /// <param name="stubDatabaseContextGenerator">
        /// An instance of <see cref="IStubDatabaseContextGenerator" />. 
        /// </param>
        /// <param name="stubImplementationGenerator">
        /// An instance of <see cref="IStubImplementationGenerator" />. 
        /// </param>
        public StubAssemblyGenerator(
            ILoggingProvider loggingProvider,
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider,
            IStubDatabaseContextGenerator stubDatabaseContextGenerator,
            IStubImplementationGenerator stubImplementationGenerator)
        {
            this.loggingProvider =
                loggingProvider;
            this.stubAssemblyGeneratorSettingsProvider =
                stubAssemblyGeneratorSettingsProvider;
            this.stubDatabaseContextGenerator =
                stubDatabaseContextGenerator;
            this.stubImplementationGenerator =
                stubImplementationGenerator;

            this.csharpCodeProvider =
                new CSharpCodeProvider();
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubAssemblyGenerator.Create{DatabaseContractType}(FileInfo, ContractMethodInformation[])" />. 
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="destinationLocation">
        /// The destination location for the new stub <see cref="Assembly" />.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" />. 
        /// </returns>
        public Assembly Create<DatabaseContractType>(
            FileInfo destinationLocation,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            Type databaseContractType = typeof(DatabaseContractType);

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            this.loggingProvider.Debug(
                $"Generating {nameof(CodeNamespace)} for entire stub " +
                $"assembly...");

            // Generate the entire namespace and...
            CodeNamespace codeNamespace = this.GenerateEntireStubAssemblyDom(
                databaseContractType,
                contractMethodInformations);

            this.loggingProvider.Info(
                $"{nameof(CodeNamespace)} generation complete.");

            // ... add it to the CodeCompileUnit.
            codeCompileUnit.Namespaces.Add(codeNamespace);

            // Then, depending on the settings, output it to a code file.
            if (this.stubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)
            {
                this.loggingProvider.Debug(
                    $"{nameof(IStubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)} " +
                    $"= {true.ToString()}. Generating .cs code file prior " +
                    $"to compilation...");

                this.GenerateCodeFile(destinationLocation, codeCompileUnit);

                this.loggingProvider.Info($".cs file generated.");
            }

            Assembly hostAssembly = databaseContractType.Assembly;

            this.loggingProvider.Debug(
                $"Compiling {nameof(CodeNamespace)} to " +
                $"\"{destinationLocation.FullName}\"...");

            // Then finally, compile.
            toReturn = this.CompileStubAssembly(
                destinationLocation,
                hostAssembly,
                codeCompileUnit);

            this.loggingProvider.Info(
                $"\"{toReturn.FullName}\" generated. Returning.");

            return toReturn;
        }

        /// <summary>
        /// Compiles a stub assembly.
        /// Takes a <see cref="CodeCompileUnit" />, the
        /// <paramref name="hostAssembly" /> and a
        /// <paramref name="destinationLocation" />, and compiles an
        /// <see cref="Assembly" />.
        /// </summary>
        /// <param name="destinationLocation">
        /// A <see cref="FileInfo" /> describing the destination location of
        /// the compiled assembly.
        /// </param>
        /// <param name="hostAssembly">
        /// An instance of <see cref="Assembly" />, describing the host
        /// assembly containing the database contract. 
        /// </param>
        /// <param name="codeCompileUnit">
        /// An instance of <see cref="CodeCompileUnit" />, describing the
        /// <see cref="Assembly" /> to generate. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" />. 
        /// </returns>
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
            string[] dotNetAssemblies =
            {
                // System.Data
                typeof(IDbConnection).Assembly.Location,

                // System.Data.Linq
                typeof(DataContext).Assembly.Location,

                // System.Linq
                typeof(Enumerable).Assembly.Location
            };

            compilerParameters.ReferencedAssemblies.AddRange(dotNetAssemblies);

            this.loggingProvider.Debug(
                $"About to compile stub assembly with the following " +
                $"referenced assemblies:");

            foreach (string referencedAssembly in compilerParameters.ReferencedAssemblies)
            {
                this.loggingProvider.Debug($"-> {referencedAssembly}");
            }

            CompilerResults compilerResults = this.csharpCodeProvider
                .CompileAssemblyFromDom(
                    compilerParameters,
                    codeCompileUnit);

            if (compilerResults.Errors.Count > 0)
            {
                this.loggingProvider.Fatal(
                    $"Fatal - more than one error was thrown during " +
                    $"compilation! Throwing a " +
                    $"{nameof(StubGenerationException)}...");

                CompilerError[] compilerErrors = compilerResults.Errors
                    .Cast<CompilerError>()
                    .ToArray();

                throw new StubGenerationException(compilerErrors);
            }
            else
            {
                toReturn = compilerResults.CompiledAssembly;

                this.loggingProvider.Info(
                    $"Compilation complete. {nameof(Assembly)} = " +
                    $"\"{toReturn.FullName}\".");
            }

            return toReturn;
        }

        /// <summary>
        /// Generates a <c>.cs</c> code file from the specified
        /// <see cref="CodeCompileUnit" />. 
        /// </summary>
        /// <param name="destinationLocation">
        /// A <see cref="FileInfo" /> instance describing where to output
        /// the generated <c>.cs</c> file.
        /// </param>
        /// <param name="codeCompileUnit">
        /// An instance of <see cref="CodeCompileUnit" />, describing the
        /// <see cref="Assembly" /> that is about to be generated. 
        /// </param>
        private void GenerateCodeFile(
            FileInfo destinationLocation,
            CodeCompileUnit codeCompileUnit)
        {
            FileInfo codeFileLoc =
                new FileInfo($"{destinationLocation.FullName}.cs");

            this.loggingProvider.Debug(
                $"Generating .cs codefile at: \"{codeFileLoc.FullName}\"...");

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

            this.loggingProvider.Info(
                $".cs codefile generated at: \"{codeFileLoc.FullName}\".");
        }

        /// <summary>
        /// Generates the entire <see cref="Assembly" /> DOM, in the form of
        /// a <see cref="CodeNamespace" /> instance. 
        /// </summary>
        /// <param name="databaseContractType">
        /// The database contract interface type.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeNamespace" />, containing the stub
        /// assembly DOM.
        /// </returns>
        private CodeNamespace GenerateEntireStubAssemblyDom(
            Type databaseContractType,
            ContractMethodInformation[] contractMethodInformations)
        {
            CodeNamespace toReturn = new CodeNamespace(BaseStubNamespace);

            // Add usings...
            // Add System.Linq...
            toReturn.Imports.Add(
                new CodeNamespaceImport(
                    typeof(Enumerable).Namespace));

            this.loggingProvider.Debug(
                $"Generating custom {nameof(DataContext)} class...");

            // Start first with the custom data context.
            CodeTypeDeclaration customDataContext =
                this.stubDatabaseContextGenerator.CreateClass(
                    databaseContractType,
                    contractMethodInformations);
            toReturn.Types.Add(customDataContext);

            this.loggingProvider.Info(
                $"Custom {nameof(DataContext)} generated.");

            CodeMemberMethod[] dataContextMethods =
                customDataContext.Members
                    .Cast<CodeTypeMember>()
                    .Where(x => x is CodeMemberMethod)
                    .Select(x => x as CodeMemberMethod)
                    .ToArray();

            this.loggingProvider.Debug(
                $"Generating implementation of " +
                $"{databaseContractType.Name}...");

            // Then the actual interface implementation.
            CodeTypeDeclaration interfaceImplementation =
                this.stubImplementationGenerator.CreateClass(
                    databaseContractType,
                    new CodeTypeReference(customDataContext.Name),
                    contractMethodInformations,
                    dataContextMethods);
            toReturn.Types.Add(interfaceImplementation);

            this.loggingProvider.Info(
                $"Implementation of {databaseContractType.Name} generated.");

            return toReturn;
        }
    }
}