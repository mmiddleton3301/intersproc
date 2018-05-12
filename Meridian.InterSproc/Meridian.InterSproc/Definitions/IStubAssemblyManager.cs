// ----------------------------------------------------------------------------
// <copyright file="IStubAssemblyManager.cs" company="MTCS">
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
    /// Describes the operations provided by the stub assembly manager.
    /// </summary>
    public interface IStubAssemblyManager
    {
        /// <summary>
        /// Cleans up all temporary stub assemblies in the output directory.
        /// </summary>
        void CleanupTemporaryAssemblies();

        /// <summary>
        /// Generates a new stub <see cref="IAssemblyWrapper" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="contractHashStr">
        /// A hash of the contract about to be generated.
        /// </param>
        /// <param name="contractMethodInformations">
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IAssemblyWrapper" />.
        /// </returns>
        IAssemblyWrapper GenerateStubAssembly<TDatabaseContractType>(
            string contractHashStr,
            IEnumerable<ContractMethodInformation> contractMethodInformations)
            where TDatabaseContractType : class;

        /// <summary>
        /// Inspects the output directory for any stub assemblies matching the
        /// input <paramref name="contractHashStr" />.
        /// Returns null if no <see cref="IAssemblyWrapper" /> exists for the
        /// given <paramref name="contractHashStr" />.
        /// </summary>
        /// <param name="contractHashStr">
        /// A hash of the database contract to look for.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IAssemblyWrapper" /> if found,
        /// otherwise null.
        /// </returns>
        IAssemblyWrapper GetValidStubAssembly(string contractHashStr);
    }
}