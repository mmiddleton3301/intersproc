namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    public class SprocStubFactory : ISprocStubFactory
    {
        private readonly IDatabaseContractHashProvider databaseContractHashProvider;
        private readonly IStubAssemblyManager stubAssemblyManager;
        private readonly IStubInstanceProvider stubInstanceProvider;

        public SprocStubFactory(
            IDatabaseContractHashProvider databaseContractHashProvider,
            IStubAssemblyManager stubAssemblyManager,
            IStubInstanceProvider stubInstanceProvider)
        {
            this.databaseContractHashProvider = databaseContractHashProvider;
            this.stubAssemblyManager = stubAssemblyManager;
            this.stubInstanceProvider = stubInstanceProvider;
        }

        public DatabaseContractType Create<DatabaseContractType>()
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            // 1) Take a hash of the contract as it stands.
            byte[] contractHash = this.databaseContractHashProvider
                .GetContractHash<DatabaseContractType>();

            // 2) Look for a temporary stub assembly that matches this hash.
            Assembly temporaryStubAssembly =
                stubAssemblyManager.GetValidStubAssembly(contractHash);

            // 3) If it doesn't exist, or if the contract hash doesn't match
            //    up, then lets generate a new temporary stub assembly.
            //    Otherwise, use the existing one.
            if (temporaryStubAssembly == null)
            {
                // If it doesn't exist, clean up the existing temporary
                // stub assemblies from the bin directory.
                stubAssemblyManager.CleanupTemporaryAssemblies();

                // Then generate a new stub assembly.
                temporaryStubAssembly = stubAssemblyManager
                    .GenerateStubAssembly<DatabaseContractType>();
            }

            // 4) Get a new instance of DatabaseContractType from the temporary
            //    stub assembly using StructureMap (which is nicer than
            //    raw reflection!).
            toReturn = this.stubInstanceProvider
                .GetInstance<DatabaseContractType>(temporaryStubAssembly);

            return toReturn;
        }
    }
}