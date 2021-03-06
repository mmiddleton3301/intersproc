﻿// ----------------------------------------------------------------------------
// <copyright file="IStubAssemblyGeneratorSettingsProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
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