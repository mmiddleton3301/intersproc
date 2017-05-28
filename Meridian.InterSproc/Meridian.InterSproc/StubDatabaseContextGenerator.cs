// ----------------------------------------------------------------------------
// <copyright
//      file="StubDatabaseContextGenerator.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IStubDatabaseContextGenerator" />. 
    /// </summary>
    public class StubDatabaseContextGenerator : IStubDatabaseContextGenerator
    {
        /// <summary>
        /// The format of the name of the custom <see cref="DataContext" /> 
        /// class, {0} being the original database contract name.
        /// </summary>
        private const string StubImplementationDataContextName = 
            "{0}DataContext";

        /// <summary>
        /// An instance of <see cref="IStubCommonGenerator" />. 
        /// </summary>
        private readonly IStubCommonGenerator stubCommonGenerator;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubDatabaseContextGenerator" /> class. 
        /// </summary>
        /// <param name="stubCommonGenerator">
        /// An instance of <see cref="IStubCommonGenerator" />. 
        /// </param>
        public StubDatabaseContextGenerator(
            IStubCommonGenerator stubCommonGenerator)
        {
            this.stubCommonGenerator = stubCommonGenerator;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubDatabaseContextGenerator.CreateClass(Type, ContractMethodInformation[])" />. 
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances,
        /// also describing the input <paramref name="databaseContractType" />.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeTypeDeclaration" />. 
        /// </returns>
        public CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            ContractMethodInformation[] contractMethodInformations)
        {
            CodeTypeDeclaration toReturn = null;

            // First, come up with the class name.
            string className = string.Format(
                StubImplementationDataContextName,
                databaseContractType.Name);

            // Declare class.
            toReturn = new CodeTypeDeclaration()
            {
                Name = className,
                IsClass = true,
                TypeAttributes = TypeAttributes.NotPublic
            };

            // Inherits from DataContext.
            Type dataContextType = typeof(DataContext);
            toReturn.BaseTypes.Add(dataContextType);

            // Constructor.
            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.FamilyAndAssembly
            };
            constructor.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(string), "connStr"));
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("connStr"));

            constructor.Statements.Add(new CodeCommentStatement("Just bubbles down."));

            toReturn.Members.Add(constructor);

            // Add the actual data access methods.
            CodeTypeMember[] dataContextMethods = contractMethodInformations
                .Select(this.CreateDataContextMethod)
                .ToArray();
            toReturn.Members.AddRange(dataContextMethods);

            return toReturn;
        }

        /// <summary>
        /// Builds the <see cref="DataContext" /> method's bodies, by adding
        /// <see cref="CodeStatement" /> instances to
        /// <paramref name="codeStatements" />. 
        /// </summary>
        /// <param name="codeStatements">
        /// An instance of <see cref="List{CodeStatement}" /> - method body
        /// contents are to be added to this collection.
        /// </param>
        /// <param name="methodParams">
        /// An array of <see cref="CodeParameterDeclarationExpression" />
        /// instances, descibing the method's own parameters.
        /// </param>
        /// <param name="dataContextMethodReturnType">
        /// The return type of the corresponding <see cref="DataContext" />
        /// method, which the implementation will invoke.
        /// </param>
        private void BuildMethodBody(
            List<CodeStatement> codeStatements,
            CodeParameterDeclarationExpression[] methodParams,
            Type dataContextMethodReturnType)
        {
            Type methodInfoType = typeof(MethodInfo);

            // MethodInfo.GetCurrentMethod()
            CodeMethodInvokeExpression getCurrentMethodRef =
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(methodInfoType),
                    "GetCurrentMethod");

            // (MethodInfo)MethodInfo.GetCurrentMethod()
            CodeCastExpression castExpr = new CodeCastExpression(
                methodInfoType,
                getCurrentMethodRef);

            // MethodInfo mi = (MethodInfo)MethodInfo.GetCurrentMethod()
            CodeVariableDeclarationStatement methodInfo =
                new CodeVariableDeclarationStatement(
                    methodInfoType,
                    "mi",
                    castExpr);

            codeStatements.Add(methodInfo);

            // New line.
            CodeSnippetStatement newLine =
                new CodeSnippetStatement(string.Empty);

            codeStatements.Add(newLine);

            // object[] { param1, param2, etc }
            CodeTypeReference objectType =
                new CodeTypeReference(typeof(object));

            CodeVariableReferenceExpression[] paramRefs =
                methodParams
                    .Select(y => new CodeVariableReferenceExpression(y.Name))
                    .ToArray();

            CodeArrayCreateExpression objectArrayCreate =
                new CodeArrayCreateExpression(
                    objectType,
                    paramRefs);

            // object[] methodParams = object[] { param1, param2, etc }
            CodeVariableDeclarationStatement objectArrayDecl =
                new CodeVariableDeclarationStatement(
                    typeof(object[]),
                    "methodParams",
                    objectArrayCreate);

            codeStatements.Add(objectArrayDecl);

            // Another new line.
            codeStatements.Add(newLine);

            // this.ExecuteMethodCall(this, mi, methodParams)
            CodeThisReferenceExpression thisRef =
                new CodeThisReferenceExpression();

            CodeMethodReferenceExpression executeMethodCallRef =
                new CodeMethodReferenceExpression(
                    thisRef,
                    "ExecuteMethodCall");

            CodeMethodInvokeExpression executeMethodCall =
                new CodeMethodInvokeExpression(
                    executeMethodCallRef,
                    thisRef,
                    new CodeVariableReferenceExpression(methodInfo.Name),
                    new CodeVariableReferenceExpression(objectArrayDecl.Name));

            // IExecuteResult result =
            //      this.ExecuteMethodCall(this, mi, methodParams)
            CodeVariableDeclarationStatement resultExecution =
                new CodeVariableDeclarationStatement(
                    typeof(IExecuteResult),
                    "result",
                    executeMethodCall);

            codeStatements.Add(resultExecution);

            codeStatements.Add(newLine);

            // result.ReturnValue
            CodePropertyReferenceExpression returnValuePropRef =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(resultExecution.Name),
                    "ReturnValue");

            // (ReturnType)result.ReturnValue
            CodeCastExpression resultCastExpr =
                new CodeCastExpression(
                    dataContextMethodReturnType,
                    returnValuePropRef);

            // toReturn = (ReturnType)result.ReturnValue
            CodeAssignStatement returnVarAssign =
                new CodeAssignStatement(
                    new CodeVariableReferenceExpression("toReturn"),
                    resultCastExpr);

            codeStatements.Add(returnVarAssign);
        }

        /// <summary>
        /// Converts the input
        /// <see cref="ContractMethodInformation.MethodInfo" />'s return type
        /// (as is declared on the data contract type itself), into the
        /// corresponding/required <see cref="ISingleResult{T}" /> generic
        /// type.
        /// If the method returns nothing (i.e. <see cref="void" />), then
        /// the return type will be <see cref="int" />. 
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />. 
        /// </param>
        /// <returns>
        /// An instnace of <see cref="Type" />, describing the return type for
        /// the <see cref="DataContext" /> method. 
        /// </returns>
        private Type ConvertReturnTypeToDataContextReturnType(
            ContractMethodInformation contractMethodInformation)
        {
            Type toReturn = null;

            if (contractMethodInformation.MethodInfo.ReturnType != typeof(void))
            {
                Type innerType =
                    contractMethodInformation.MethodInfo.ReturnType;

                if (innerType.BaseType == typeof(Array))
                {
                    innerType = innerType.GetElementType();
                }

                toReturn = typeof(ISingleResult<>);
                toReturn = toReturn
                    .MakeGenericType(innerType);
            }
            else
            {
                toReturn = typeof(int);
            }

            return toReturn;
        }

        /// <summary>
        /// Creates the <see cref="DataContext" /> method with information
        /// derived from <paramref name="contractMethodInformation" />. 
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeMemberMethod" />. 
        /// </returns>
        private CodeMemberMethod CreateDataContextMethod(
            ContractMethodInformation contractMethodInformation)
        {
            CodeMemberMethod toReturn = new CodeMemberMethod()
            {
                // Internal method...
                Attributes = MemberAttributes.FamilyAndAssembly
            };

            CodeAttributeDeclaration methodAttr =
                this.CreateMethodAttribute(contractMethodInformation);
            toReturn.CustomAttributes.Add(methodAttr);

            toReturn.Name = contractMethodInformation.MethodInfo.Name;

            ParameterInfo[] methodParamInfos = contractMethodInformation
                .MethodInfo
                .GetParameters();

            CodeParameterDeclarationExpression[] methodParams =
                methodParamInfos
                    .Select(x => new CodeParameterDeclarationExpression(
                        x.ParameterType,
                        x.Name))
                    .ToArray();

            toReturn.Parameters.AddRange(methodParams);

            Type dataContextMethodReturnType =
                this.ConvertReturnTypeToDataContextReturnType(
                    contractMethodInformation);

            CodeStatement[] body =
                this.stubCommonGenerator.GenerateMethodBody(
                    dataContextMethodReturnType,
                    x => this.BuildMethodBody(
                        x,
                        methodParams,
                        dataContextMethodReturnType));

            toReturn.Statements.AddRange(body);

            toReturn.ReturnType =
                new CodeTypeReference(dataContextMethodReturnType);

            return toReturn;
        }

        /// <summary>
        /// Creates the <see cref="DataContext" /> method's
        /// <see cref="FunctionAttribute" /> declaration.  
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeAttributeDeclaration" />.
        /// </returns>
        private CodeAttributeDeclaration CreateMethodAttribute(
            ContractMethodInformation contractMethodInformation)
        {
            CodeAttributeDeclaration toReturn = null;

            CodeTypeReference functionAttrType =
                new CodeTypeReference(typeof(FunctionAttribute));
            CodeAttributeArgument isComposibleArg =
                new CodeAttributeArgument(
                    "IsComposable",
                    new CodePrimitiveExpression(false));
            CodeAttributeArgument nameArgument =
                new CodeAttributeArgument(
                    "Name",
                    new CodePrimitiveExpression(
                        $"{contractMethodInformation.Schema}." +
                        $"{contractMethodInformation.Prefix}{contractMethodInformation.Name}"));

            toReturn = new CodeAttributeDeclaration(
                functionAttrType,
                isComposibleArg,
                nameArgument);

            return toReturn;
        }
    }
}