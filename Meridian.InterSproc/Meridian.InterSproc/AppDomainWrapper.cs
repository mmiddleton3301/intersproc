// ----------------------------------------------------------------------------
// <copyright file="AppDomainWrapper.cs" company="MTCS">
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
    using System.Linq;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IAppDomainWrapper" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AppDomainWrapper : IAppDomainWrapper
    {
        private readonly IAssemblyWrapperFactory assemblyWrapperFactory;
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the <see cref="AppDomainWrapper" />
        /// class.
        /// </summary>
        /// <param name="assemblyWrapperFactory">
        /// An instance of type <see cref="IAssemblyWrapperFactory" />.
        /// </param>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        public AppDomainWrapper(
            IAssemblyWrapperFactory assemblyWrapperFactory,
            ILoggingProvider loggingProvider)
        {
            this.assemblyWrapperFactory = assemblyWrapperFactory;
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements <see cref="IAppDomainWrapper.GetAssemblies()" />.
        /// </summary>
        /// <returns>
        /// A collection of instances of type <see cref="IAssemblyWrapper" />.
        /// </returns>
        public IEnumerable<IAssemblyWrapper> GetAssemblies()
        {
            IEnumerable<IAssemblyWrapper> toReturn = null;

            toReturn = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Select(x => this.assemblyWrapperFactory.Create(x));

            return toReturn;
        }
    }
}