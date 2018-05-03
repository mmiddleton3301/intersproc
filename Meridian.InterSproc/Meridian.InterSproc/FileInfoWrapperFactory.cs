// ----------------------------------------------------------------------------
// <copyright file="FileInfoWrapperFactory.cs" company="MTCS">
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
    /// Implements <see cref="IFileInfoWrapperFactory" />.
    /// </summary>
    public class FileInfoWrapperFactory : IFileInfoWrapperFactory
    {
        /// <summary>
        /// Implements <see cref="IFileInfoWrapperFactory.Create(string)" />.
        /// </summary>
        /// <param name="fileName">
        /// The fully qualified name of the new file, or the relative file
        /// name. Do not end the path with the directory separator character.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IFileInfoWrapper" />.
        /// </returns>
        public IFileInfoWrapper Create(string fileName)
        {
            IFileInfoWrapper toReturn = null;

            FileInfo fileInfo = new FileInfo(fileName);
            toReturn = new FileInfoWrapper(fileInfo);

            return toReturn;
        }
    }
}