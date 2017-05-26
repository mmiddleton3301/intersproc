namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    public class StubInstanceProvider : IStubInstanceProvider
    {
        public DatabaseContractType GetInstance<DatabaseContractType>(Assembly temporaryStubAssembly) where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            return toReturn;
        }
    }
}
