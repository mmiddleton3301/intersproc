// ----------------------------------------------------------------------------
// <copyright file="IStubDomGenerator.cs" company="MTCS">
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
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Describes the operations provided by the stub DOM Generator.
    /// </summary>
    public interface IStubDomGenerator
    {
        /// <summary>
        /// Generates the stub assembly's DOM in its entirity.
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="contractMethodInformations">
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeNamespace" />, containing
        /// implementations of all the relevant sub-classes and configuration
        /// to produce the stub assembly.
        /// </returns>
        CodeNamespace GenerateEntireStubAssemblyDom(
            Type databaseContractType,
            IEnumerable<ContractMethodInformation> contractMethodInformations);
    }
}