// ----------------------------------------------------------------------------
// <copyright file="ICSharpCompilationWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Describes the operations provided by the
    /// <see cref="CSharpCompilation" /> wrapper factory.
    /// </summary>
    public interface ICSharpCompilationWrapperFactory
    {
        /// <summary>
        /// Creates an instance of type
        /// <see cref="ICSharpCompilationWrapper" />.
        /// </summary>
        /// <param name="destinationLocation">
        /// An instance of type <see cref="IFileInfoWrapper" />, describing
        /// where the stub assembly will be stored.
        /// This is used in naming the new stub assembly - the new stub
        /// assembly is not actually stored to disk in this method.
        /// </param>
        /// <param name="syntaxTree">
        /// An instance of <see cref="SyntaxTree" /> containing the parsed
        /// code which will be compiled.
        /// </param>
        /// <param name="metadataReferenceWrappers">
        /// A collection of instances of type
        /// <see cref="IMetadataReferenceWrapper" /> describing the
        /// dependencies for the new stub assembly.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="ICSharpCompilationWrapper" />.
        /// </returns>
        ICSharpCompilationWrapper Create(
            IFileInfoWrapper destinationLocation,
            SyntaxTree syntaxTree,
            IEnumerable<IMetadataReferenceWrapper> metadataReferenceWrappers);
    }
}