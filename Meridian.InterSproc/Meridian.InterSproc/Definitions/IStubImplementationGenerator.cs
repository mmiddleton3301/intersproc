// ----------------------------------------------------------------------------
// <copyright file="IStubImplementationGenerator.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Describes the operations provided by the stub implementation generator.
    /// </summary>
    public interface IStubImplementationGenerator
    {
        /// <summary>
        /// Creates a dapper-based implementation of the
        /// <paramref name="databaseContractType" /> based on the array
        /// <see cref="ContractMethodInformation" /> instances passed in.
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="contractMethodInformations">
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeTypeDeclaration" />.
        /// </returns>
        CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            IEnumerable<ContractMethodInformation> contractMethodInformations);
    }
}