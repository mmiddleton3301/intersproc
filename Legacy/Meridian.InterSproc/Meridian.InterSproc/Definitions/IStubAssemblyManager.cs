// ----------------------------------------------------------------------------
// <copyright file="IStubAssemblyManager.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;
    using Meridian.InterSproc.Model;

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
        /// Generates a new stub <see cref="Assembly" />.
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="contractHashStr">
        /// A hash of the contract about to be generated.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" />. 
        /// </returns>
        Assembly GenerateStubAssembly<DatabaseContractType>(
            string contractHashStr,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class;

        /// <summary>
        /// Inspects the output directory for any stub assemblies matching the
        /// input <paramref name="contractHashStr" />.
        /// Returns null if no <see cref="Assembly" /> exists for the given
        /// <paramref name="contractHashStr" />.
        /// </summary>
        /// <param name="contractHashStr">
        /// A hash of the database contract to look for.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" /> if found, otherwise null. 
        /// </returns>
        Assembly GetValidStubAssembly(string contractHashStr);
    }
}