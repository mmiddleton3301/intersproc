namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;

    public interface IStubInstanceProvider
    {
        DatabaseContractType GetInstance<DatabaseContractType>(
            Assembly temporaryStubAssembly)
            where DatabaseContractType : class;
    }
}
