// ----------------------------------------------------------------------------
// <copyright file="SprocStubFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using StructureMap;

    /// <summary>
    /// Implements <see cref="ISprocStubFactory" />.
    /// </summary>
    public class SprocStubFactory : ISprocStubFactory
    {
        private readonly IContractMethodInformationConverter contractMethodInformationConverter;
        private readonly IDatabaseContractHashProvider databaseContractHashProvider;
        private readonly ILoggingProvider loggingProvider;
        private readonly ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider;
        private readonly IStubAssemblyManager stubAssemblyManager;
        private readonly IStubInstanceProvider stubInstanceProvider;

        /// <summary>
        /// Initialises a new instance of the <see cref="SprocStubFactory" />
        /// class.
        /// </summary>
        /// <param name="contractMethodInformationConverter">
        /// An instance of <see cref="IContractMethodInformationConverter" />.
        /// </param>
        /// <param name="databaseContractHashProvider">
        /// An instance of <see cref="IDatabaseContractHashProvider" />.
        /// </param>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />.
        /// </param>
        /// <param name="sprocStubFactorySettingsProvider">
        /// An instance of <see cref="ISprocStubFactorySettingsProvider" />.
        /// </param>
        /// <param name="stubAssemblyManager">
        /// An instance of <see cref="IStubAssemblyManager" />.
        /// </param>
        /// <param name="stubInstanceProvider">
        /// An instance of <see cref="IStubInstanceProvider" />.
        /// </param>
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

        /// <summary>
        /// Creates a concrete instance of type
        /// <typeparamref name="TDatabaseContractType" /> for use within the
        /// host application.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        public static TDatabaseContractType Create<TDatabaseContractType>(
            string connStr)
            where TDatabaseContractType : class
        {
            TDatabaseContractType toReturn = null;

            // Overload - call the other Create with default options.
            SprocStubFactoryCreateOptions sprocStubFactoryCreateOptions =
                new SprocStubFactoryCreateOptions();

            toReturn = SprocStubFactory.Create<TDatabaseContractType>(
                connStr,
                sprocStubFactoryCreateOptions);

            return toReturn;
        }

        /// <summary>
        /// Creates a concrete instance of type
        /// <typeparamref name="TDatabaseContractType" /> for use within the
        /// host application.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <param name="sprocStubFactoryCreateOptions">
        /// An instance of <see cref="SprocStubFactoryCreateOptions" />,
        /// providing various optional settings to the create call.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        public static TDatabaseContractType Create<TDatabaseContractType>(
            string connStr,
            SprocStubFactoryCreateOptions sprocStubFactoryCreateOptions)
            where TDatabaseContractType : class
        {
            TDatabaseContractType toReturn = null;

            // Create the injectable settings providers...
            ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider =
                new SprocStubFactorySettingsProvider(
                    sprocStubFactoryCreateOptions.UseCachedStubAssemblies);
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider =
                new StubAssemblyGeneratorSettingsProvider(
                    sprocStubFactoryCreateOptions.GenerateAssemblyCodeFile);
            ILoggingProvider loggingProviderInstance =
                sprocStubFactoryCreateOptions.LoggingProvider;

            Registry registry = new Registry();
            Container container = new Container(
                x =>
                {
                    x.For<ILoggingProvider>().Use(loggingProviderInstance);
                    x.For<IStubAssemblyGeneratorSettingsProvider>()
                        .Use(stubAssemblyGeneratorSettingsProvider);
                    x.For<ISprocStubFactorySettingsProvider>()
                        .Use(sprocStubFactorySettingsProvider);

                    // Add our custom registry with all the base rules in it.
                    x.AddRegistry(registry);
                });

            SprocStubFactory sprocStubFactory =
                container.GetInstance<SprocStubFactory>();

            toReturn =
                sprocStubFactory.CreateStub<TDatabaseContractType>(connStr);

            return toReturn;
        }

        /// <summary>
        /// Implements
        /// <see cref="ISprocStubFactory.CreateStub{TDatabaseContractType}(string)" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        public TDatabaseContractType CreateStub<TDatabaseContractType>(
            string connStr)
            where TDatabaseContractType : class
        {
            TDatabaseContractType toReturn = null;

            Type type = typeof(TDatabaseContractType);

            ContractMethodInformation[] contractMethodInformations =
                this.contractMethodInformationConverter
                    .GetContractMethodInformationFromContract<TDatabaseContractType>();

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
                    this.stubAssemblyManager
                        .GetValidStubAssembly(contractHashStr);
            }
            else
            {
                this.loggingProvider.Warn(
                    $"{nameof(ISprocStubFactorySettingsProvider.UseCachedStubAssemblies)}" +
                    $" = {false.ToString(CultureInfo.InvariantCulture)} -" +
                    $"therefore, a new stub assembly will be generated. This " +
                    $"setting is not recommend for production.");
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
                this.stubAssemblyManager.CleanupTemporaryAssemblies();

                this.loggingProvider.Debug(
                    $"Temporary stub assemblies cleaned up. Now generating " +
                    $"a new stub assembly for {type.FullName}...");

                // Then generate a new stub assembly.
                temporaryStubAssembly = this.stubAssemblyManager
                    .GenerateStubAssembly<TDatabaseContractType>(
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

            // 4) Get a new instance of TDatabaseContractType from the
            //    temporary stub assembly using StructureMap (which is nicer
            //    than raw reflection!).
            this.loggingProvider.Debug(
                $"Using temporary stub assembly (location: " +
                $"\"{temporaryStubAssembly.Location}\") to create stub " +
                $"instance of {type.FullName}...");

            toReturn = this.stubInstanceProvider
                .GetInstance<TDatabaseContractType>(
                    temporaryStubAssembly,
                    connStr);

            this.loggingProvider.Info(
                $"Stub instance of type {type.FullName} created. Returning.");

            return toReturn;
        }
    }
}