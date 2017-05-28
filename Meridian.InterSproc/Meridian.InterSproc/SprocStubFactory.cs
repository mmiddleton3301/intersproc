// ----------------------------------------------------------------------------
// <copyright file="SprocStubFactory.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using StructureMap;

    /// <summary>
    /// Implements <see cref="ISprocStubFactory" />. 
    /// </summary>
    public class SprocStubFactory : ISprocStubFactory
    {
        /// <summary>
        /// An instance of <see cref="IContractMethodInformationConverter" />. 
        /// </summary>
        private readonly IContractMethodInformationConverter contractMethodInformationConverter;

        /// <summary>
        /// An instance of <see cref="IDatabaseContractHashProvider" />. 
        /// </summary>
        private readonly IDatabaseContractHashProvider databaseContractHashProvider;

        /// <summary>
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// An instance of <see cref="ISprocStubFactorySettingsProvider" />. 
        /// </summary>
        private readonly ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider;

        /// <summary>
        /// An instance of <see cref="IStubAssemblyManager" />. 
        /// </summary>
        private readonly IStubAssemblyManager stubAssemblyManager;

        /// <summary>
        /// An instance of <see cref="IStubInstanceProvider" />. 
        /// </summary>
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
        /// <typeparamref name="DatabaseContractType" /> for use within the
        /// host application.
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="DatabaseContractType" />.
        /// </returns>
        public static DatabaseContractType Create<DatabaseContractType>(
            string connStr)
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            // Overload - call the other Create with default options.
            SprocStubFactoryCreateOptions sprocStubFactoryCreateOptions =
                new SprocStubFactoryCreateOptions();

            toReturn = SprocStubFactory.Create<DatabaseContractType>(
                connStr,
                sprocStubFactoryCreateOptions);

            return toReturn;
        }

        /// <summary>
        /// Creates a concrete instance of type
        /// <typeparamref name="DatabaseContractType" /> for use within the
        /// host application.
        /// </summary>
        /// <typeparam name="DatabaseContractType">
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
        /// An instance of <typeparamref name="DatabaseContractType" />.
        /// </returns>
        public static DatabaseContractType Create<DatabaseContractType>(
            string connStr,
            SprocStubFactoryCreateOptions sprocStubFactoryCreateOptions)
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            // Create the injectable settings providers...
            ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider =
                new SprocStubFactorySettingsProvider(
                    sprocStubFactoryCreateOptions.UseCachedStubAssemblies);
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider =
                new StubAssemblyGeneratorSettingsProvider(
                    sprocStubFactoryCreateOptions.GenerateAssemblyCodeFile);

            Registry registry = new Registry();
            Container container = new Container(registry);

            SprocStubFactory sprocStubFactory =
                container
                    .With(sprocStubFactorySettingsProvider)
                    .With(stubAssemblyGeneratorSettingsProvider)
                    .GetInstance<SprocStubFactory>();

            toReturn =
                sprocStubFactory.CreateStub<DatabaseContractType>(connStr);

            return toReturn;
        }

        /// <summary>
        /// Implements
        /// <see cref="ISprocStubFactory.CreateStub{DatabaseContractType}(string)" />. 
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="DatabaseContractType" />.  
        /// </returns>
        public DatabaseContractType CreateStub<DatabaseContractType>(
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
                    this.stubAssemblyManager
                        .GetValidStubAssembly(contractHashStr);
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