namespace Meridian.InterSproc
{
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Microsoft.CSharp;

    public class StubAssemblyGenerator : IStubAssemblyGenerator
    {
        private const string BaseStubNamespace =
            "Meridian.InterSproc.TemporaryStub";

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
            FileInfo destinationLocation)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            CodeNamespace codeNamespace = new CodeNamespace(BaseStubNamespace);

            codeCompileUnit.Namespaces.Add(codeNamespace);

            if (this.stubAssemblyGeneratorSettingsProvider.GenerateAssemblyCodeFile)
            {
                this.GenerateCodeFile(destinationLocation, codeCompileUnit);
            }

            toReturn = this.CompileStubAssembly(
                destinationLocation,
                codeCompileUnit);

            return toReturn;
        }

        private Assembly CompileStubAssembly(
            FileInfo destinationLocation,
            CodeCompileUnit codeCompileUnit)
        {
            Assembly toReturn = null;

            CompilerParameters compilerParameters = new CompilerParameters()
            {
                OutputAssembly = destinationLocation.FullName
            };

            CompilerResults compilerResults = this.csharpCodeProvider
                .CompileAssemblyFromDom(
                    compilerParameters,
                    codeCompileUnit);

            toReturn = compilerResults.CompiledAssembly;

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
