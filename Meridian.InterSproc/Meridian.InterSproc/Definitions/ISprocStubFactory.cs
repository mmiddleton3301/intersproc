namespace Meridian.InterSproc.Definitions
{
    public interface ISprocStubFactory
    {
        DatabaseContractType Create<DatabaseContractType>(string connStr)
            where DatabaseContractType : class;
    }
}