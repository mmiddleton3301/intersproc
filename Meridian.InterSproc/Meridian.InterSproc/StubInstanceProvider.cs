namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using StructureMap;
    using StructureMap.Pipeline;

    public class StubInstanceProvider : IStubInstanceProvider
    {
        public DatabaseContractType GetInstance<DatabaseContractType>(
            Assembly temporaryStubAssembly,
            string connStr)
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            CustomStubRegistry customStubRegistry =
                new CustomStubRegistry(temporaryStubAssembly);

            Container container = new Container(customStubRegistry);

            IStubImplementationSettingsProvider stubImplementationSettingsProvider =
                new StubImplementationSettingsProvider(connStr);

            toReturn = container
                .With(stubImplementationSettingsProvider)
                .GetInstance<DatabaseContractType>();

            return toReturn;
        }
    }
}