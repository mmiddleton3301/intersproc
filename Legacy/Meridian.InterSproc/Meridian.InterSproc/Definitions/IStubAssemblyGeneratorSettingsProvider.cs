// ----------------------------------------------------------------------------
// <copyright
//      file="IStubAssemblyGeneratorSettingsProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    /// <summary>
    /// Describes the operations of the stub assembly generator settings
    /// provider.
    /// </summary>
    public interface IStubAssemblyGeneratorSettingsProvider
    {
        /// <summary>
        /// Gets a value indicating whether to generate a <c>.cs</c> file in
        /// the same directory as the stub assembly file.
        /// </summary>
        bool GenerateAssemblyCodeFile
        {
            get;
        }
    }
}