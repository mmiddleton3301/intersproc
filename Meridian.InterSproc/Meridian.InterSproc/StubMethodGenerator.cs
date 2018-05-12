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
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Implements <see cref="IStubMethodGenerator" />.
    /// </summary>
    public class StubMethodGenerator : IStubMethodGenerator
    {
        private const string ConnectionVariableName = "connection";
        private const string DynamicParametersVariableName = "sprocParameters";
        private const string ReturnPlaceholderVariableName = "toReturn";

        private readonly CodeMemberField connectionStringMember;
        private readonly CodeVariableReferenceExpression connectionVariableReference;
        private readonly CodeSnippetStatement emptyLine;
        private readonly ILoggingProvider loggingProvider;
        private readonly CodePrimitiveExpression nullValue;

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

            this.connectionVariableReference =
                new CodeVariableReferenceExpression(
                    ConnectionVariableName);
            this.emptyLine = new CodeSnippetStatement(string.Empty);
            this.nullValue = new CodePrimitiveExpression(null);
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
                    x => this.CreateTryCatchDataInfrastructure(
                        x,
                        methodName,
                        contractMethodInformation,
                        paramsToAdd,
                        returnType,
                        this.BuildConditionalInvokeStatements));

            toReturn.Statements.AddRange(body);

            this.loggingProvider.Info(
                $"Method body generated, total: {body.Length} line(s). " +
                $"Added to method implementation of {methodName}.");

            return toReturn;
        }

        private void BuildConditionalInvokeStatements(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation,
            CodeParameterDeclarationExpression[] paramsToAdd,
            Type returnType)
        {
            bool returnTypeIsVoid = returnType == typeof(void);

            if (returnTypeIsVoid)
            {
                this.BuildExecuteStatement(
                    codeStatements,
                    contractMethodInformation);
            }
            else
            {
                // 1) ienumDapperReturnType is an IEnumerable<> type,
                //    regardless of whether or not the stub returns a single
                //    item, or multiple.
                Type ienumDapperReturnType = null;

                // 2) Record in a bool whether or not the stub method returns
                //    multiple, or just a single record.
                bool returnTypeIsCollection =
                    returnType.GetInterface(nameof(IEnumerable)) != null;

                // Assign ienumDapperReturnType accordingly.
                if (returnTypeIsCollection)
                {
                    ienumDapperReturnType = returnType;
                }
                else
                {
                    Type ienumerableType = typeof(IEnumerable<>);

                    // Then leave it be.
                    ienumDapperReturnType =
                        ienumerableType.MakeGenericType(returnType);
                }

                // 3) Build call to Query<> statement.
                string resultsVariableName = this.BuildQueryStatement(
                    codeStatements,
                    contractMethodInformation,
                    ienumDapperReturnType);

                // Another new line.
                codeStatements.Add(this.emptyLine);

                // 4) Now prepare the results prior to returning them from the
                //    stub, depending on whether the implementation returns
                //    a collection or not.
                CodeExpression resultsPreparer = null;
                if (returnTypeIsCollection)
                {
                    // The return type is a collection anyway, so simply assign
                    // the results from dapper to the return value placeholder.
                    // i.e.
                    // results;
                    resultsPreparer = new CodeVariableReferenceExpression(
                        resultsVariableName);
                }
                else
                {
                    // Otherwise, the intention of this method is to return
                    // a single item.
                    // results.SingleOrDefault();
                    resultsPreparer = new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression(
                            resultsVariableName),
                        nameof(Enumerable.SingleOrDefault));
                }

                // 5) Assign the prepared results in the return value variable.
                CodeAssignStatement assignReturnVariable =
                    new CodeAssignStatement(
                        new CodeVariableReferenceExpression(
                            ReturnPlaceholderVariableName),
                        resultsPreparer);

                codeStatements.Add(assignReturnVariable);
            }
        }

        // TODO: Log this method.
        private void BuildExecuteStatement(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation)
        {
            // 1) connection.Execute() ...
            CodeMethodReferenceExpression method =
                new CodeMethodReferenceExpression(
                    this.connectionVariableReference,
                    nameof(Dapper.SqlMapper.Execute));

            // 2) ... ("dbo.FullStoredProcedureName", ...
            CodePrimitiveExpression fullSprocName =
                new CodePrimitiveExpression(
                    contractMethodInformation.GetStoredProcedureFullName());

            // 3) ... , sprocParameters, ...
            CodeVariableReferenceExpression sprocParametersReference =
                new CodeVariableReferenceExpression(
                    DynamicParametersVariableName);

            // Not entirely correct, but works in the same way.
            // We're not refering to a property, but the DOM sees it as the
            // same.
            // 4) ... , CommandType.StoredProcedure);
            CodePropertyReferenceExpression storedPrcoedureEnum =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(nameof(CommandType)),
                    nameof(CommandType.StoredProcedure));

            CodeMethodInvokeExpression invokeExecuteStatement =
                new CodeMethodInvokeExpression(
                    method,
                    fullSprocName,
                    sprocParametersReference,
                    this.nullValue,
                    this.nullValue,
                    storedPrcoedureEnum);

            codeStatements.Add(
                new CodeExpressionStatement(invokeExecuteStatement));
        }

        private string BuildQueryStatement(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation,
            Type ienumDapperReturnType)
        {
            string toReturn = null;

            Type dapperReturnTypeSingle =
                ienumDapperReturnType.GenericTypeArguments.Single();

            // 1) connection.Query<T>()...
            CodeMethodReferenceExpression method =
                new CodeMethodReferenceExpression(
                    this.connectionVariableReference,
                    nameof(Dapper.SqlMapper.Query),
                    new CodeTypeReference(dapperReturnTypeSingle));

            // 2) ... ("dbo.FullStoredProcedureName", ...
            CodePrimitiveExpression fullSprocName =
                new CodePrimitiveExpression(
                    contractMethodInformation.GetStoredProcedureFullName());

            // 3) ... , sprocParameters, ...
            CodeVariableReferenceExpression sprocParametersReference =
                new CodeVariableReferenceExpression(
                    DynamicParametersVariableName);

            // Not entirely correct, but works in the same way.
            // We're not refering to a property, but the DOM sees it as the
            // same.
            // 4) ... , CommandType.StoredProcedure);
            CodePropertyReferenceExpression storedPrcoedureEnum =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(nameof(CommandType)),
                    nameof(CommandType.StoredProcedure));

            // connection.Query<ReturnType>(
            //      "sprocName",
            //      sprocParameters,
            //      null,
            //      true,
            //      null,
            //      CommandType.StoredProcedure);
            CodeMethodInvokeExpression invokeQueryStatement =
                new CodeMethodInvokeExpression(
                    method,
                    fullSprocName,
                    sprocParametersReference,
                    this.nullValue,
                    new CodePrimitiveExpression(true),
                    this.nullValue,
                    storedPrcoedureEnum);

            // IEnumerable<ReturnType> results = connection.Query<ReturnTyp...
            CodeVariableDeclarationStatement resultsVariable =
                new CodeVariableDeclarationStatement(
                    ienumDapperReturnType,
                    "results",
                    invokeQueryStatement);

            codeStatements.Add(resultsVariable);

            toReturn = resultsVariable.Name;

            return toReturn;
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
            CodeVariableDeclarationStatement returnPlaceholderVariable = null;
            if (returnType != typeof(void))
            {
                CodeExpression initialisationValue = null;
                if (returnType.IsValueType)
                {
                    initialisationValue = new CodeDefaultValueExpression(
                        new CodeTypeReference(returnType));

                    this.loggingProvider.Info(
                        $"The return type is a struct. Therefore, the " +
                        $"default value will be default({returnType.Name}).");
                }
                else
                {
                    // Class type initialises to null.
                    initialisationValue = this.nullValue;

                    this.loggingProvider.Info(
                        $"The return type is a class. Therefore, the default " +
                        $"value will be null.");
                }

                // 2) Generate return value variable...
                this.loggingProvider.Debug(
                    $"Generating return value placholder for " +
                    $"\"{methodName}\"...");

                returnPlaceholderVariable =
                    new CodeVariableDeclarationStatement(
                        returnType,
                        ReturnPlaceholderVariableName,
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
            this.loggingProvider.Debug(
                $"Invoking {nameof(bodyBuilderAction)} for method " +
                $"\"{methodName}\"...");

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

        private void CreateTryCatchDataInfrastructure(
            List<CodeStatement> codeStatements,
            string methodName,
            ContractMethodInformation contractMethodInformation,
            CodeParameterDeclarationExpression[] paramsToAdd,
            Type returnType,
            Action<List<CodeStatement>, ContractMethodInformation, CodeParameterDeclarationExpression[], Type> buildMethod)
        {
            // 1) IDbConnection connection = null;
            string idbConnectionName = typeof(IDbConnection).Name;

            this.loggingProvider.Debug(
                $"Generating the {idbConnectionName} variable for the " +
                $"\"{methodName}\" implementation...");

            CodeVariableDeclarationStatement connectionVariable =
                new CodeVariableDeclarationStatement(
                    idbConnectionName,
                    ConnectionVariableName,
                    this.nullValue);

            codeStatements.Add(connectionVariable);

            // 2) new SqlConnection(this.connectionString);
            string sqlConnectionName = typeof(SqlConnection).Name;

            this.loggingProvider.Debug(
                $"Generating the {sqlConnectionName} creation statement " +
                $"for the \"{methodName}\" implementation...");

            CodeObjectCreateExpression createSqlConnectionInstance =
                new CodeObjectCreateExpression(
                    sqlConnectionName,
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        methodName));

            // 3) Declare, but not attach
            //    DynamicParameters sprocParameters = new DynamicParameters();
            string dynamicParametersName =
                typeof(Dapper.DynamicParameters).Name;

            this.loggingProvider.Debug(
                $"Generating the {dynamicParametersName} variable " +
                $"declaration and creation statement for the " +
                $"\"{methodName}\" implementation...");

            CodeVariableDeclarationStatement dynamicParamsDeclaration =
                new CodeVariableDeclarationStatement(
                    typeof(Dapper.DynamicParameters).Name,
                    DynamicParametersVariableName,
                    new CodeObjectCreateExpression(dynamicParametersName));

            // 4) try {
            this.loggingProvider.Debug(
                $"Generating initial try statement block, including " +
                $"{sqlConnectionName} assignment and " +
                $"{dynamicParametersName} variable/creation for method " +
                $"\"{methodName}\"...");

            List<CodeStatement> tryStatements = new List<CodeStatement>
            {
                // 5) connection = new SqlConnection(this.connectionString);
                new CodeAssignStatement(
                    this.connectionVariableReference,
                    createSqlConnectionInstance),

                // Empty line.
                this.emptyLine,

                // 6) DynamicParameters sprocParameters =
                //      new DynamicParameters();
                dynamicParamsDeclaration,
            };

            // 7) Add each param in the method to the dynamic parameters.
            //    sprocParameters.Add("x", x);...
            this.loggingProvider.Debug(
                $"Generating {dynamicParametersName} for all method " +
                $"parameters, to pass into the stored procedure for " +
                $"method \"{methodName}\"...");

            IEnumerable<CodeStatement> codeMethodInvokeExpressions =
                paramsToAdd
                    .Select(x => new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression(
                            DynamicParametersVariableName),
                        nameof(Dapper.DynamicParameters.Add),
                        new CodePrimitiveExpression(x.Name.FirstCharToUpper()),
                        new CodeVariableReferenceExpression(x.Name)))
                    .Select(x => new CodeExpressionStatement(x));

            this.loggingProvider.Debug(
                $"{codeMethodInvokeExpressions.Count()} " +
                $"{dynamicParametersName}(s) generated for method " +
                $"\"{methodName}\".");

            tryStatements.AddRange(codeMethodInvokeExpressions);

            // Another empty line.
            tryStatements.Add(this.emptyLine);

            // 8) Build the meat of the method.
            this.loggingProvider.Debug(
                $"Invoking {nameof(buildMethod)} for \"{methodName}\"...");

            buildMethod(
                tryStatements,
                contractMethodInformation,
                paramsToAdd,
                returnType);

            // 9) finally {
            string disposeName = nameof(IDbConnection.Dispose);

            this.loggingProvider.Debug(
                $"Generating finally statement, containing a call to " +
                $"{disposeName}...");

            CodeStatement[] finallyStatements =
            {
                // 10) connection.Dipose();
                new CodeExpressionStatement(
                    new CodeMethodInvokeExpression(
                        this.connectionVariableReference,
                        disposeName)),
            };

            CodeTryCatchFinallyStatement disposeTryStatement =
                new CodeTryCatchFinallyStatement(
                    tryStatements.ToArray(),
                    Array.Empty<CodeCatchClause>(), // Empty
                    finallyStatements);

            codeStatements.Add(disposeTryStatement);
        }
    }
}