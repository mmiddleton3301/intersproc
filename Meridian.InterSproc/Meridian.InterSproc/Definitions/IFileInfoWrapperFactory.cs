// ----------------------------------------------------------------------------
// <copyright file="IFileInfoWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.IO;

    /// <summary>
    /// Describes the operations provided by the <see cref="FileInfo" />
    /// wrapper factory.
    /// </summary>
    public interface IFileInfoWrapperFactory
    {
        /// <summary>
        /// Creates an instance of type <see cref="IFileInfoWrapper" />.
        /// </summary>
        /// <param name="fileName">
        /// The fully qualified name of the new file, or the relative file
        /// name. Do not end the path with the directory separator character.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IFileInfoWrapper" />.
        /// </returns>
        IFileInfoWrapper Create(string fileName);
    }
}