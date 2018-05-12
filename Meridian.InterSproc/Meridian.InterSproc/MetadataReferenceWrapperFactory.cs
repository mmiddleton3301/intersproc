// ----------------------------------------------------------------------------
// <copyright file="MetadataReferenceWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Diagnostics.CodeAnalysis;
    using Meridian.InterSproc.Definitions;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Implements <see cref="IMetadataReferenceWrapperFactory" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MetadataReferenceWrapperFactory
        : IMetadataReferenceWrapperFactory
    {
        /// <summary>
        /// Implements
        /// <see cref="IMetadataReferenceWrapperFactory.Create(string)" />.
        /// </summary>
        /// <param name="path">
        /// Path to the assembly file.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IMetadataReferenceWrapper" />.
        /// </returns>
        public IMetadataReferenceWrapper Create(string path)
        {
            IMetadataReferenceWrapper toReturn = null;

            MetadataReference metadataReference =
                MetadataReference.CreateFromFile(path);

            toReturn = new MetadataReferenceWrapper(metadataReference);

            return toReturn;
        }
    }
}