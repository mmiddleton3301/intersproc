// ----------------------------------------------------------------------------
// <copyright
//      file="IContractMethodInformationConverter.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Describes the operations provided by the contract method information
    /// converter.
    /// </summary>
    public interface IContractMethodInformationConverter
    {
        /// <summary>
        /// Takes an interface type
        /// (<typeparamref name="DatabaseContractType" />) and examines the
        /// structure/custom attributes, to compile the information into an
        /// array of <see cref="ContractMethodInformation" /> instances. 
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <returns>
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </returns>
        ContractMethodInformation[] GetContractMethodInformationFromContract<DatabaseContractType>()
            where DatabaseContractType : class;
    }
}