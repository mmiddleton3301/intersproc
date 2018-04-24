// ----------------------------------------------------------------------------
// <copyright
//      file="IStubImplementationSettingsProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    /// <summary>
    /// Describes the operations of the stub implementation settings provider.
    /// </summary>
    public interface IStubImplementationSettingsProvider
    {
        /// <summary>
        /// Gets an SQL database connection string.
        /// </summary>
        string ConnStr
        {
            get;
        }
    }
}