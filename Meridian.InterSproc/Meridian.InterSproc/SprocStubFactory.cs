namespace Meridian.InterSproc
{
    using System;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class SprocStubFactory : ISprocStubFactory
    {
        private readonly IContractMethodInformationConverter contractMethodInformationConverter;
        private readonly IDatabaseContractHashProvider databaseContractHashProvider;
        private readonly ILoggingProvider loggingProvider;
        private readonly ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider;
        private readonly IStubAssemblyManager stubAssemblyManager;
        private readonly IStubInstanceProvider stubInstanceProvider;

        public SprocStubFactory(
            IContractMethodInformationConverter contractMethodInformationConverter,
            IDatabaseContractHashProvider databaseContractHashProvider,
            ILoggingProvider loggingProvider,
            ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider,
            IStubAssemblyManager stubAssemblyManager,
            IStubInstanceProvider stubInstanceProvider)
        {
            this.contractMethodInformationConverter =
                contractMethodInformationConverter;
            this.databaseContractHashProvider =
                databaseContractHashProvider;
            this.loggingProvider =
                loggingProvider;
            this.sprocStubFactorySettingsProvider =
                sprocStubFactorySettingsProvider;
            this.stubAssemblyManager =
                stubAssemblyManager;
            this.stubInstanceProvider =
                stubInstanceProvider;
        }

        public DatabaseContractType Create<DatabaseContractType>(
            string connStr)
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            Type type = typeof(DatabaseContractType);

            ContractMethodInformation[] contractMethodInformations =
                this.contractMethodInformationConverter
                    .GetContractMethodInformationFromContract<DatabaseContractType>();

            // 1) Take a hash of the contract as it stands.
            this.loggingProvider.Debug(
                $"Getting hash of contract {type.FullName}...");

            string contractHashStr = this.databaseContractHashProvider
                .GetContractHash(contractMethodInformations);

            this.loggingProvider.Info(
                $"Hash of {type.FullName} is \"{contractHashStr}\". Looking " +
                "for temporary stub assembly with this hash...");

            // 2) Look for a temporary stub assembly that matches this hash.
            Assembly temporaryStubAssembly = null;

            if (this.sprocStubFactorySettingsProvider.UseCachedStubAssemblies)
            {
                temporaryStubAssembly =
                    stubAssemblyManager.GetValidStubAssembly(contractHashStr);
            }

            // 3) If it doesn't exist, or if the contract hash doesn't match
            //    up, then lets generate a new temporary stub assembly.
            //    Otherwise, use the existing one.
            if (temporaryStubAssembly == null)
            {
                this.loggingProvider.Debug(
                    $"No temporary stub assemblies exist with the hash " +
                    $"\"{contractHashStr}\". Cleaning up any temporary " +
                    $"assemblies in the binaries directory...");

                // If it doesn't exist, clean up the existing temporary
                // stub assemblies from the bin directory.
                stubAssemblyManager.CleanupTemporaryAssemblies();

                this.loggingProvider.Debug(
                    $"Temporary stub assemblies cleaned up. Now generating " +
                    $"a new stub assembly for {type.FullName}...");

                // Then generate a new stub assembly.
                temporaryStubAssembly = stubAssemblyManager
                    .GenerateStubAssembly<DatabaseContractType>(
                        contractHashStr,
                        contractMethodInformations);

                this.loggingProvider.Info(
                    $"Temporary stub assembly generated for type " +
                    $"{type.FullName}, location: " +
                    $"\"{temporaryStubAssembly.Location}\".");
            }
            else
            {
                this.loggingProvider.Info(
                    $"Temporary stub assembly that matches the hash " +
                    $"exists. Location: " +
                    $"\"{temporaryStubAssembly.Location}\".");
            }

            // 4) Get a new instance of DatabaseContractType from the temporary
            //    stub assembly using StructureMap (which is nicer than
            //    raw reflection!).
            this.loggingProvider.Debug(
                $"Using temporary stub assembly (location: " +
                $"\"{temporaryStubAssembly.Location}\") to create stub " +
                $"instance of {type.FullName}...");

            toReturn = this.stubInstanceProvider
                .GetInstance<DatabaseContractType>(
                    temporaryStubAssembly,
                    connStr);

            this.loggingProvider.Info(
                $"Stub instance of type {type.FullName} created. Returning.");

            return toReturn;
        }
    }
}