// ----------------------------------------------------------------------------
// <copyright file="ISprocStubFactorySettingsProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
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