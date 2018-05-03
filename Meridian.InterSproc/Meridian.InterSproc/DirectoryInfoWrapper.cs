// ----------------------------------------------------------------------------
// <copyright file="DirectoryInfoWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IDirectoryInfoWrapper" />.
    /// </summary>
    public class DirectoryInfoWrapper
        : FileSystemInfoWrapper, IDirectoryInfoWrapper
    {
        private readonly IFileInfoWrapperFactory fileInfoWrapperFactory;
        private readonly DirectoryInfo directoryInfo;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="DirectoryInfoWrapper" /> class.
        /// </summary>
        /// <param name="fileInfoWrapperFactory">
        /// An instance of type <see cref="IFileInfoWrapperFactory" />.
        /// </param>
        /// <param name="directoryInfo">
        /// An instance of <see cref="DirectoryInfo" /> to wrap.
        /// </param>
        public DirectoryInfoWrapper(
            IFileInfoWrapperFactory fileInfoWrapperFactory,
            DirectoryInfo directoryInfo)
            : base(directoryInfo)
        {
            this.fileInfoWrapperFactory = fileInfoWrapperFactory;
            this.directoryInfo = directoryInfo;
        }

        /// <summary>
        /// Implements <see cref="IDirectoryInfoWrapper.GetFiles(string)" />.
        /// </summary>
        /// <param name="searchPattern">
        /// The search string to match against the names of files. This
        /// parameter can contain a combination of valid literal path and
        /// wildcard (* and ?) characters, but it doesn't support regular
        /// expressions. The default pattern is "*", which returns all files.
        /// </param>
        /// <returns>
        /// A collection of instances of type <see cref="IFileInfoWrapper" />.
        /// </returns>
        public IEnumerable<IFileInfoWrapper> GetFiles(string searchPattern)
        {
            IEnumerable<IFileInfoWrapper> toReturn = null;

            FileInfo[] getFilesResult =
                this.directoryInfo.GetFiles(searchPattern);

            toReturn = getFilesResult
                .Select(x => this.fileInfoWrapperFactory.Create(x.FullName));

            return toReturn;
        }
    }
}