// ----------------------------------------------------------------------------
// <copyright file="DefaultContainerProvider.cs" company="MTCS">
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
    /// Prepares default <see cref="IContainer" /> instances for use by the
    /// <see cref="SprocStubFactory" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DefaultContainerProvider
    {
        /// <summary>
        /// Creates an instance of type <see cref="IContainer" />, with
        /// injected settings providers and logger.
        /// </summary>
        /// <param name="loggingProviderInstance">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        /// <param name="stubAssemblyGeneratorSettingsProvider">
        /// An instance of type
        /// <see cref="IStubAssemblyGeneratorSettingsProvider" />.
        /// </param>
        /// <param name="sprocStubFactorySettingsProvider">
        /// An instance of type
        /// <see cref="ISprocStubFactorySettingsProvider" />.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IContainer" />.
        /// </returns>
        public static IContainer Create(
            ILoggingProvider loggingProviderInstance,
            IStubAssemblyGeneratorSettingsProvider stubAssemblyGeneratorSettingsProvider,
            ISprocStubFactorySettingsProvider sprocStubFactorySettingsProvider)
        {
            IContainer toReturn = null;

            Registry registry = new Registry();
            toReturn = new Container(
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

            return toReturn;
        }
    }
}