// ----------------------------------------------------------------------------
// <copyright file="IStubAssemblyGenerator.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.Collections.Generic;
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Describes the operations of the stub assembly generator.
    /// </summary>
    public interface IStubAssemblyGenerator
    {
        /// <summary>
        /// Generates a new stub <see cref="IAssemblyWrapper" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="destinationLocation">
        /// The destination location for the new stub
        /// <see cref="IAssemblyWrapper" />.
        /// </param>
        /// <param name="contractMethodInformations">
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IAssemblyWrapper" />.
        /// </returns>
        IAssemblyWrapper Create<TDatabaseContractType>(
            IFileInfoWrapper destinationLocation,
            IEnumerable<ContractMethodInformation> contractMethodInformations)
            where TDatabaseContractType : class;
    }
}