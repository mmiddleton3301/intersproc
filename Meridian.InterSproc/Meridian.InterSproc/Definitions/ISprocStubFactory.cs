// ----------------------------------------------------------------------------
// <copyright file="ISprocStubFactory.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    /// <summary>
    /// Describes the operations provided by the sproc stub factory.
    /// </summary>
    internal interface ISprocStubFactory
    {
        /// <summary>
        /// Creates a concrete instance of type
        /// <typeparamref name="DatabaseContractType" /> for use within the
        /// host application.
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="DatabaseContractType" />.  
        /// </returns>
        DatabaseContractType CreateStub<DatabaseContractType>(string connStr)
            where DatabaseContractType : class;
    }
}