// ----------------------------------------------------------------------------
// <copyright file="StubGenerationException.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A custom <see cref="Exception" /> thrown when a compilation error
    /// occurs during stub assembly generation.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1032",
        Justification = "The exception was created for a single purpose, as no other appropriate exception was available. With this exception, it doesn't make sense to allow the caller to provide their own message, it's cleaner to keep it encapsulated within the class itself.")]
    public class StubGenerationException : Exception
    {
        private const string ExceptionMessageTemplate =
            "{0} error(s) were raised during the compilation of the stub " +
            "assembly.";

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubGenerationException" /> class.
        /// </summary>
        public StubGenerationException()
            : this(null)
        {
            // Nothing - just an overload. Allows easy (de)serialisation.
        }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubGenerationException" /> class.
        /// </summary>
        /// <param name="diagnostics">
        /// A collection of <see cref="Diagnostic" /> instances.
        /// </param>
        public StubGenerationException(IEnumerable<Diagnostic> diagnostics)
            : base(string.Format(
                CultureInfo.InvariantCulture,
                ExceptionMessageTemplate,
                diagnostics.Count()))
        {
            this.Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets a collection of <see cref="CompilerError" /> instances for
        /// inspection.
        /// </summary>
        public IEnumerable<Diagnostic> Diagnostics
        {
            get;
            private set;
        }
    }
}