// ----------------------------------------------------------------------------
// <copyright file="CSharpCompilationWrapper.cs" company="MTCS">
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
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;

    /// <summary>
    /// Implements <see cref="ICSharpCompilationWrapper" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CSharpCompilationWrapper : ICSharpCompilationWrapper
    {
        private readonly CSharpCompilation cSharpCompilation;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="CSharpCompilationWrapper" /> class.
        /// </summary>
        /// <param name="cSharpCompilation">
        /// An instance of <see cref="CSharpCompilation" /> in which to wrap.
        /// </param>
        public CSharpCompilationWrapper(CSharpCompilation cSharpCompilation)
        {
            this.cSharpCompilation = cSharpCompilation;
        }

        /// <summary>
        /// Implements <see cref="ICSharpCompilationWrapper.Emit(Stream)" />.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream" /> in which to store the compiled assembly.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IEmitResultWrapper" />, containing
        /// information regarding the results of the compilation.
        /// </returns>
        public IEmitResultWrapper Emit(Stream stream)
        {
            IEmitResultWrapper toReturn = null;

            EmitResult emitResult = this.cSharpCompilation.Emit(stream);

            toReturn = new EmitResultWrapper(emitResult);

            return toReturn;
        }
    }
}