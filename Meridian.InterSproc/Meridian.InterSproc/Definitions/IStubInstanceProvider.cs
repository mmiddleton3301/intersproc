// ----------------------------------------------------------------------------
// <copyright file="IStubInstanceProvider.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;

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
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="temporaryStubAssembly">
        /// The recently generated temporary stub <see cref="Assembly" />. 
        /// </param>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// A concerete instance of
        /// <typeparamref name="DatabaseContractType" />.
        /// </returns>
        DatabaseContractType GetInstance<DatabaseContractType>(
            Assembly temporaryStubAssembly,
            string connStr)
            where DatabaseContractType : class;
    }
}