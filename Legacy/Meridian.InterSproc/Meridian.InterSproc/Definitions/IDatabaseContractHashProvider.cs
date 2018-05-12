// ----------------------------------------------------------------------------
// <copyright
//      file="IDatabaseContractHashProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Describes the operations of the database contract hash provider.
    /// </summary>
    public interface IDatabaseContractHashProvider
    {
        /// <summary>
        /// Creates a filename-safe hash from the combined
        /// <see cref="ContractMethodInformation" /> instances passed in. 
        /// </summary>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// A base-64 encoded SHA-1 hash, describing the uniqueness of the
        /// values passed in via
        /// <paramref name="contractMethodInformations" />.
        /// </returns>
        string GetContractHash(
            ContractMethodInformation[] contractMethodInformations);
    }
}