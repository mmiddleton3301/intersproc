// ----------------------------------------------------------------------------
// <copyright file="ICSharpCompilationWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.IO;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;

    /// <summary>
    /// Describes the operations of the <see cref="CSharpCompilation" />
    /// wrapper.
    /// </summary>
    public interface ICSharpCompilationWrapper
    {
        /// <summary>
        /// Performs compilation on the underlying
        /// <see cref="CSharpCompilation" /> instance.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream" /> in which to store the compiled assembly.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IEmitResultWrapper" />, containing
        /// information regarding the results of the compilation.
        /// </returns>
        IEmitResultWrapper Emit(Stream stream);
    }
}