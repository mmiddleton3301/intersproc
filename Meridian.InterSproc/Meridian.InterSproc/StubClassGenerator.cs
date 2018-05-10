// ----------------------------------------------------------------------------
// <copyright file="StubClassGenerator.cs" company="MTCS">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Implements <see cref="IStubClassGenerator" />.
    /// </summary>
    public class StubClassGenerator : IStubClassGenerator
    {
        private const string StubImplementationClassName =
            "{0}StubImplementation";

        private readonly ILoggingProvider loggingProvider;
        private readonly IStubMethodGeneratorFactory stubMethodGeneratorFactory;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubClassGenerator" /> class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        /// <param name="stubMethodGeneratorFactory">
        /// An instance of type <see cref="IStubMethodGeneratorFactory" />.
        /// </param>
        public StubClassGenerator(
            ILoggingProvider loggingProvider,
            IStubMethodGeneratorFactory stubMethodGeneratorFactory)
        {
            this.loggingProvider = loggingProvider;
            this.stubMethodGeneratorFactory = stubMethodGeneratorFactory;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubClassGenerator.CreateClass(Type, IEnumerable{ContractMethodInformation})" />.
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

            // 1) Declare class.
            this.loggingProvider.Debug("Setting up stub class declaration...");

            toReturn = new CodeTypeDeclaration()
            {
                Name = className,
                IsClass = true,
                Attributes = MemberAttributes.Public,
            };

            // 2) Implements the database contract passed in.
            this.loggingProvider.Debug(
                $"Implementing {databaseContractType.Name}...");

            toReturn.BaseTypes.Add(databaseContractType);

            // 3) Connection string field.
            this.loggingProvider.Debug(
                "Adding in the private connection string member...");

            CodeMemberField connectionStringMember = new CodeMemberField(
                typeof(string),
                "connectionString");
            toReturn.Members.Add(connectionStringMember);

            // 4) The constructor.
            this.loggingProvider.Debug(
                "Generating the constructor declaration...");

            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public,
            };

            // 5) Injectable settings provider, containing connection string.
            this.loggingProvider.Debug(
                "Declaring the stub implementation settings provider param " +
                "in the constructor...");

            CodeParameterDeclarationExpression settingsProviderInject =
                new CodeParameterDeclarationExpression(
                    typeof(IStubImplementationSettingsProvider).Name,
                    "stubImplementationSettingsProvider");
            constructor.Parameters.Add(settingsProviderInject);

            // 6) this.connectionString = stubImplementationSettingsProvider.ConnStr;
            this.loggingProvider.Debug(
                "Assigning the private connection string member with the " +
                "value from the ConnStr property of the stub implementation " +
                "settings provider...");
            CodePropertyReferenceExpression connStrPropertyRef =
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(
                        settingsProviderInject.Name),
                    nameof(IStubImplementationSettingsProvider.ConnStr));

            constructor.Statements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        connectionStringMember.Name),
                    connStrPropertyRef));
            toReturn.Members.Add(constructor);

            this.loggingProvider.Debug($"Implementing interface methods...");

            IStubMethodGenerator stubMethodGenerator =
                this.stubMethodGeneratorFactory.Create(connectionStringMember);

            CodeMemberMethod[] implementedMethods = contractMethodInformations
                .Select(stubMethodGenerator.CreateMethod)
                .ToArray();

            toReturn.Members.AddRange(implementedMethods);

            this.loggingProvider.Info(
                $"{implementedMethods.Length} method(s) generated and " +
                $"appended to the {nameof(CodeTypeDeclaration)} instance.");

            return toReturn;
        }
    }
}