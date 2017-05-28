// ----------------------------------------------------------------------------
// <copyright file="StubCommonGenerator.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IStubCommonGenerator" />. 
    /// </summary>
    public class StubCommonGenerator : IStubCommonGenerator
    {
        /// <summary>
        /// Implements
        /// <see cref="IStubCommonGenerator.GenerateMethodBody(Type, Action{List{CodeStatement}})" />. 
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
        public CodeStatement[] GenerateMethodBody(
            Type returnType,
            Action<List<CodeStatement>> bodyBuilderAction)
        {
            CodeStatement[] toReturn = null;

            List<CodeStatement> lines =
                new List<CodeStatement>();

            // Generate return type placeholder variables.
            CodeExpression initialisationValue = null;
            if (returnType.IsValueType)
            {
                initialisationValue = new CodeDefaultValueExpression(
                    new CodeTypeReference(returnType));
            }
            else
            {
                // Class type initialises to null.
                initialisationValue = new CodePrimitiveExpression(null);
            }

            CodeSnippetStatement emptyLine =
                    new CodeSnippetStatement(string.Empty);

            CodeVariableDeclarationStatement returnPlaceholderVariable = null;

            if (returnType != typeof(void))
            {
                returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        "toReturn",
                        initialisationValue);

                lines.Add(returnPlaceholderVariable);
                lines.Add(emptyLine);
            }

            bodyBuilderAction(lines);

            if (returnType != typeof(void))
            {
                lines.Add(emptyLine);

                CodeVariableReferenceExpression returnVarRef =
                    new CodeVariableReferenceExpression(
                        returnPlaceholderVariable.Name);

                CodeMethodReturnStatement returnStatement =
                    new CodeMethodReturnStatement(returnVarRef);

                lines.Add(returnStatement);
            }

            toReturn = lines.ToArray();

            return toReturn;
        }
    }
}