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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using Microsoft.CSharp;

    /// <summary>
    /// Implements <see cref="IStubAssemblyGenerator" />.
    /// </summary>
    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        private const string BaseStubNamespace =
            "Meridian.InterSproc.TemporaryStub";

        private readonly ILoggingProvider loggingProvider;
        private readonly IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider;

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
        public StubAssemblyGenerator(
            ILoggingProvider loggingProvider,
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider)
        {
            this.loggingProvider =
                loggingProvider;
            this.stubAssemblyGeneratorSettingsProvider =
                stubAssemblyGeneratorSettingsProvider;

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

            // TODO: The below needs to be rewritten to use Rosyln to compile
            //       the assembly.
            toReturn = this.CompileStubAssembly(
                destinationLocation,
                hostAssembly,
                codeCompileUnit);

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
            CodeCompileUnit codeCompileUnit)
        {
            Assembly toReturn = null;

            CompilerParameters compilerParameters = new CompilerParameters()
            {
                OutputAssembly = destinationLocation.FullName,
            };

            Assembly interSprocAssembly = this.GetType().Assembly;

            // Add a reference to the InterSproc assembly.
            compilerParameters.ReferencedAssemblies.Add(
                interSprocAssembly.Location);

            // Add a reference to the host assembly.
            compilerParameters.ReferencedAssemblies
                .Add(hostAssembly.Location);

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

            // Add usings...
            // Add System.Linq...
            toReturn.Imports.Add(
                new CodeNamespaceImport(
                    typeof(Enumerable).Namespace));

            // TODO: Review.
            // this.loggingProvider.Debug(
            //    $"Generating custom {nameof(DataContext)} class...");

            // Start first with the custom data context.
            // CodeTypeDeclaration customDataContext =
            //    this.stubDatabaseContextGenerator.CreateClass(
            //        databaseContractType,
            //        contractMethodInformations);
            // toReturn.Types.Add(customDataContext);

            // TODO: Review.
            // this.loggingProvider.Info(
            //    $"Custom {nameof(DataContext)} generated.");
            // CodeMemberMethod[] dataContextMethods =
            //     customDataContext.Members
            //         .Cast<CodeTypeMember>()
            //         .Where(x => x is CodeMemberMethod)
            //         .Select(x => x as CodeMemberMethod)
            //         .ToArray();
            this.loggingProvider.Debug(
                $"Generating implementation of " +
                $"{databaseContractType.Name}...");

            // Then the actual interface implementation.
            // CodeTypeDeclaration interfaceImplementation =
            //     this.stubImplementationGenerator.CreateClass(
            //         databaseContractType,
            //         new CodeTypeReference(customDataContext.Name),
            //         contractMethodInformations,
            //         dataContextMethods);
            // toReturn.Types.Add(interfaceImplementation);
            this.loggingProvider.Info(
                $"Implementation of {databaseContractType.Name} generated.");

            return toReturn;
        }
    }
}