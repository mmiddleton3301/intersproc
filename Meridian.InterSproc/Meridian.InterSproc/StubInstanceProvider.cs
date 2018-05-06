// ----------------------------------------------------------------------------
// <copyright file="StubInstanceProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Diagnostics.CodeAnalysis;
    using Meridian.InterSproc.Definitions;
    using StructureMap;

    /// <summary>
    /// Implements <see cref="IStubInstanceProvider" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StubInstanceProvider : IStubInstanceProvider
    {
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubInstanceProvider" /> class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />.
        /// </param>
        public StubInstanceProvider(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubInstanceProvider.GetInstance{TDatabaseContractType}(IAssemblyWrapper, string)" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="temporaryStubAssembly">
        /// The recently generated temporary stub
        /// <see cref="IAssemblyWrapper" />.
        /// </param>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// A concerete instance of
        /// <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        public TDatabaseContractType GetInstance<TDatabaseContractType>(
            IAssemblyWrapper temporaryStubAssembly,
            string connStr)
            where TDatabaseContractType : class
        {
            TDatabaseContractType toReturn = null;

            CustomStubRegistry customStubRegistry =
                new CustomStubRegistry(temporaryStubAssembly);

            IStubImplementationSettingsProvider stubImplementationSettingsProvider =
                new StubImplementationSettingsProvider(connStr);

            Container container = new Container(
                x =>
                {
                    // Provide our stub implementation settings provider
                    // instance and then...
                    x.For<IStubImplementationSettingsProvider>()
                        .Use(stubImplementationSettingsProvider);

                    // ... add the custom registry.
                    x.AddRegistry(customStubRegistry);
                });

            this.loggingProvider.Debug(
                $"Activating a concrete instance of " +
                $"{typeof(TDatabaseContractType).FullName} using stub " +
                $"assembly \"{temporaryStubAssembly.FullName}\" using " +
                $"StructureMap...");

            toReturn = container.GetInstance<TDatabaseContractType>();

            this.loggingProvider.Info($"Instance created: {toReturn}.");

            return toReturn;
        }
    }
}