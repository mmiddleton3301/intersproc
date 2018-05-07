// ----------------------------------------------------------------------------
// <copyright file="EmitResultWrapper.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Collections.Generic;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Microsoft.CodeAnalysis.Emit;

    /// <summary>
    /// Implements <see cref="IEmitResultWrapper" />.
    /// </summary>
    public class EmitResultWrapper : IEmitResultWrapper
    {
        private readonly EmitResult emitResult;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmitResultWrapper" />
        /// class.
        /// </summary>
        /// <param name="emitResult">
        /// An instance of <see cref="EmitResult" /> in which to wrap.
        /// </param>
        public EmitResultWrapper(EmitResult emitResult)
        {
            this.emitResult = emitResult;
        }

        /// <summary>
        /// Gets a collection of instances of type
        /// <see cref="IDiagnosticWrapper" />.
        /// </summary>
        public IEnumerable<IDiagnosticWrapper> Diagnostics
        {
            get
            {
                IEnumerable<IDiagnosticWrapper> toReturn = this.emitResult
                    .Diagnostics
                    .Select(x => new DiagnosticWrapper(x));

                return toReturn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the compilation successfully
        /// produced an executable. If false then the diagnostics should
        /// include at least one error diagnostic indicating the cause of the
        /// failure.
        /// </summary>
        public bool Success
        {
            get
            {
                return this.emitResult.Success;
            }
        }
    }
}