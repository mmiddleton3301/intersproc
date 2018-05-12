// ----------------------------------------------------------------------------
// <copyright file="AssemblyWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IAssemblyWrapper" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AssemblyWrapper : IAssemblyWrapper
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AssemblyWrapper" />
        /// class.
        /// </summary>
        /// <param name="assembly">
        /// An instance of <see cref="System.Reflection.Assembly" />.
        /// </param>
        public AssemblyWrapper(Assembly assembly)
        {
            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the wrapped <see cref="System.Reflection.Assembly" />
        /// instance.
        /// </summary>
        public Assembly Assembly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name of the assembly.
        /// </summary>
        public string FullName
        {
            get
            {
                string toReturn = this.Assembly.FullName;

                return toReturn;
            }
        }

        /// <summary>
        /// Gets the full path or UNC location of the loaded file that contains
        /// the manifest.
        /// </summary>
        public string Location
        {
            get
            {
                string toReturn = this.Assembly.Location;

                return toReturn;
            }
        }
    }
}