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
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using StructureMap;

    /// <summary>
    /// Implements <see cref="IStubInstanceProvider" />.
    /// </summary>
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
        /// <see cref="IStubInstanceProvider.GetInstance{TDatabaseContractType}(Assembly, string)" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="temporaryStubAssembly">
        /// The recently generated temporary stub <see cref="Assembly" />.
        /// </param>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// A concerete instance of
        /// <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        public TDatabaseContractType GetInstance<TDatabaseContractType>(
            Assembly temporaryStubAssembly,
            string connStr)
            where TDatabaseContractType : class
        {
            TDatabaseContractType toReturn = null;

            CustomStubRegistry customStubRegistry =
                new CustomStubRegistry(temporaryStubAssembly);

            Container container = new Container(customStubRegistry);

            IStubImplementationSettingsProvider stubImplementationSettingsProvider =
                new StubImplementationSettingsProvider(connStr);

            this.loggingProvider.Debug(
                $"Activating a concrete instance of " +
                $"{typeof(TDatabaseContractType).FullName} using stub " +
                $"assembly \"{temporaryStubAssembly.FullName}\" using " +
                $"StructureMap...");

            toReturn = container
                .With(stubImplementationSettingsProvider)
                .GetInstance<TDatabaseContractType>();

            this.loggingProvider.Info($"Instance created: {toReturn}.");

            return toReturn;
        }
    }
}