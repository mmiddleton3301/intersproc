// ----------------------------------------------------------------------------
// <copyright
//      file="StubImplementationGenerator.cs"
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
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;
    using System.Reflection;

    /// <summary>
    /// Implements <see cref="IStubImplementationGenerator" />. 
    /// </summary>
    public class StubImplementationGenerator : IStubImplementationGenerator
    {
        /// <summary>
        /// The format of the name of the contract implementation 
        /// class, {0} being the original database contract name.
        /// </summary>
        private const string StubImplementationClassName = 
            "{0}StubImplementation";

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
        /// <see cref="StubImplementationGenerator" /> class. 
        /// </summary>
        /// <param name="stubCommonGenerator">
        /// An instance of <see cref="IStubCommonGenerator" />. 
        /// </param>
        public StubImplementationGenerator(
            ILoggingProvider loggingProvider,
            IStubCommonGenerator stubCommonGenerator)
        {
            this.loggingProvider = loggingProvider;
            this.stubCommonGenerator = stubCommonGenerator;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubImplementationGenerator.CreateClass(Type, CodeTypeReference, ContractMethodInformation[], CodeMemberMethod[])" />. 
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="dataContextType">
        /// An instance of <see cref="CodeTypeReference" />, refering to the
        /// previously generated
        /// <see cref="System.Data.Linq.DataContext" />-derived class.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </param>
        /// <param name="dataContextMethods">
        /// An array of <see cref="CodeMemberMethod" /> instances, beloinging
        /// to the input <paramref name="dataContextType" />.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeTypeDeclaration" />. 
        /// </returns>
        public CodeTypeDeclaration CreateClass(
            Type databaseContractType,
            CodeTypeReference dataContextType,
            ContractMethodInformation[] contractMethodInformations,
            CodeMemberMethod[] dataContextMethods)
        {
            CodeTypeDeclaration toReturn = null;

            // Come up with the class name.
            string className = string.Format(
                StubImplementationClassName,
                databaseContractType.Name);

            this.loggingProvider.Debug(
                $"Constructing implementation of " +
                $"{databaseContractType.Name} called \"{className}\".");

            // Declare class.
            toReturn = new CodeTypeDeclaration()
            {
                Name = className,
                IsClass = true,
                Attributes = MemberAttributes.Public
            };

            // Implements the database contract.
            toReturn.BaseTypes.Add(databaseContractType);

            this.loggingProvider.Debug(
                $"Adding private member for {dataContextType.BaseType} " +
                $"instance...");

            // DataContext member declearation
            CodeMemberField dataContextMember = new CodeMemberField(
                dataContextType,
                "dataContext");

            toReturn.Members.Add(dataContextMember);

            this.loggingProvider.Info(
                $"Private member of type {dataContextType.BaseType} " +
                $"added.");

            this.loggingProvider.Debug(
                $"Putting together constructor for \"{className}\"...");

            // Constructor
            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };

            // Injectable settings provider, containing connection string.
            CodeParameterDeclarationExpression settingsProviderInject =
                new CodeParameterDeclarationExpression(
                    typeof(IStubImplementationSettingsProvider),
                    "stubImplementationSettingsProvider");

            constructor.Parameters.Add(settingsProviderInject);

            // Create a reference to the DataContext member...
            CodeThisReferenceExpression thisRef =
                new CodeThisReferenceExpression();
            CodeFieldReferenceExpression dataContextRef =
                new CodeFieldReferenceExpression(
                    thisRef,
                    dataContextMember.Name);

            // Make a reference to the "ConnStr" property of the injectable
            // settings provider.
            CodePropertyReferenceExpression settingsRefExpr =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(
                        settingsProviderInject.Name),
                    "ConnStr");

            // new DataContext()...
            CodeObjectCreateExpression createExpr =
                new CodeObjectCreateExpression(
                    dataContextType,
                    settingsRefExpr);

            // Into our class member.
            CodeAssignStatement assignStatement = new CodeAssignStatement(
                dataContextRef,
                createExpr);

            constructor.Statements.Add(assignStatement);

            toReturn.Members.Add(constructor);

            this.loggingProvider.Info(
                $"Constructor generated and added. Generating " +
                $"{contractMethodInformations.Length} implementation(s)...");

            // Method implemnetations...
            CodeMemberMethod[] implementedMethods = contractMethodInformations
                .Select(x => this.ImplementContractMethod(x, dataContextMethods))
                .ToArray();

            toReturn.Members.AddRange(implementedMethods);

            this.loggingProvider.Info(
                $"{implementedMethods.Length} method(s) generated and " +
                $"appended to the class. Returning.");

            return toReturn;
        }

        /// <summary>
        /// Builds the contract implementation's method's bodies, by adding
        /// <see cref="CodeStatement" /> instances to
        /// <paramref name="codeStatements" />.
        /// </summary>
        /// <param name="codeStatements">
        /// An instance of <see cref="List{CodeStatement}" /> - method body
        /// contents are to be added to this collection.
        /// </param>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />. 
        /// </param>
        /// <param name="paramsToAdd">
        /// An array of <see cref="CodeParameterDeclarationExpression" />
        /// instances, describing the parameters added to the method so that
        /// they can be used to invoke the injected
        /// <see cref="System.Data.Linq.DataContext" /> instance.
        /// TODO: Review the params here - do we really need
        ///       <paramref name="returnType" />? Can't we derive this from
        ///       <paramref name="contractMethodInformation" /> instead?
        /// </param>
        /// <param name="dataContextMethods">
        /// An array of <see cref="CodeMemberMethod" />s describing the
        /// injected <see cref="System.Data.Linq.DataContext" /> methods.
        /// Used to easily assess the return type of each method (i.e. as they
        /// are, wrapped up in the
        /// <see cref="System.Data.Linq.ISingleResult{T}" /> generic). 
        /// </param>
        /// <param name="returnType">
        /// The return <see cref="Type" /> of the stub method.
        /// </param>
        private void BuildMethodBody(
            List<CodeStatement> codeStatements,
            ContractMethodInformation contractMethodInformation,
            CodeParameterDeclarationExpression[] paramsToAdd,
            CodeMemberMethod[] dataContextMethods,
            Type returnType)
        {
            this.loggingProvider.Debug(
                $"Generating {nameof(System.Data.Linq.DataContext)} invoke " +
                "line...");

            CodeFieldReferenceExpression dataContextRef =
                       new CodeFieldReferenceExpression(
                           new CodeThisReferenceExpression(),
                           "dataContext");

            CodeMethodReferenceExpression methodRef =
                new CodeMethodReferenceExpression(
                        dataContextRef,
                        contractMethodInformation.MethodInfo.Name);

            CodeVariableReferenceExpression[] paramsRefs =
                paramsToAdd
                    .Select(y => new CodeVariableReferenceExpression(y.Name))
                    .ToArray();

            // Invoke the data context method for this stub method.
            CodeMethodInvokeExpression dataContextMethodInvoke =
                new CodeMethodInvokeExpression(
                    methodRef,
                    paramsRefs);

            // Find the method that was generated previously.
            CodeMemberMethod dataContextMethod = dataContextMethods
                .Single(y => y.Name == contractMethodInformation.MethodInfo.Name);

            if (returnType != typeof(void))
            {
                CodeVariableDeclarationStatement singleResultCont =
                    new CodeVariableDeclarationStatement(
                        dataContextMethod.ReturnType,
                        $"single{dataContextMethod.Name}Result",
                        dataContextMethodInvoke);

                codeStatements.Add(singleResultCont);

                this.loggingProvider.Info(
                    $"Generated {nameof(System.Data.Linq.DataContext)} " +
                    "invoke line (return value captured).");

                CodeSnippetStatement newLine =
                        new CodeSnippetStatement(string.Empty);

                this.loggingProvider.Debug(
                    "Generting unboxing line based on array status...");

                CodeMethodReferenceExpression toArrayMethRef =
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression(singleResultCont.Name),
                        returnType.BaseType == typeof(Array) ? "ToArray" : "SingleOrDefault");

                CodeMethodInvokeExpression toArrayMethodInvoke =
                    new CodeMethodInvokeExpression(toArrayMethRef);

                CodeAssignStatement assignToReturn =
                    new CodeAssignStatement(
                        new CodeVariableReferenceExpression("toReturn"),
                        toArrayMethodInvoke);

                this.loggingProvider.Info($"Unboxing line generated.");

                codeStatements.Add(newLine);
                codeStatements.Add(assignToReturn);
            }
            else
            {
                codeStatements.Add(
                    new CodeExpressionStatement(dataContextMethodInvoke));

                this.loggingProvider.Info(
                    $"Generated {nameof(System.Data.Linq.DataContext)} " +
                    "invoke line (no return value captured).");
            }
        }

        /// <summary>
        /// Implements a single databse contract method, using the supplied
        /// <see cref="ContractMethodInformation" />.
        /// TODO: Would it be more sensible just to pass the corresponding
        ///       <see cref="System.Data.Linq.DataContext" /> method into
        ///       this one, rather than the whole array?
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />. 
        /// </param>
        /// <param name="dataContextMethods">
        /// An array of <see cref="CodeMemberMethod" /> instances, describing
        /// all of the corresponding
        /// <see cref="System.Data.Linq.DataContext" /> methods.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeMemberMethod" /> (i.e. the
        /// implementation DOM).
        /// </returns>
        private CodeMemberMethod ImplementContractMethod(
            ContractMethodInformation contractMethodInformation,
            CodeMemberMethod[] dataContextMethods)
        {
            CodeMemberMethod toReturn = null;

            toReturn = new CodeMemberMethod()
            {
                Name = contractMethodInformation.MethodInfo.Name,
                Attributes = MemberAttributes.Public | MemberAttributes.Final
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

            CodeStatement[] body = this.stubCommonGenerator.GenerateMethodBody(
                returnType,
                x => this.BuildMethodBody(
                    x,
                    contractMethodInformation,
                    paramsToAdd,
                    dataContextMethods,
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