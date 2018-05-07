// ----------------------------------------------------------------------------
// <copyright file="MetadataReferenceWrapper.cs" company="MTCS">
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
    /// Implements <see cref="IMetadataReferenceWrapper" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MetadataReferenceWrapper : IMetadataReferenceWrapper
    {
        private readonly MetadataReference metadataReference;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="MetadataReferenceWrapper" /> class.
        /// </summary>
        /// <param name="metadataReference">
        /// An instance of <see cref="MetadataReference" />.
        /// </param>
        public MetadataReferenceWrapper(MetadataReference metadataReference)
        {
            this.metadataReference = metadataReference;
        }

        /// <summary>
        /// Gets the path or name used in error messages to identity the
        /// reference.
        /// </summary>
        public string Display
        {
            get
            {
                return this.metadataReference.Display;
            }
        }

        /// <summary>
        /// Gets the wrapped
        /// <see cref="Microsoft.CodeAnalysis.MetadataReference" /> instance.
        /// </summary>
        public MetadataReference MetadataReference
        {
            get
            {
                return this.metadataReference;
            }
        }
    }
}