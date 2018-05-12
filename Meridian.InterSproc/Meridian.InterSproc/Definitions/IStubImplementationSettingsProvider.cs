// ----------------------------------------------------------------------------
// <copyright file="IStubImplementationSettingsProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
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