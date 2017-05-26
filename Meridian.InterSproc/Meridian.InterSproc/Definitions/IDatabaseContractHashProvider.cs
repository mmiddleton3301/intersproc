namespace Meridian.InterSproc.Definitions
{
    public interface IDatabaseContractHashProvider
    {
        byte[] GetContractHash<DatabaseContractType>()
            where DatabaseContractType : class;
    }
}
