// ----------------------------------------------------------------------------
// <copyright file="IFileInfoWrapper.cs" company="MTCS">
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
    /// wrapper.
    /// </summary>
    public interface IFileInfoWrapper : IFileSystemInfoWrapper
    {
        /// <summary>
        /// Gets the parent directory path.
        /// </summary>
        string ParentDirectoryPath
        {
            get;
        }

        /// <summary>
        /// Creates a <see cref="StreamWriter" /> that appends text to the file
        /// represented by this instance of the <see cref="FileInfo" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="StreamWriter" />.
        /// </returns>
        Stream Create();

        /// <summary>
        /// Permanently deletes a file.
        /// </summary>
        void Delete();
    }
}