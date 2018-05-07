// ----------------------------------------------------------------------------
// <copyright file="IMetadataReferenceWrapper.cs" company="MTCS">
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
    /// wrapper.
    /// </summary>
    public interface IMetadataReferenceWrapper
    {
        /// <summary>
        /// Gets the path or name used in error messages to identity the
        /// reference.
        /// </summary>
        string Display
        {
            get;
        }

        /// <summary>
        /// Gets the wrapped
        /// <see cref="Microsoft.CodeAnalysis.MetadataReference" /> instance.
        /// </summary>
        MetadataReference MetadataReference
        {
            get;
        }
    }
}