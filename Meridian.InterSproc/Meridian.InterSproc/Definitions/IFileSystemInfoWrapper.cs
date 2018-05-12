// ----------------------------------------------------------------------------
// <copyright file="IFileSystemInfoWrapper.cs" company="MTCS">
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
    /// Describes the operations provided by the <see cref="FileSystemInfo" />
    /// wrapper.
    /// </summary>
    public interface IFileSystemInfoWrapper
    {
        /// <summary>
        /// Gets a value indicating whether the file or directory exists.
        /// </summary>
        bool Exists
        {
            get;
        }

        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// Gets the name of the file. For directories, gets the name of the
        /// last directory in the hierarchy if a hierarchy exists. Otherwise,
        /// the Name property gets the name of the directory.
        /// </summary>
        string Name
        {
            get;
        }
    }
}