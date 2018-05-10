// ----------------------------------------------------------------------------
// <copyright file="IStubMethodGeneratorFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.CodeDom;

    /// <summary>
    /// Describes the operations of the stub method generator factory.
    /// </summary>
    public interface IStubMethodGeneratorFactory
    {
        /// <summary>
        /// Creates an instance of type <see cref="IStubMethodGenerator" />.
        /// </summary>
        /// <param name="connectionStringMember">
        /// The connection string <see cref="CodeMemberField" /> for the parent
        /// stub's class.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IStubMethodGenerator" />.
        /// </returns>
        IStubMethodGenerator Create(CodeMemberField connectionStringMember);
    }
}