// ----------------------------------------------------------------------------
// <copyright file="EnvironmentTrustedAssembliesProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements
    /// <see cref="IEnvironmentTrustedAssembliesProvider.GetAssemblies()" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EnvironmentTrustedAssembliesProvider
        : IEnvironmentTrustedAssembliesProvider
    {
        private const string TrustedPlatformAssembliesKey =
            "TRUSTED_PLATFORM_ASSEMBLIES";

        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="EnvironmentTrustedAssembliesProvider" /> class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        public EnvironmentTrustedAssembliesProvider(
            ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements
        /// <see cref="IEnvironmentTrustedAssembliesProvider.GetAssemblies()" />.
        /// </summary>
        /// <returns>
        /// A list of assemblies as full paths, as a collection of
        /// <see cref="string" />s.
        /// </returns>
        public IEnumerable<string> GetAssemblies()
        {
            string[] toReturn = null;

            this.loggingProvider.Debug(
                $"Pulling back all {TrustedPlatformAssembliesKey}...");

            toReturn =
                ((string)AppContext.GetData(TrustedPlatformAssembliesKey))
                .Split(Path.PathSeparator);

            this.loggingProvider.Info(
                $"{toReturn.Length} {TrustedPlatformAssembliesKey} entries " +
                $"returned.");

            this.loggingProvider.Debug(
                $"The {TrustedPlatformAssembliesKey} returned:");

            foreach (string trustedAssemblyPath in toReturn)
            {
                this.loggingProvider.Debug($"-> {trustedAssemblyPath}");
            }

            return toReturn;
        }
    }
}