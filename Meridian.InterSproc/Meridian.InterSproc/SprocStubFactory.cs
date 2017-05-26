namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;

    public class SprocStubFactory : ISprocStubFactory
    {
        public DatabaseContractType Create<DatabaseContractType>()
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            return toReturn;
        }
    }
}