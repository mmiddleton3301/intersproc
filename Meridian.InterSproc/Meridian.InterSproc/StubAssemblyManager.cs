namespace Meridian.InterSproc
{
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class StubAssemblyManager : IStubAssemblyManager
    {
        private const string TemporaryStubAssemblyName =
            "Temporary_{0}.isa";

        private readonly DirectoryInfo temporaryAssemblyLocation;
        private readonly IStubAssemblyGenerator stubAssemblyGenerator;

        public StubAssemblyManager(
            IStubAssemblyGenerator stubAssemblyGenerator)
        {
            Assembly executing = Assembly.GetExecutingAssembly();

            string assemlyExecutionLocation =
                $"{executing.Location}";

            this.temporaryAssemblyLocation =
                (new FileInfo(assemlyExecutionLocation)).Directory;

            this.stubAssemblyGenerator = stubAssemblyGenerator;
        }

        public void CleanupTemporaryAssemblies()
        {
            string wildcardAssem =
                string.Format(TemporaryStubAssemblyName, "*");

            FileInfo[] temporaryAssemblies =
                this.temporaryAssemblyLocation.GetFiles(wildcardAssem);

            FileInfo sourceFileSearch = null;
            foreach (FileInfo fileInfo in temporaryAssemblies)
            {
                // Unload it first - if it's in the bin dir, then it'll get
                // loaded by the host app by default.
                fileInfo.Delete();

                sourceFileSearch = new FileInfo(fileInfo.Name + ".cs");

                if (sourceFileSearch.Exists)
                {
                    sourceFileSearch.Delete();
                }
            }
        }

        public Assembly GenerateStubAssembly<DatabaseContractType>(
            string contractHashStr,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            string destinationFilename = string.Format(
                TemporaryStubAssemblyName,
                contractHashStr);

            FileInfo destinationLocation = new FileInfo(
                $"{temporaryAssemblyLocation.FullName}\\{destinationFilename}");

            toReturn =
                this.stubAssemblyGenerator.Create<DatabaseContractType>(
                    destinationLocation,
                    contractMethodInformations);

            return toReturn;
        }

        public Assembly GetValidStubAssembly(string contractHashStr)
        {
            Assembly toReturn = null;

            string searchFilename = string.Format(
                TemporaryStubAssemblyName,
                contractHashStr);

            FileInfo fileInfo = new FileInfo(
                $"{this.temporaryAssemblyLocation.FullName}\\{searchFilename}");

            if (fileInfo.Exists)
            {
                // TODO: Needs testing when we're actually generating
                // temporary stub assemblies. Should work fine, though.
                toReturn = Assembly.LoadFile(fileInfo.FullName);
            }

            return toReturn;
        }
    }
}
