// ----------------------------------------------------------------------------
// <copyright file="StubDependencyException.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// A custom <see cref="Exception" /> thrown when a preparing a stub
    /// assembly for compilation, and more than one required dependencies are
    /// missing.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1032",
        Justification = "The exception was created for a single purpose, as no other appropriate exception was available. With this exception, it doesn't make sense to allow the caller to provide their own message, it's cleaner to keep it encapsulated within the class itself.")]
    [ExcludeFromCodeCoverage]
    public class StubDependencyException : Exception
    {
        private const string ExceptionMessageTemplate =
            "{0} trusted dependencies were missing prior to compilation. " +
            "Please check the debug logs, in particular the list returned " +
            "by querying for the TRUSTED_PLATFORM_ASSEMBLIES.";

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubDependencyException" /> class.
        /// </summary>
        public StubDependencyException()
            : this(null)
        {
            // Nothing - just an overload. Allows easy (de)serialisation.
        }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubDependencyException" /> class.
        /// </summary>
        /// <param name="missingDependencies">
        /// A list of the missing dependencies, required prior to compilation.
        /// </param>
        public StubDependencyException(IEnumerable<string> missingDependencies)
            : base(string.Format(
                CultureInfo.InvariantCulture,
                ExceptionMessageTemplate,
                missingDependencies.Count()))
        {
            this.MissingDependencies = missingDependencies;
        }

        /// <summary>
        /// Gets a list of the missing dependencies, required prior to
        /// compilation.
        /// </summary>
        public IEnumerable<string> MissingDependencies
        {
            get;
            private set;
        }
    }
}