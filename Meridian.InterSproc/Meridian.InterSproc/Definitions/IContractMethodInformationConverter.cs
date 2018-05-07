// ----------------------------------------------------------------------------
// <copyright file="IContractMethodInformationConverter.cs" company="MTCS">
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
    /// Describes the operations provided by the contract method information
    /// converter.
    /// </summary>
    public interface IContractMethodInformationConverter
    {
        /// <summary>
        /// Takes an interface type
        /// (<typeparamref name="TDatabaseContractType" />) and examines the
        /// structure/custom attributes, to compile the information into an
        /// array of <see cref="ContractMethodInformation" /> instances.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </returns>
        IEnumerable<ContractMethodInformation> GetContractMethodInformationFromContract<TDatabaseContractType>()
            where TDatabaseContractType : class;
    }
}