// ----------------------------------------------------------------------------
// <copyright
//      file="StubGenerationException.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom.Compiler;

    /// <summary>
    /// A custom <see cref="Exception" /> thrown when a compilation error
    /// occurs during stub assembly generation.
    /// </summary>
    public class StubGenerationException : Exception
    {
        /// <summary>
        /// The format of the error message thrown by the
        /// <see cref="StubGenerationException" />. 
        /// </summary>
        private const string ExceptionMessageTemplate = 
            "{0} error(s) were raised during the compilation of the stub " +
            "assembly.";

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubGenerationException" /> class. 
        /// </summary>
        /// <param name="compilerErrors">
        /// An array of <see cref="CompilerError" /> instances. 
        /// </param>
        public StubGenerationException(CompilerError[] compilerErrors)
            : base(string.Format(
                ExceptionMessageTemplate,
                compilerErrors.Length))
        {
            this.CompilerErrors = compilerErrors;
        }

        /// <summary>
        /// Gets the array of <see cref="CompilerError" /> instances for
        /// inspection.
        /// </summary>
        public CompilerError[] CompilerErrors
        {
            get;
            private set;
        }
    }
}