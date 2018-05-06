// ----------------------------------------------------------------------------
// <copyright file="FileInfoWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IFileInfoWrapper" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FileInfoWrapper : FileSystemInfoWrapper, IFileInfoWrapper
    {
        private readonly FileInfo fileInfo;

        /// <summary>
        /// Initialises a new instance of the <see cref="FileInfoWrapper" />
        /// class.
        /// </summary>
        /// <param name="fileInfo">
        /// An instance of <see cref="FileInfo" /> to wrap.
        /// </param>
        public FileInfoWrapper(
            FileInfo fileInfo)
            : base(fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        /// <summary>
        /// Gets the parent directory path.
        /// </summary>
        public string ParentDirectoryPath
        {
            get
            {
                return this.fileInfo.Directory.FullName;
            }
        }

        /// <summary>
        /// Implements <see cref="IFileInfoWrapper.Create()" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="StreamWriter" />.
        /// </returns>
        public FileStream Create()
        {
            FileStream toReturn = this.fileInfo.Create();

            return toReturn;
        }

        /// <summary>
        /// Implements <see cref="IFileInfoWrapper.Delete()" />.
        /// </summary>
        public void Delete()
        {
            this.fileInfo.Delete();
        }
    }
}