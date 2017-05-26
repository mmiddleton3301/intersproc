namespace Meridian.InterSproc.Definitions
{
    public interface IDatabaseContractHashProvider
    {
        string GetContractHash<DatabaseContractType>()
            where DatabaseContractType : class;
    }
}
