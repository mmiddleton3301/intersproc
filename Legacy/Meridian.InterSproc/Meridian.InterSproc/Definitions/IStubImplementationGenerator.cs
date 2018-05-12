// ----------------------------------------------------------------------------
// <copyright
//      file="IStubImplementationGenerator.cs"
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
    /// Describes the operations provided by the stub implementation generator.
    /// </summary>
    public interface IStubImplementationGenerator
    {
        /// <summary>
        /// Creates the concrete implementation of
        /// <paramref name="databaseContractType" />.
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="dataContextType">
        /// An instance of <see cref="CodeTypeReference" />, refering to the
        /// previously generated
        /// <see cref="System.Data.Linq.DataContext" />-derived class.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </param>
        /// <param name="dataContextMethods">
        /// An array of <see cref="CodeMemberMethod" /> instances, beloinging
        /// to the input <paramref name="dataContextType" />.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeTypeDeclaration" />. 
        /// </returns>
        CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            CodeTypeReference dataContextType,
            ContractMethodInformation[] contractMethodInformations,
            CodeMemberMethod[] dataContextMethods);
    }
}