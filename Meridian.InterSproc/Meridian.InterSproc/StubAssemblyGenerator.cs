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

        public Assembly Create<DatabaseContractType>(
            FileInfo destinationLocation)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            CodeNamespace codeNamespace = new CodeNamespace(BaseStubNamespace);

            codeCompileUnit.Namespaces.Add(codeNamespace);

            CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();

            CompilerParameters compilerParameters = new CompilerParameters()
            {
                OutputAssembly = destinationLocation.FullName
            };

            CompilerResults compilerResults = csharpCodeProvider
                .CompileAssemblyFromDom(
                    compilerParameters,
                    codeCompileUnit);

            return toReturn;
        }
    }
}
