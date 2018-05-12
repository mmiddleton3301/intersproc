// ----------------------------------------------------------------------------
// <copyright file="IEnvironmentTrustedAssembliesProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes the operations provided by the environment trusted assemblies
    /// provider.
    /// </summary>
    public interface IEnvironmentTrustedAssembliesProvider
    {
        /// <summary>
        /// Returns a list of paths to trusted assemblies for the given
        /// environment.
        /// </summary>
        /// <returns>
        /// A list of assemblies as full paths, as a collection of
        /// <see cref="string" />s.
        /// </returns>
        IEnumerable<string> GetAssemblies();
    }
}