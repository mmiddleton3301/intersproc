// ----------------------------------------------------------------------------
// <copyright
//      file="ISprocStubFactorySettingsProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    /// <summary>
    /// Describes the operations provided by the sproc stub factory settings
    /// provider.
    /// </summary>
    public interface ISprocStubFactorySettingsProvider
    {
        /// <summary>
        /// Gets a value indicating whether or not to use cached stub
        /// assemblies (set to true when troubleshooting stub generation
        /// issues).
        /// </summary>
        bool UseCachedStubAssemblies
        {
            get;
        }
    }
}