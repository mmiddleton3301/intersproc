namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using StructureMap;

    public class StubInstanceProvider : IStubInstanceProvider
    {
        public DatabaseContractType GetInstance<DatabaseContractType>(
            Assembly temporaryStubAssembly)
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            CustomStubRegistry customStubRegistry =
                new CustomStubRegistry(temporaryStubAssembly);

            Container container = new Container(customStubRegistry);

            toReturn = container.GetInstance<DatabaseContractType>();

            return toReturn;
        }
    }
}
