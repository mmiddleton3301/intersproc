// ----------------------------------------------------------------------------
// <copyright file="IAssemblyWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Reflection;

    /// <summary>
    /// Describes the operations provided by the <see cref="Assembly" />
    /// wrapper factory.
    /// </summary>
    public interface IAssemblyWrapperFactory
    {
        /// <summary>
        /// Creates an instance of type <see cref="IAssemblyWrapper" />.
        /// </summary>
        /// <param name="assembly">
        /// An <see cref="Assembly" /> to wrap.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IAssemblyWrapper" />.
        /// </returns>
        IAssemblyWrapper Create(Assembly assembly);

        /// <summary>
        /// Gets the assembly that contains the code that is currently
        /// executing.
        /// </summary>
        /// <returns>
        /// The assembly that contains the code that is currently executing.
        /// </returns>
        IAssemblyWrapper GetExecutingAssembly();

        /// <summary>
        /// Loads the contents of an assembly file on the specified path.
        /// </summary>
        /// <param name="path">
        /// The fully qualified path of the file to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        IAssemblyWrapper LoadFile(string path);
    }
}