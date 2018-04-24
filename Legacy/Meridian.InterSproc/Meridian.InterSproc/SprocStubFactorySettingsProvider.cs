// ----------------------------------------------------------------------------
// <copyright
//      file="SprocStubFactorySettingsProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="ISprocStubFactorySettingsProvider" />. 
    /// </summary>
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
