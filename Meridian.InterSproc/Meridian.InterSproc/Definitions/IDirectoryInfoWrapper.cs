// ----------------------------------------------------------------------------
// <copyright file="IDirectoryInfoWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Describes the operations provided by the <see cref="DirectoryInfo" />
    /// wrapper.
    /// </summary>
    public interface IDirectoryInfoWrapper : IFileSystemInfoWrapper
    {
        /// <summary>
        /// Wraps the <see cref="DirectoryInfo.GetFiles(string)" /> method.
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
        IEnumerable<IFileInfoWrapper> GetFiles(string searchPattern);
    }
}