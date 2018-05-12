// ----------------------------------------------------------------------------
// <copyright file="DirectoryInfoWrapperFactory.cs" company="MTCS">
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
    /// Implements <see cref="IDirectoryInfoWrapperFactory" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DirectoryInfoWrapperFactory : IDirectoryInfoWrapperFactory
    {
        private readonly IFileInfoWrapperFactory fileInfoWrapperFactory;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="DirectoryInfoWrapperFactory" /> class.
        /// </summary>
        /// <param name="fileInfoWrapperFactory">
        /// An instance of type <see cref="IFileInfoWrapperFactory" />.
        /// </param>
        public DirectoryInfoWrapperFactory(
            IFileInfoWrapperFactory fileInfoWrapperFactory)
        {
            this.fileInfoWrapperFactory = fileInfoWrapperFactory;
        }

        /// <summary>
        /// Creates an instance of type <see cref="IDirectoryInfoWrapper" />.
        /// </summary>
        /// <param name="path">
        /// A string specifying the path on which to create the
        /// <see cref="DirectoryInfo" />.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IDirectoryInfoWrapper" />.
        /// </returns>
        public IDirectoryInfoWrapper Create(string path)
        {
            IDirectoryInfoWrapper toReturn = null;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            toReturn = new DirectoryInfoWrapper(
                this.fileInfoWrapperFactory,
                directoryInfo);

            return toReturn;
        }
    }
}