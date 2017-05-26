namespace Meridian.InterSproc.Definitions
{
    using Meridian.InterSproc.Model;

    public interface IContractMethodInformationConverter
    {
        ContractMethodInformation[] GetContractMethodInformationFromContract<DatabaseContractType>()
            where DatabaseContractType : class;

    }
}
