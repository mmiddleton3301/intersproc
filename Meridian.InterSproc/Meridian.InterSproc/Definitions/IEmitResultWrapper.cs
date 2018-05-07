// ----------------------------------------------------------------------------
// <copyright file="IEmitResultWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.Emit;

    /// <summary>
    /// Describes the operations provided by the <see cref="EmitResult" />
    /// wrapper.
    /// </summary>
    public interface IEmitResultWrapper
    {
        /// <summary>
        /// Gets a collection of instances of type
        /// <see cref="IDiagnosticWrapper" />.
        /// </summary>
        IEnumerable<IDiagnosticWrapper> Diagnostics
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the compilation successfully
        /// produced an executable. If false then the diagnostics should
        /// include at least one error diagnostic indicating the cause of the
        /// failure.
        /// </summary>
        bool Success
        {
            get;
        }
    }
}