// ----------------------------------------------------------------------------
// <copyright file="IMetadataReferenceWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Describes the operations of the <see cref="MetadataReference" />
    /// wrapper factory.
    /// </summary>
    public interface IMetadataReferenceWrapperFactory
    {
        /// <summary>
        /// Creates an instance of type
        /// <see cref="IMetadataReferenceWrapper" />.
        /// </summary>
        /// <param name="path">
        /// Path to the assembly file.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IMetadataReferenceWrapper" />.
        /// </returns>
        IMetadataReferenceWrapper Create(string path);
    }
}