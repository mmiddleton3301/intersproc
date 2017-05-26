namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;

    public interface IStubAssemblyManager
    {
        Assembly GetValidStubAssembly(byte[] contractHash);

        void CleanupTemporaryAssemblies();

        Assembly GenerateStubAssembly<DatabaseContractType>()
            where DatabaseContractType : class;
    }
}
