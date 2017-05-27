// ----------------------------------------------------------------------------
// <copyright file="IStubAssemblyGenerator.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Describes the operations of the stub assembly generator.
    /// </summary>
    public interface IStubAssemblyGenerator
    {
        /// <summary>
        /// Generates a new stub <see cref="Assembly" />.
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="destinationLocation">
        /// The destination location for the new stub <see cref="Assembly" />.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" />. 
        /// </returns>
        Assembly Create<DatabaseContractType>(
            FileInfo destinationLocation,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class;
    }
}