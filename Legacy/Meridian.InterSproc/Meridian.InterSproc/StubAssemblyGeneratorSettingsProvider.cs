// ----------------------------------------------------------------------------
// <copyright
//      file="StubAssemblyGeneratorSettingsProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IStubAssemblyGeneratorSettingsProvider" />. 
    /// </summary>
    public class StubAssemblyGeneratorSettingsProvider
        : IStubAssemblyGeneratorSettingsProvider
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubAssemblyGeneratorSettingsProvider" /> class. 
        /// </summary>
        /// <param name="generateAssemblyCodeFile">
        /// A value indicating whether to generate a <c>.cs</c> file in the
        /// same directory as the stub assembly file.
        /// </param>
        public StubAssemblyGeneratorSettingsProvider(
            bool generateAssemblyCodeFile)
        {
            this.GenerateAssemblyCodeFile = generateAssemblyCodeFile;
        }

        /// <summary>
        /// Gets a value indicating whether to generate a <c>.cs</c> file in
        /// the same directory as the stub assembly file.
        /// </summary>
        public bool GenerateAssemblyCodeFile
        {
            get;
            private set;
        }
    }
}
