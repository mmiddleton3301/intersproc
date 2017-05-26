namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;

    public interface IStubAssemblyManager
    {
        Assembly GetValidStubAssembly(string contractHashStr);

        void CleanupTemporaryAssemblies();

        Assembly GenerateStubAssembly<DatabaseContractType>(
            string contractHashStr)
            where DatabaseContractType : class;
    }
}
