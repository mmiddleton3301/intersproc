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
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubCommonGenerator" /> class. 
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </param>
        public StubCommonGenerator(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

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

            this.loggingProvider.Debug(
                $"Generating method body return value placeholders for " +
                $"return type {returnType.Name}...");

            // Generate return type placeholder variables.
            CodeExpression initialisationValue = null;
            if (returnType.IsValueType)
            {
                initialisationValue = new CodeDefaultValueExpression(
                    new CodeTypeReference(returnType));

                this.loggingProvider.Info(
                    $"The return type is a struct. Therefore, the default " +
                    $"value will be default({returnType.Name}).");
            }
            else
            {
                // Class type initialises to null.
                initialisationValue = new CodePrimitiveExpression(null);

                this.loggingProvider.Info(
                    $"The return type is a class. Therefore, the default " +
                    $"value will be null.");
            }

            CodeSnippetStatement emptyLine =
                    new CodeSnippetStatement(string.Empty);

            CodeVariableDeclarationStatement returnPlaceholderVariable = null;

            if (returnType != typeof(void))
            {
                this.loggingProvider.Debug(
                    $"Generating return value placholder...");

                returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        nameof(toReturn),
                        initialisationValue);

                lines.Add(returnPlaceholderVariable);
                lines.Add(emptyLine);

                this.loggingProvider.Info(
                    $"Return value placeholder generated and added to " +
                    $"{nameof(CodeStatement)}s.");
            }
            else
            {
                this.loggingProvider.Info(
                    $"This method does not return anything. Therefore, no " +
                    $"return value placeholder will be generated.");
            }

            bodyBuilderAction(lines);

            if (returnType != typeof(void))
            {
                this.loggingProvider.Debug(
                    $"Generating return statement for method...");

                lines.Add(emptyLine);

                CodeVariableReferenceExpression returnVarRef =
                    new CodeVariableReferenceExpression(
                        returnPlaceholderVariable.Name);

                CodeMethodReturnStatement returnStatement =
                    new CodeMethodReturnStatement(returnVarRef);

                lines.Add(returnStatement);

                this.loggingProvider.Info(
                    $"Return statement generated and appended to the " +
                    $"{nameof(CodeStatement)} list.");
            }
            else
            {
                this.loggingProvider.Info(
                    $"This method doesn't return anything, therefore, no " +
                    $"return statement will be generated.");
            }

            toReturn = lines.ToArray();

            return toReturn;
        }
    }
}