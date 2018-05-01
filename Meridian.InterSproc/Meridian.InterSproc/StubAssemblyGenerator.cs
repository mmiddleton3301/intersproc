// ----------------------------------------------------------------------------
// <copyright file="StubAssemblyGenerator.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using Microsoft.CSharp;

    /// <summary>
    /// Implements <see cref="IStubAssemblyGenerator" />.
    /// TODO: This class probably needs breaking up more.
    /// </summary>
    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        private const string BaseStubNamespace =
            "Meridian.InterSproc.TemporaryStub";

        private readonly ILoggingProvider loggingProvider;
        private readonly IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider;
        private readonly IStubImplementationGenerator stubImplementationGenerator;

        private CSharpCodeProvider csharpCodeProvider;

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
        /// <param name="stubImplementationGenerator">
        /// An instance of <see cref="IStubImplementationGenerator" />.
        /// </param>
        public StubAssemblyGenerator(
            ILoggingProvider loggingProvider,
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider,
            IStubImplementationGenerator stubImplementationGenerator)
        {
            this.loggingProvider =
                loggingProvider;
            this.stubAssemblyGeneratorSettingsProvider =
                stubAssemblyGeneratorSettingsProvider;
            this.stubImplementationGenerator =
                stubImplementationGenerator;

            this.csharpCodeProvider =
                new CSharpCodeProvider();
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubAssemblyGenerator.Create{TDatabaseContractType}(FileInfo, ContractMethodInformation[])" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
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
        public Assembly Create<TDatabaseContractType>(
            FileInfo destinationLocation,
            ContractMethodInformation[] contractMethodInformations)
            where TDatabaseContractType : class
        {
            Assembly toReturn = null;

            Type databaseContractType = typeof(TDatabaseContractType);

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

            // Generate the source...
            string generatedAssemblyCode =
                this.GenerateAssemblyCode(codeCompileUnit);

            // Then, depending on the settings, output it to a code file.
            if (this.stubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)
            {
                this.loggingProvider.Debug(
                    $"{nameof(IStubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)} " +
                    $"= {true.ToString(CultureInfo.InvariantCulture)}. " +
                    $"Saving .cs code to file prior to compilation...");

                FileInfo codeFileLocation = new FileInfo(
                    $"{destinationLocation.FullName}.cs");
                using (FileStream fileStream = codeFileLocation.Create())
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(generatedAssemblyCode);
                    }
                }

                this.loggingProvider.Info($".cs file generated.");
            }

            Assembly hostAssembly = databaseContractType.Assembly;

            this.loggingProvider.Debug(
                $"Compiling {nameof(CodeNamespace)} to " +
                $"\"{destinationLocation.FullName}\"...");

            toReturn = this.CompileStubAssembly(
                destinationLocation,
                hostAssembly,
                generatedAssemblyCode);

            this.loggingProvider.Info(
                $"\"{toReturn.FullName}\" generated. Returning.");

            return toReturn;
        }

        /// <summary>
        /// Implements <see cref="IDisposable.Dispose()" />.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Diposes of class instance resources.
        /// </summary>
        /// <param name="disposing">
        /// If supplied with the value of true, then the class instance's
        /// resources are disposed of, and members set to null.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.csharpCodeProvider != null)
                {
                    this.csharpCodeProvider.Dispose();
                    this.csharpCodeProvider = null;
                }
            }
        }

        private Assembly CompileStubAssembly(
            FileInfo destinationLocation,
            Assembly hostAssembly,
            string assemblySource)
        {
            Assembly toReturn = null;

            this.loggingProvider.Debug(
                $"Parsing the {nameof(assemblySource)} into a " +
                $"{nameof(SyntaxTree)} instance...");

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(assemblySource);

            this.loggingProvider.Info(
                $"{nameof(SyntaxTree)} instance generated.");

            this.loggingProvider.Debug("Adding assembly references...");

            string[] trustedAssembliesPaths =
                ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
                .Split(Path.PathSeparator);

            string[] neededAssemblies = new string[]
            {
                Path.GetFileNameWithoutExtension(typeof(object).Assembly.Location),
                "System.Runtime",
                "mscorlib",
                "netstandard",

                "System.ComponentModel.Primitives",
                "System.Data",
                "System.Data.Common",
                "System.Data.SqlClient",
                "System.Linq",

                "Dapper",

                // Meridian.InterSproc
                this.GetType().Namespace,

                // The host assembly, whatever that might be called.
                Path.GetFileNameWithoutExtension(hostAssembly.Location),
            };

            IEnumerable<MetadataReference> references = trustedAssembliesPaths
                .Where(x => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(x)))
                .Select(x => MetadataReference.CreateFromFile(x));

            this.loggingProvider.Info(
                $"The following {references.Count()} references have been " +
                $"prepared:");

            foreach (MetadataReference reference in references)
            {
                this.loggingProvider.Info($"-> {reference.Display}");
            }

            this.loggingProvider.Debug(
                "About to perform compilation of assembly source...");

            CSharpCompilation compilation = CSharpCompilation.Create(
                destinationLocation.Name,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            this.loggingProvider.Info(
                "Compilation complete. Gathering results...");

            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (result.Success)
                {
                    this.loggingProvider.Info(
                        $"Compilation was successful.");

                    // Rewind the stream and...
                    ms.Seek(0, SeekOrigin.Begin);

                    this.loggingProvider.Debug(
                        $"Writing generated assembly to " +
                        $"{destinationLocation.FullName}...");

                    // Save it to file.
                    using (FileStream fileStream = destinationLocation.Create())
                    {
                        ms.WriteTo(fileStream);
                    }

                    this.loggingProvider.Info(
                        $"Generated assembly written to " +
                        $"{destinationLocation.FullName}. Loading assembly " +
                        $"into memory...");

                    // Then load it.
                    toReturn = Assembly.LoadFrom(destinationLocation.FullName);

                    this.loggingProvider.Info(
                        $"{toReturn} loaded into memory with success.");
                }
                else
                {
                    this.loggingProvider.Fatal(
                        "Compilation failed. Gathering error information...");

                    IEnumerable<Diagnostic> diagnostics = result.Diagnostics
                        .Where(x =>
                        x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error);

                    this.loggingProvider.Fatal(
                        $"{diagnostics.Count()} {nameof(Diagnostic)} " +
                        $"instance(s) fetched. Throwing in " +
                        $"{nameof(StubGenerationException)} instance...");

                    throw new StubGenerationException(diagnostics);
                }
            }

            return toReturn;
        }

        private string GenerateAssemblyCode(CodeCompileUnit codeCompileUnit)
        {
            string toReturn = null;

            byte[] codeBytes = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    CodeGeneratorOptions codeGeneratorOptions =
                        new CodeGeneratorOptions()
                        {
                            BracingStyle = "C",
                            BlankLinesBetweenMembers = true,
                        };

                    this.loggingProvider.Debug(
                        "Generating assembly code in memory...");

                    this.csharpCodeProvider.GenerateCodeFromCompileUnit(
                        codeCompileUnit,
                        streamWriter,
                        codeGeneratorOptions);
                }

                // Extract the bytes, so that we can convert it to a string.
                codeBytes = memoryStream.ToArray();

                this.loggingProvider.Info(
                    $"Assembly code generated ({codeBytes.Length} byte(s)).");
            }

            toReturn = Encoding.UTF8.GetString(codeBytes);

            return toReturn;
        }

        private CodeNamespace GenerateEntireStubAssemblyDom(
            Type databaseContractType,
            ContractMethodInformation[] contractMethodInformations)
        {
            CodeNamespace toReturn = new CodeNamespace(BaseStubNamespace);

            string[] namespacesToAdd =
            {
                // System.Data
                typeof(System.Data.IDbConnection).Namespace,

                // System.Data.SqlClient
                typeof(SqlConnection).Namespace,

                // System.Linq
                typeof(Enumerable).Namespace,

                // Dapper
                nameof(Dapper),

                // Meridian.InterSproc.Definitions
                typeof(IStubImplementationSettingsProvider).Namespace,
            };

            // Add usings...
            CodeNamespaceImport[] usingStatements = namespacesToAdd
                .Select(x => new CodeNamespaceImport(x))
                .ToArray();

            toReturn.Imports.AddRange(usingStatements);

            this.loggingProvider.Debug(
                $"Generating implementation of " +
                $"{databaseContractType.Name}...");

            CodeTypeDeclaration interfaceImplementation =
                this.stubImplementationGenerator.CreateClass(
                    databaseContractType,
                    contractMethodInformations);
            toReturn.Types.Add(interfaceImplementation);

            this.loggingProvider.Info(
                $"Implementation of {databaseContractType.Name} generated.");

            return toReturn;
        }
    }
}