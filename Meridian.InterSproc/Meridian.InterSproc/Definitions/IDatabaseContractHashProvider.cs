// ----------------------------------------------------------------------------
// <copyright file="IDatabaseContractHashProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Collections.Generic;
    using Meridian.InterSproc.Models;

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
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// A base-64 encoded SHA-1 hash, describing the uniqueness of the
        /// values passed in via
        /// <paramref name="contractMethodInformations" />.
        /// </returns>
        string GetContractHash(
            IEnumerable<ContractMethodInformation> contractMethodInformations);
    }
}