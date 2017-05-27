// ----------------------------------------------------------------------------
// <copyright file="IStubCommonGenerator.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the operations of the stub common generator.
    /// </summary>
    public interface IStubCommonGenerator
    {
        /// <summary>
        /// Helper method used in generating the body of a method, including
        /// return variable placeholder and return statement.
        /// </summary>
        /// <param name="returnType">
        /// The return <see cref="Type" /> of the host method.
        /// </param>
        /// <param name="bodyBuilderAction">
        /// An instance of <see cref="Action{List{CodeStatement}}" />, used
        /// in building up the main content of the body.
        /// </param>
        /// <returns>
        /// An array of <see cref="CodeStatement" /> instances, including
        /// return variable placeholder and return statement.
        /// </returns>
        CodeStatement[] GenerateMethodBody(
            Type returnType,
            Action<List<CodeStatement>> bodyBuilderAction);
    }
}