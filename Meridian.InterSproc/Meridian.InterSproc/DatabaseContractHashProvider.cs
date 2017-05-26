namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;

    public class DatabaseContractHashProvider : IDatabaseContractHashProvider
    {
        public byte[] GetContractHash<DatabaseContractType>() where DatabaseContractType : class
        {
            byte[] toReturn = null;

            return toReturn;
        }
    }
}
