// ----------------------------------------------------------------------------
// <copyright file="SprocStubFactorySettingsProvider.cs" company="MTCS">
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

    /// <summary>
    /// Implements <see cref="ISprocStubFactorySettingsProvider" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SprocStubFactorySettingsProvider
        : ISprocStubFactorySettingsProvider
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="SprocStubFactorySettingsProvider" /> class.
        /// </summary>
        /// <param name="useCachedStubAssemblies">
        /// A value indicating whether or not to use cached stub assemblies
        /// (set to true when troubleshooting stub generation issues).
        /// </param>
        public SprocStubFactorySettingsProvider(bool useCachedStubAssemblies)
        {
            this.UseCachedStubAssemblies = useCachedStubAssemblies;
        }

        /// <summary>
        /// Gets a value indicating whether or not to use cached stub
        /// assemblies (set to true when troubleshooting stub generation
        /// issues).
        /// </summary>
        public bool UseCachedStubAssemblies
        {
            get;
            private set;
        }
    }
}