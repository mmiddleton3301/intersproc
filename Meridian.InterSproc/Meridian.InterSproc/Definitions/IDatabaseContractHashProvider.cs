namespace Meridian.InterSproc.Definitions
{
    using Meridian.InterSproc.Model;

    public interface IDatabaseContractHashProvider
    {
        string GetContractHash(
            ContractMethodInformation[] contractMethodInformations);
    }
}
