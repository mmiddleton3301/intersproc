namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;

    public interface IStubAssemblyManager
    {
        Assembly GetValidStubAssembly(string contractHashStr);

        void CleanupTemporaryAssemblies();

        Assembly GenerateStubAssembly<DatabaseContractType>()
            where DatabaseContractType : class;
    }
}
