// ----------------------------------------------------------------------------
// <copyright file="IDirectoryInfoWrapperFactory.cs" company="MTCS">
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
    /// Describes the operations provided by the <see cref="DirectoryInfo" />
    /// wrapper factory.
    /// </summary>
    public interface IDirectoryInfoWrapperFactory
    {
        /// <summary>
        /// Creates an instance of type <see cref="IDirectoryInfoWrapper" />.
        /// </summary>
        /// <param name="path">
        /// A string specifying the path on which to create the
        /// <see cref="DirectoryInfo" />.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IDirectoryInfoWrapperFactory" />.
        /// </returns>
        IDirectoryInfoWrapper Create(string path);
    }
}