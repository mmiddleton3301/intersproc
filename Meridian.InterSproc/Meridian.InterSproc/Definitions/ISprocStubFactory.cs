// ----------------------------------------------------------------------------
// <copyright file="ISprocStubFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    /// <summary>
    /// Describes the operations provided by the sproc stub factory.
    /// </summary>
    public interface ISprocStubFactory
    {
        /// <summary>
        /// Creates a concrete instance of type
        /// <typeparamref name="TDatabaseContractType" /> for use within the
        /// host application.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        TDatabaseContractType CreateStub<TDatabaseContractType>(string connStr)
            where TDatabaseContractType : class;
    }
}