// ----------------------------------------------------------------------------
// <copyright file="SprocStubFactoryCreateOptions.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// An options class, used to provide optional settings to
    /// <see cref="SprocStubFactory.Create{DatabaseContractType}(string, SprocStubFactoryCreateOptions)" />.
    /// </summary>
    public class SprocStubFactoryCreateOptions
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="SprocStubFactoryCreateOptions" /> class.
        /// </summary>
        public SprocStubFactoryCreateOptions()
        {
            // Defaults for the options should be specified here.
            this.GenerateAssemblyCodeFile = false;
            this.UseCachedStubAssemblies = true;
            this.LoggingProvider = new LoggingProvider();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to generate a <c>.cs</c>
        /// file in the same directory as the stub assembly file.
        /// </summary>
        public bool GenerateAssemblyCodeFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use cached stub
        /// assemblies (set to true when troubleshooting stub generation
        /// issues).
        /// </summary>
        public bool UseCachedStubAssemblies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an instance of <see cref="ILoggingProvider" />, to
        /// provide logging support to <c>InterSproc</c>.
        /// </summary>
        public ILoggingProvider LoggingProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Overrides <see cref="object.ToString()" />.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string toReturn = null;

            Type optionsType = this.GetType();

            PropertyInfo[] optionsProperties = optionsType.GetProperties();

            string[] allOptionsAndValuesArr = optionsProperties
                .Select(x => $"{x.Name} = {x.GetValue(this).ToString()}")
                .ToArray();

            toReturn = string.Join(", ", allOptionsAndValuesArr);

            toReturn = $"{nameof(SprocStubFactoryCreateOptions)} ({toReturn})";

            return toReturn;
        }
    }
}