// ----------------------------------------------------------------------------
// <copyright file="StubImplementationSettingsProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IStubImplementationSettingsProvider" />.
    /// </summary>
    public class StubImplementationSettingsProvider
        : IStubImplementationSettingsProvider
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubImplementationSettingsProvider" /> class.
        /// </summary>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        public StubImplementationSettingsProvider(string connStr)
        {
            this.ConnStr = connStr;
        }

        /// <summary>
        /// Gets an SQL database connection string.
        /// </summary>
        public string ConnStr
        {
            get;
            private set;
        }
    }
}