namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;
    using Meridian.InterSproc.Model;

    public interface IStubAssemblyManager
    {
        Assembly GetValidStubAssembly(string contractHashStr);

        void CleanupTemporaryAssemblies();

        Assembly GenerateStubAssembly<DatabaseContractType>(
            string contractHashStr,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class;
    }
}
