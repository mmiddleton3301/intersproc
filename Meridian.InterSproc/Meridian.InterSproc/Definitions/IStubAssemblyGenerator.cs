namespace Meridian.InterSproc.Definitions
{
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Model;

    public interface IStubAssemblyGenerator
    {
        Assembly Create<DatabaseContractType>(
            FileInfo destinationLocation,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class;
    }
}
