// ----------------------------------------------------------------------------
// <copyright file="StubMethodGenerator.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Implements <see cref="IStubMethodGenerator" />.
    /// </summary>
    public class StubMethodGenerator : IStubMethodGenerator
    {
        private readonly CodeMemberField connectionStringMember;
        private readonly CodeSnippetStatement emptyLine;
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubMethodGenerator" /> class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        /// <param name="connectionStringMember">
        /// The connection string <see cref="CodeMemberField" /> for the parent
        /// stub's class.
        /// </param>
        public StubMethodGenerator(
            ILoggingProvider loggingProvider,
            CodeMemberField connectionStringMember)
        {
            this.loggingProvider = loggingProvider;
            this.connectionStringMember = connectionStringMember;

            this.emptyLine = new CodeSnippetStatement(string.Empty);
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubMethodGenerator.CreateMethod(ContractMethodInformation)" />.
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />, containing
        /// information on the method to generate.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="CodeMemberMethod" />.
        /// </returns>
        public CodeMemberMethod CreateMethod(
            ContractMethodInformation contractMethodInformation)
        {
            CodeMemberMethod toReturn = null;

            // 1) Declare method.
            string methodName = contractMethodInformation.MethodInfo.Name;

            this.loggingProvider.Debug(
                $"Generating method implementation for \"{methodName}\"...");

            toReturn = new CodeMemberMethod()
            {
                Name = methodName,
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };

            ParameterInfo[] parameterInfos = contractMethodInformation
                .MethodInfo
                .GetParameters();

            // 2) Generate and add parameters.
            CodeParameterDeclarationExpression[] paramsToAdd =
                parameterInfos
                    .Select(x => new CodeParameterDeclarationExpression(
                        x.ParameterType,
                        x.Name))
                    .ToArray();

            this.loggingProvider.Debug(
                $"Adding {parameterInfos.Length} parameter(s) to method " +
                $"implementation \"{methodName}\"...");

            toReturn.Parameters.AddRange(paramsToAdd);

            this.loggingProvider.Debug(
                $"{paramsToAdd.Length} parameter(s) added to method " +
                $"implementation.");

            // 3) Set return type.
            Type returnType = contractMethodInformation.MethodInfo.ReturnType;

            this.loggingProvider.Debug(
                $"Setting the return type of the generated \"{methodName}\" " +
                $"method to {returnType.Name}...");

            toReturn.ReturnType = new CodeTypeReference(returnType);

            // 4) Generate method body.
            this.loggingProvider.Debug(
                $"Generating method body for {methodName}...");

            CodeStatement[] body =
                this.CreateEmptyMethodWithVariablePlacehholders(
                    methodName,
                    returnType,
                    x => this.BuildMethodBody(
                        x,
                        contractMethodInformation,
                        paramsToAdd,
                        returnType));

            toReturn.Statements.AddRange(body);

            this.loggingProvider.Info(
                $"Method body generated, total: {body.Length} line(s). " +
                $"Added to method implementation of {methodName}.");

            return toReturn;
        }

        private void BuildMethodBody(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation,
            CodeParameterDeclarationExpression[] paramsToAdd,
            Type returnType)
        {
            // 1) IDbConnection connection = null;
            CodeVariableDeclarationStatement connectionVariable =
                new CodeVariableDeclarationStatement(
                    typeof(IDbConnection).Name,
                    "connection",
                    new CodePrimitiveExpression(null));

            codeStatements.Add(connectionVariable);

            // new SqlConnection(this.connectionString);
            CodeObjectCreateExpression createSqlConnectionInstance =
                new CodeObjectCreateExpression(
                    typeof(SqlConnection).Name,
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        this.connectionStringMember.Name));

            CodeVariableDeclarationStatement dynamicParamsDeclaration =
                new CodeVariableDeclarationStatement(
                    typeof(Dapper.DynamicParameters).Name,
                    "sprocParameters",
                    new CodeObjectCreateExpression(typeof(Dapper.DynamicParameters).Name));

            // try {
            List<CodeStatement> tryStatements = new List<CodeStatement>
            {
                // connection = new SqlConnection(this.connectionString);
                new CodeAssignStatement(
                    new CodeVariableReferenceExpression(connectionVariable.Name),
                    createSqlConnectionInstance),

                // Empty line.
                this.emptyLine,

                // DynamicParameters sprocParameters = new DynamicParameters();
                dynamicParamsDeclaration,
            };

            // Add each param to the dynamic parameters.
            IEnumerable<CodeStatement> codeMethodInvokeExpressions =
                paramsToAdd
                    .Select(x =>
                        new CodeMethodInvokeExpression(
                            new CodeVariableReferenceExpression(dynamicParamsDeclaration.Name),
                            nameof(Dapper.DynamicParameters.Add),
                            new CodePrimitiveExpression(x.Name.FirstCharToUpper()),
                            new CodeVariableReferenceExpression(x.Name)))
                    .Select(x => new CodeExpressionStatement(x));
            tryStatements.AddRange(codeMethodInvokeExpressions);

            // Another empty line.
            tryStatements.Add(this.emptyLine);

            bool returnTypeIsVoid = returnType == typeof(void);
            if (returnTypeIsVoid)
            {
                CodeMethodInvokeExpression invokeExecuteStatement =
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodeVariableReferenceExpression(
                                connectionVariable.Name),
                            nameof(Dapper.SqlMapper.Execute)),
                        new CodePrimitiveExpression(
                            contractMethodInformation.GetStoredProcedureFullName()),
                        new CodeVariableReferenceExpression(
                            dynamicParamsDeclaration.Name),
                        new CodePrimitiveExpression(null),
                        new CodePrimitiveExpression(null),
                        new CodePropertyReferenceExpression( // Not entirely correct, but works in the same way.
                            new CodeVariableReferenceExpression(nameof(CommandType)),
                            nameof(CommandType.StoredProcedure)));

                tryStatements.Add(new CodeExpressionStatement(invokeExecuteStatement));
            }
            else
            {
                Type ienumReturnType = null;
                bool returnTypeIsCollection =
                    returnType.GetInterface(nameof(IEnumerable)) != null;

                if (returnTypeIsCollection)
                {
                    ienumReturnType = returnType;
                }
                else
                {
                    Type ienumerableType = typeof(IEnumerable<>);

                    // Then leave it be.
                    ienumReturnType = ienumerableType.MakeGenericType(returnType);
                }

                Type sqlMapperType = typeof(Dapper.SqlMapper);

                if (!returnTypeIsVoid)
                {
                    IEnumerable<MethodInfo> genericQueryMethods = sqlMapperType
                        .GetMethods()
                        .Where(x => x.IsGenericMethod == true && x.Name == "Query");

                    MethodInfo firstQueryMethod = genericQueryMethods.First();

                    firstQueryMethod = firstQueryMethod.MakeGenericMethod(returnType);

                    // TODO: Needs a whole bunch of refactoring. Messy as heck.
                    // connection.Query<ReturnType>("sprocName", sprocParameters, null, true, null, CommandType.StoredProcedure);
                    CodeMethodInvokeExpression invokeQueryStatement =
                        new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodeVariableReferenceExpression(
                                    connectionVariable.Name),
                                firstQueryMethod.Name,
                                new CodeTypeReference(ienumReturnType.GenericTypeArguments.Single())),
                            new CodePrimitiveExpression(
                                contractMethodInformation.GetStoredProcedureFullName()),
                            new CodeVariableReferenceExpression(
                                dynamicParamsDeclaration.Name),
                            new CodePrimitiveExpression(null),
                            new CodePrimitiveExpression(true),
                            new CodePrimitiveExpression(null),
                            new CodePropertyReferenceExpression( // Not entirely correct, but works in the same way.
                                new CodeVariableReferenceExpression(nameof(CommandType)),
                                nameof(CommandType.StoredProcedure)));

                    // IEnumerable<ReturnType> results = connection.Query<ReturnType>("sprocName", sprocParameters, null, true, null, CommandType.StoredProcedure);
                    CodeVariableDeclarationStatement resultsVariable = new CodeVariableDeclarationStatement(
                        ienumReturnType,
                        "results",
                        invokeQueryStatement);

                    tryStatements.Add(resultsVariable);

                    // Another new line.
                    tryStatements.Add(this.emptyLine);

                    CodeExpression resultsPreparer = null;
                    if (returnTypeIsCollection)
                    {
                        resultsPreparer = new CodeVariableReferenceExpression(
                            resultsVariable.Name);
                    }
                    else
                    {
                        resultsPreparer = new CodeMethodInvokeExpression(
                            new CodeVariableReferenceExpression(resultsVariable.Name),
                            nameof(Enumerable.SingleOrDefault));
                    }

                    // Finally, assess the return type and use LINQ to return the
                    // appropriate value.
                    CodeAssignStatement assignReturnVariable = new CodeAssignStatement(
                        new CodeVariableReferenceExpression("toReturn"),
                        resultsPreparer);

                    tryStatements.Add(assignReturnVariable);
                }
            }

            // finally {
            CodeStatement[] finallyStatements =
            {
                // connection.Dipose();
                new CodeExpressionStatement(
                    new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression(connectionVariable.Name),
                        nameof(IDbConnection.Dispose))),
            };

            CodeTryCatchFinallyStatement disposeTryStatement =
                new CodeTryCatchFinallyStatement(
                    tryStatements.ToArray(),
                    Array.Empty<CodeCatchClause>(), // Empty
                    finallyStatements);

            codeStatements.Add(disposeTryStatement);
        }

        private CodeStatement[] CreateEmptyMethodWithVariablePlacehholders(
            string methodName,
            Type returnType,
            Action<List<CodeStatement>> bodyBuilderAction)
        {
            CodeStatement[] toReturn = null;

            List<CodeStatement> lines = new List<CodeStatement>();

            // 1) Generate return variable value placeholders, if applicable...
            //    e.g. default(struct) or null.
            this.loggingProvider.Debug(
                $"Generating method body (\"{methodName}\") return value " +
                $"placeholders for return type {returnType.Name}...");

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

            // 2) Generate return value variable...
            CodeVariableDeclarationStatement returnPlaceholderVariable = null;
            if (returnType != typeof(void))
            {
                this.loggingProvider.Debug(
                    $"Generating return value placholder for " +
                    $"\"{methodName}\"...");

                returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        nameof(toReturn),
                        initialisationValue);

                lines.Add(returnPlaceholderVariable);
                lines.Add(this.emptyLine);

                this.loggingProvider.Info(
                    $"Return value placeholder generated and added to " +
                    $"\"{methodName}\" implementation.");
            }
            else
            {
                this.loggingProvider.Info(
                    $"\"{methodName}\" does not return anything. Therefore, " +
                    $"no return value placeholder will be generated.");
            }

            // 3) Invoke the lambda argument in order to build up the bulk
            //    of the method.
            bodyBuilderAction(lines);

            // 4) If the method returns, then return the placeholder variable.
            if (returnType != typeof(void))
            {
                this.loggingProvider.Debug(
                    $"Generating return statement for method " +
                    $"implementation \"{methodName}\"...");

                lines.Add(this.emptyLine);

                CodeVariableReferenceExpression returnVarRef =
                    new CodeVariableReferenceExpression(
                        returnPlaceholderVariable.Name);

                CodeMethodReturnStatement returnStatement =
                    new CodeMethodReturnStatement(returnVarRef);

                lines.Add(returnStatement);

                this.loggingProvider.Info(
                    $"Return statement generated and appended to the " +
                    $"{nameof(CodeStatement)} list for the \"{methodName}\" " +
                    $"method implementation.");
            }
            else
            {
                this.loggingProvider.Info(
                    $"\"{methodName}\" doesn't return anything, therefore, " +
                    $"no return statement will be generated.");
            }

            toReturn = lines.ToArray();

            return toReturn;
        }
    }
}