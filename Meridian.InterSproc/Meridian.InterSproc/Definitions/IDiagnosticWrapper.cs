// ----------------------------------------------------------------------------
// <copyright file="IDiagnosticWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Describes the operations of the
    /// <see cref="Microsoft.CodeAnalysis.Diagnostic" /> wrapper.
    /// </summary>
    public interface IDiagnosticWrapper
    {
        /// <summary>
        /// Gets the underlying
        /// <see cref="Microsoft.CodeAnalysis.Diagnostic" /> instance.
        /// </summary>
        Diagnostic Diagnostic
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this is a warning treated as an
        /// error; otherwise false.
        /// </summary>
        bool IsWarningAsError
        {
            get;
        }

        /// <summary>
        /// Gets the effective <see cref="DiagnosticSeverity" /> of the
        /// diagnostic.
        /// </summary>
        DiagnosticSeverity Severity
        {
            get;
        }
    }
}