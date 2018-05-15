// ----------------------------------------------------------------------------
// <copyright file="IAppDomainWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the operations provided by the <see cref="AppDomain" />
    /// wrapper.
    /// </summary>
    public interface IAppDomainWrapper
    {
        /// <summary>
        /// Gets all of the assemblies currently loaded in the current
        /// <see cref="AppDomain" />.
        /// </summary>
        /// <returns>
        /// A collection of instances of type <see cref="IAssemblyWrapper" />.
        /// </returns>
        IEnumerable<IAssemblyWrapper> GetAssemblies();
    }
}