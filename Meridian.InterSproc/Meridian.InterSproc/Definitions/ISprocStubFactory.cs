namespace Meridian.InterSproc.Definitions
{
    public interface ISprocStubFactory
    {
        DatabaseContractType Create<DatabaseContractType>()
            where DatabaseContractType : class;
    }
}