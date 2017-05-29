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
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// An instance of <see cref="IStubCommonGenerator" />. 
        /// </summary>
        private readonly IStubCommonGenerator stubCommonGenerator;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubDatabaseContextGenerator" /> class. 
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </param>
        /// <param name="stubCommonGenerator">
        /// An instance of <see cref="IStubCommonGenerator" />. 
        /// </param>
        public StubDatabaseContextGenerator(
            ILoggingProvider loggingProvider,
            IStubCommonGenerator stubCommonGenerator)
        {
            this.loggingProvider = loggingProvider;
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

            this.loggingProvider.Debug(
                $"Constructing {nameof(DataContext)} class named " +
                $"\"{className}\".");

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

            this.loggingProvider.Debug(
                $"Adding constructor to class \"{className}\"...");

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

            this.loggingProvider.Debug(
                $"Constructor generated and added. Generating " +
                $"{contractMethodInformations.Length} data access " +
                $"method(s)...");

            // Add the actual data access methods.
            CodeTypeMember[] dataContextMethods = contractMethodInformations
                .Select(this.CreateDataContextMethod)
                .ToArray();
            toReturn.Members.AddRange(dataContextMethods);

            this.loggingProvider.Info(
                $"{dataContextMethods.Length} data access method(s) " +
                $"generated and appended to the class. Returning.");

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

            this.loggingProvider.Debug(
                $"Generating MethodInfo line...");

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

            this.loggingProvider.Info("MethodInfo line generated.");

            // New line.
            CodeSnippetStatement newLine =
                new CodeSnippetStatement(string.Empty);

            codeStatements.Add(newLine);

            this.loggingProvider.Debug("Generating object array line...");

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

            this.loggingProvider.Info("Generated object array line.");

            // Another new line.
            codeStatements.Add(newLine);

            this.loggingProvider.Debug("Generating ExecuteMethodCall line...");

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

            this.loggingProvider.Info("Generated ExecuteMethodCall line.");

            codeStatements.Add(newLine);

            this.loggingProvider.Debug("Generting unboxing line...");

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

            this.loggingProvider.Info("Unboxing line generated.");

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

            Type rawReturnType =
                contractMethodInformation.MethodInfo.ReturnType;

            this.loggingProvider.Debug(
                $"Converting {nameof(DataContext)} return type based on the " +
                $"{nameof(ContractMethodInformation)}." +
                $"{nameof(contractMethodInformation.MethodInfo)} return " +
                $"type provided: {rawReturnType.Name}...");

            if (rawReturnType != typeof(void))
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

                this.loggingProvider.Info(
                    $"Return type composed: {toReturn.Name}.");
            }
            else
            {
                toReturn = typeof(int);

                this.loggingProvider.Info(
                    $"Return type is {typeof(void).Name}, therefore the " +
                    $"return type will simply be {typeof(int).Name}.");
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

            this.loggingProvider.Debug(
                $"About to generate {nameof(FunctionAttribute)} for new " +
                $"data access method ({contractMethodInformation})...");

            CodeAttributeDeclaration methodAttr =
                this.CreateMethodAttribute(contractMethodInformation);
            toReturn.CustomAttributes.Add(methodAttr);

            this.loggingProvider.Debug(
                $"Data access method generated and appended to " +
                $"{nameof(CodeAttributeDeclaration)}.");

            toReturn.Name = contractMethodInformation.MethodInfo.Name;

            this.loggingProvider.Debug(
                $"Method name will be {toReturn.Name}. Setting up " +
                $"parameters based on {nameof(contractMethodInformation)}." +
                $"{contractMethodInformation.MethodInfo}...");

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

            this.loggingProvider.Debug(
                $"{methodParams.Length} parameter(s) set up and attached. " +
                $"Generating the return type for the method...");

            Type dataContextMethodReturnType =
                this.ConvertReturnTypeToDataContextReturnType(
                    contractMethodInformation);

            toReturn.ReturnType =
                new CodeTypeReference(dataContextMethodReturnType);

            this.loggingProvider.Info(
                $"Return type for method will be: " +
                $"{dataContextMethodReturnType.Name}. Generating body of " +
                $"method...");

            CodeStatement[] body =
                this.stubCommonGenerator.GenerateMethodBody(
                    dataContextMethodReturnType,
                    x => this.BuildMethodBody(
                        x,
                        methodParams,
                        dataContextMethodReturnType));

            toReturn.Statements.AddRange(body);

            this.loggingProvider.Info(
                $"Body generation complete, {body.Length} " +
                $"{nameof(CodeStatement)} instance(s) in total - attached " +
                $"to method. Returning.");

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

            Type functionTypeAttr = typeof(FunctionAttribute);

            CodeTypeReference functionAttrType =
                new CodeTypeReference(functionTypeAttr);

            string isComposobleArgName = "IsComposable";
            bool isComposobleArgValue = false;

            string nameArgName = "Name";
            string nameArgValue =
                $"{contractMethodInformation.Schema}." +
                $"{contractMethodInformation.Prefix}" +
                $"{contractMethodInformation.Name}";
            
            this.loggingProvider.Debug(
                $"Creating {functionTypeAttr.Name} for new method, with " +
                $"\"{isComposobleArgName}\" = \"{isComposobleArgValue}\" " +
                $"and \"{nameArgName}\" = \"{nameArgValue}\"...");

            CodeAttributeArgument isComposibleArg = new CodeAttributeArgument(
                isComposobleArgName,
                new CodePrimitiveExpression(isComposobleArgValue));
            CodeAttributeArgument nameArgument = new CodeAttributeArgument(
                nameArgName,
                new CodePrimitiveExpression(nameArgValue));

            toReturn = new CodeAttributeDeclaration(
                functionAttrType,
                isComposibleArg,
                nameArgument);

            this.loggingProvider.Info(
                $"{nameof(CodeAttributeDeclaration)} created! Returning.");

            return toReturn;
        }
    }
}