// ----------------------------------------------------------------------------
// <copyright file="StubGenerationException.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A custom <see cref="Exception" /> thrown when an error occurs during
    /// the generation of the DOM of the stub assembly.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1032",
        Justification = "Not gold plating the exception by adding multiple constructors that wont be used.")]
    [ExcludeFromCodeCoverage]
    public class StubGenerationException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubGenerationException" /> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public StubGenerationException(string message)
            : base(message)
        {
            // Nothing - just bubbles down to the base implementation.
        }
    }
}