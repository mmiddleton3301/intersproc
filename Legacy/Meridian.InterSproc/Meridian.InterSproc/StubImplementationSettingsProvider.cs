// ----------------------------------------------------------------------------
// <copyright
//      file="StubImplementationSettingsProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
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