namespace Meridian.InterSproc
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    public class StubAssemblyManager : IStubAssemblyManager
    {
        private const string TemporaryStubAssemblyName =
            "InterSprocTemporaryStubAssembly_{0}.dll";

        private readonly DirectoryInfo executingAssemblyLocation;

        public StubAssemblyManager()
        {
            Assembly executing = Assembly.GetExecutingAssembly();

            string assemlyExecutionLocation = executing.Location;

            this.executingAssemblyLocation =
                (new FileInfo(executing.Location)).Directory;
        }

        public void CleanupTemporaryAssemblies()
        {
            string wildcardAssem =
                string.Format(TemporaryStubAssemblyName, "*");

            // TODO: The below needs testing. I'm not even sure if it'd work!
            FileInfo[] temporaryAssemblies =
                this.executingAssemblyLocation.GetFiles(wildcardAssem);

            foreach(FileInfo fileInfo in temporaryAssemblies)
            {
                fileInfo.Delete();
            }
        }

        public Assembly GenerateStubAssembly<DatabaseContractType>()
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            return toReturn;
        }

        public Assembly GetValidStubAssembly(string contractHashStr)
        {
            Assembly toReturn = null;

            string searchFilename = string.Format(
                TemporaryStubAssemblyName,
                contractHashStr);

            FileInfo fileInfo = new FileInfo(
                $"{this.executingAssemblyLocation.FullName}\\{searchFilename}");

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
