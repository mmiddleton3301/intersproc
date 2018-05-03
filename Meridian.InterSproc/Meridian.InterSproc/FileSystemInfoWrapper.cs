// ----------------------------------------------------------------------------
// <copyright file="FileSystemInfoWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.IO;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IFileSystemInfoWrapper" />.
    /// </summary>
    public class FileSystemInfoWrapper : IFileSystemInfoWrapper
    {
        private readonly FileSystemInfo fileSystemInfo;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="FileSystemInfoWrapper" /> class.
        /// </summary>
        /// <param name="fileSystemInfo">
        /// An instance of type <see cref="fileSystemInfo" />.
        /// </param>
        public FileSystemInfoWrapper(FileSystemInfo fileSystemInfo)
        {
            this.fileSystemInfo = fileSystemInfo;
        }

        /// <summary>
        /// Gets a value indicating whether the file or directory exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                return this.fileSystemInfo.Exists;
            }
        }

        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        public string FullName
        {
            get
            {
                return this.fileSystemInfo.FullName;
            }
        }

        /// <summary>
        /// Gets the name of the file. For directories, gets the name of the
        /// last directory in the hierarchy if a hierarchy exists. Otherwise,
        /// the Name property gets the name of the directory.
        /// </summary>
        public string Name
        {
            get
            {
                return this.fileSystemInfo.Name;
            }
        }
    }
}