namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    internal class StubAssemblyManager : IStubAssemblyManager
    {
        public void CleanupTemporaryAssemblies()
        {
        }

        public Assembly GenerateStubAssembly<DatabaseContractType>()
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            return toReturn;
        }

        public Assembly GetValidStubAssembly(byte[] contractHash)
        {
            Assembly toReturn = null;

            return toReturn;
        }
    }
}
