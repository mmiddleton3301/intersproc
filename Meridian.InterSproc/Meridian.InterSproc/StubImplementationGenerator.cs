﻿// ----------------------------------------------------------------------------
// <copyright file="StubImplementationGenerator.cs" company="MTCS">
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
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IStubImplementationGenerator" />.
    /// </summary>
    public class StubImplementationGenerator : IStubImplementationGenerator
    {
        private const string StubImplementationClassName =
            "{0}StubImplementation";

        private readonly ILoggingProvider loggingProvider;

        private CodeMemberField connectionStringMember;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubImplementationGenerator" /> class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />.
        /// </param>
        public StubImplementationGenerator(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubImplementationGenerator.CreateClass(Type, IEnumerable{ContractMethodInformation})" />.
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeTypeDeclaration" />.
        /// </returns>
        public CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            IEnumerable<ContractMethodInformation> contractMethodInformations)
        {
            CodeTypeDeclaration toReturn = null;

            string className = string.Format(
                CultureInfo.InvariantCulture,
                StubImplementationClassName,
                databaseContractType.Name);

            this.loggingProvider.Info(
                $"Implementation class name generated: {className}.");

            // Declare class.
            toReturn = new CodeTypeDeclaration()
            {
                Name = className,
                IsClass = true,
                Attributes = MemberAttributes.Public,
            };

            // Implements the database contract.
            toReturn.BaseTypes.Add(databaseContractType);

            this.loggingProvider.Debug("Generating constructor...");

            // Connection string field.
            this.connectionStringMember = new CodeMemberField(
                typeof(string),
                "connectionString");

            toReturn.Members.Add(this.connectionStringMember);

            // Constructor
            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public,
            };

            // Injectable settings provider, containing connection string.
            CodeParameterDeclarationExpression settingsProviderInject =
                new CodeParameterDeclarationExpression(
                    typeof(IStubImplementationSettingsProvider).Name,
                    "stubImplementationSettingsProvider");
            constructor.Parameters.Add(settingsProviderInject);

            // this.connectionString = stubImplementationSettingsProvider.ConnStr;
            CodePropertyReferenceExpression connStrPropertyRef =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(
                        settingsProviderInject.Name),
                    nameof(IStubImplementationSettingsProvider.ConnStr));
            constructor.Statements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        this.connectionStringMember.Name),
                    connStrPropertyRef));
            toReturn.Members.Add(constructor);

            this.loggingProvider.Info(
                $"Constructor generated and appended to " +
                $"{nameof(CodeTypeDeclaration)} instance.");

            this.loggingProvider.Debug($"Implementing interface methods...");

            CodeMemberMethod[] implementedMethods = contractMethodInformations
                .Select(x => this.ImplementContractMethod(x))
                .ToArray();

            toReturn.Members.AddRange(implementedMethods);

            this.loggingProvider.Info(
                $"{implementedMethods.Length} method(s) generated and " +
                $"appended to the {nameof(CodeTypeDeclaration)} instance.");

            return toReturn;
        }

        private static string FirstCharToUpper(string input)
        {
            string toReturn = new string(new char[] { input.First() })
                .ToUpper(CultureInfo.InvariantCulture) + input.Substring(1);

            return toReturn;
        }

        private void BuildMethodBody(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation,
            CodeParameterDeclarationExpression[] paramsToAdd,
            Type returnType)
        {
            // IDbConnection connection = null;
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
                new CodeSnippetStatement(string.Empty),

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
                            new CodePrimitiveExpression(FirstCharToUpper(x.Name)),
                            new CodeVariableReferenceExpression(x.Name)))
                    .Select(x => new CodeExpressionStatement(x));
            tryStatements.AddRange(codeMethodInvokeExpressions);

            // Another empty line.
            tryStatements.Add(new CodeSnippetStatement(string.Empty));

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

                if (returnTypeIsCollection)
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
                    tryStatements.Add(new CodeSnippetStatement(string.Empty));

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

        private CodeStatement[] GenerateMethodBody(
            Type returnType,
            Action<List<CodeStatement>> bodyBuilderAction)
        {
            CodeStatement[] toReturn = null;

            List<CodeStatement> lines = new List<CodeStatement>();

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

        private CodeMemberMethod ImplementContractMethod(
            ContractMethodInformation contractMethodInformation)
        {
            CodeMemberMethod toReturn = null;

            toReturn = new CodeMemberMethod()
            {
                Name = contractMethodInformation.MethodInfo.Name,
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };

            ParameterInfo[] parameterInfos = contractMethodInformation
                .MethodInfo
                .GetParameters();

            this.loggingProvider.Debug(
                $"Generating method implementation for {toReturn.Name}. " +
                $"Adding {parameterInfos.Length} parameter(s)...");

            CodeParameterDeclarationExpression[] paramsToAdd =
                parameterInfos
                    .Select(x => new CodeParameterDeclarationExpression(
                        x.ParameterType,
                        x.Name))
                    .ToArray();
            toReturn.Parameters.AddRange(paramsToAdd);

            this.loggingProvider.Info(
                $"{paramsToAdd.Length} parameter(s) added to method " +
                $"implementation.");

            Type returnType = contractMethodInformation.MethodInfo.ReturnType;

            toReturn.ReturnType = new CodeTypeReference(returnType);

            this.loggingProvider.Debug(
                $"Generating method body for {toReturn.Name}...");

            CodeStatement[] body = this.GenerateMethodBody(
                returnType,
                x => this.BuildMethodBody(
                    x,
                    contractMethodInformation,
                    paramsToAdd,
                    returnType));
            toReturn.Statements.AddRange(body);

            this.loggingProvider.Info(
                $"Method body generated, total: {body.Length} line(s). " +
                $"Added to method implementation of {toReturn.Name}. " +
                $"Returning.");

            return toReturn;
        }
    }
}