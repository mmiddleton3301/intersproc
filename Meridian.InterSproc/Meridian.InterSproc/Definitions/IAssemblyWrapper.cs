// ----------------------------------------------------------------------------
// <copyright file="IAssemblyWrapper.cs" company="MTCS">
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
    /// Describes the operations provided by the
    /// <see cref="System.Reflection.Assembly" /> wrapper.
    /// </summary>
    public interface IAssemblyWrapper
    {
        /// <summary>
        /// Gets the wrapped <see cref="System.Reflection.Assembly" />
        /// instance.
        /// </summary>
        Assembly Assembly
        {
            get;
        }

        /// <summary>
        /// Gets the display name of the assembly.
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// Gets the full path or UNC location of the loaded file that contains
        /// the manifest.
        /// </summary>
        string Location
        {
            get;
        }
    }
}