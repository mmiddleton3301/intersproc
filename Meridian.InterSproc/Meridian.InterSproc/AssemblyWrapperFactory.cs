// ----------------------------------------------------------------------------
// <copyright file="AssemblyWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IAssemblyWrapperFactory" />.
    /// </summary>
    public class AssemblyWrapperFactory : IAssemblyWrapperFactory
    {
        /// <summary>
        /// Implements <see cref="IAssemblyWrapperFactory.Create(Assembly)" />.
        /// </summary>
        /// <param name="assembly">
        /// An <see cref="Assembly" /> to wrap.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IAssemblyWrapper" />.
        /// </returns>
        public IAssemblyWrapper Create(Assembly assembly)
        {
            IAssemblyWrapper toReturn = new AssemblyWrapper(assembly);

            return toReturn;
        }

        /// <summary>
        /// Implements
        /// <see cref="IAssemblyWrapperFactory.GetExecutingAssembly()" />.
        /// </summary>
        /// <returns>
        /// The assembly that contains the code that is currently executing.
        /// </returns>
        public IAssemblyWrapper GetExecutingAssembly()
        {
            IAssemblyWrapper toReturn = null;

            Assembly assembly = Assembly.GetExecutingAssembly();

            toReturn = new AssemblyWrapper(assembly);

            return toReturn;
        }

        /// <summary>
        /// Implements <see cref="IAssemblyWrapperFactory.LoadFile(string)" />.
        /// </summary>
        /// <param name="path">
        /// The fully qualified path of the file to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        public IAssemblyWrapper LoadFile(string path)
        {
            IAssemblyWrapper toReturn = null;

            Assembly assembly = Assembly.LoadFile(path);

            toReturn = new AssemblyWrapper(assembly);

            return toReturn;
        }
    }
}