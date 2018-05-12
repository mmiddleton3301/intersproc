// ----------------------------------------------------------------------------
// <copyright file="DiagnosticWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Diagnostics.CodeAnalysis;
    using Meridian.InterSproc.Definitions;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Implements <see cref="IDiagnosticWrapper" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DiagnosticWrapper : IDiagnosticWrapper
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DiagnosticWrapper" />
        /// class.
        /// </summary>
        /// <param name="diagnostic">
        /// An instance of <see cref="Microsoft.CodeAnalysis.Diagnostic" />
        /// in which to wrap.
        /// </param>
        public DiagnosticWrapper(Diagnostic diagnostic)
        {
            this.Diagnostic = diagnostic;
        }

        /// <summary>
        /// Gets the underlying
        /// <see cref="Microsoft.CodeAnalysis.Diagnostic" /> instance.
        /// </summary>
        public Diagnostic Diagnostic
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this is a warning treated as an
        /// error; otherwise false.
        /// </summary>
        public bool IsWarningAsError
        {
            get
            {
                return this.Diagnostic.IsWarningAsError;
            }
        }

        /// <summary>
        /// Gets the effective <see cref="DiagnosticSeverity" /> of the
        /// diagnostic.
        /// </summary>
        public DiagnosticSeverity Severity
        {
            get
            {
                return this.Diagnostic.Severity;
            }
        }
    }
}