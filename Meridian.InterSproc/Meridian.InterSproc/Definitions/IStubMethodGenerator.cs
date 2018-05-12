// ----------------------------------------------------------------------------
// <copyright file="IStubMethodGenerator.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.CodeDom;
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Describes the operations of the stub method generator.
    /// </summary>
    public interface IStubMethodGenerator
    {
        /// <summary>
        /// Creates an individual stub method implementation.
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />, containing
        /// information on the method to generate.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="CodeMemberMethod" />.
        /// </returns>
        CodeMemberMethod CreateMethod(
            ContractMethodInformation contractMethodInformation);
    }
}