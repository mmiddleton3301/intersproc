// ----------------------------------------------------------------------------
// <copyright file="IStubInstanceProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    /// <summary>
    /// Describes the operations provided by the stub instance provider.
    /// </summary>
    public interface IStubInstanceProvider
    {
        /// <summary>
        /// After the <paramref name="temporaryStubAssembly" /> has been
        /// generated and passed to this method, <c>StructureMap</c> is used
        /// to create an instance of the new stub, complete with injected
        /// settings provider.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="temporaryStubAssembly">
        /// The recently generated temporary stub
        /// <see cref="IAssemblyWrapper" />.
        /// </param>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// A concerete instance of
        /// <typeparamref name="TDatabaseContractType" />.
        /// </returns>
        TDatabaseContractType GetInstance<TDatabaseContractType>(
            IAssemblyWrapper temporaryStubAssembly,
            string connStr)
            where TDatabaseContractType : class;
    }
}