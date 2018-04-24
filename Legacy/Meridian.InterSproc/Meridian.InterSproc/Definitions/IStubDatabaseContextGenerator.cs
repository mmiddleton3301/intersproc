// ----------------------------------------------------------------------------
// <copyright
//      file="IStubDatabaseContextGenerator.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.CodeDom;
    using Meridian.InterSproc.Model;
    
    /// <summary>
    /// Describes the operations provided by the stub database context
    /// generator.
    /// </summary>
    public interface IStubDatabaseContextGenerator
    {
        /// <summary>
        /// Creates the <see cref="System.Data.Linq.DataContext" />-derived
        /// class, used by the stub assembly.
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances,
        /// also describing the input <paramref name="databaseContractType" />.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeTypeDeclaration" />. 
        /// </returns>
        CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            ContractMethodInformation[] contractMethodInformations);
    }
}