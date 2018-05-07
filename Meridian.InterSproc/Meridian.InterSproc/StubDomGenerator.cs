// ----------------------------------------------------------------------------
// <copyright file="StubDomGenerator.cs" company="MTCS">
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
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Models;

    /// <summary>
    /// Implements <see cref="IStubDomGenerator" />.
    /// </summary>
    public class StubDomGenerator : IStubDomGenerator
    {
        private const string BaseStubNamespace =
            "Meridian.InterSproc.TemporaryStub";

        private readonly ILoggingProvider loggingProvider;
        private readonly IStubImplementationGenerator stubImplementationGenerator;

        /// <summary>
        /// Initialises a new instance of the <see cref="StubDomGenerator" />
        /// class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        /// <param name="stubImplementationGenerator">
        /// An instance of type <see cref="IStubImplementationGenerator" />.
        /// </param>
        public StubDomGenerator(
            ILoggingProvider loggingProvider,
            IStubImplementationGenerator stubImplementationGenerator)
        {
            this.loggingProvider = loggingProvider;
            this.stubImplementationGenerator = stubImplementationGenerator;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubDomGenerator.GenerateEntireStubAssemblyDom(Type, IEnumerable{ContractMethodInformation})" />.
        /// </summary>
        /// <param name="databaseContractType">
        /// A <see cref="Type" /> instance, describing the database contract.
        /// </param>
        /// <param name="contractMethodInformations">
        /// A collection of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CodeNamespace" />, containing
        /// implementations of all the relevant sub-classes and configuration
        /// to produce the stub assembly.
        /// </returns>
        public CodeNamespace GenerateEntireStubAssemblyDom(
            Type databaseContractType,
            IEnumerable<ContractMethodInformation> contractMethodInformations)
        {
            CodeNamespace toReturn = new CodeNamespace(BaseStubNamespace);

            string[] namespacesToAdd =
            {
                // System.Data
                typeof(IDbConnection).Namespace,

                // System.Data.SqlClient
                typeof(SqlConnection).Namespace,

                // System.Linq
                typeof(Enumerable).Namespace,

                // Dapper
                nameof(Dapper),

                // Meridian.InterSproc.Definitions
                typeof(IStubImplementationSettingsProvider).Namespace,
            };

            // Add usings...
            CodeNamespaceImport[] usingStatements = namespacesToAdd
                .Select(x => new CodeNamespaceImport(x))
                .ToArray();

            toReturn.Imports.AddRange(usingStatements);

            this.loggingProvider.Debug(
                $"Generating implementation of " +
                $"{databaseContractType.Name}...");

            CodeTypeDeclaration interfaceImplementation =
                this.stubImplementationGenerator.CreateClass(
                    databaseContractType,
                    contractMethodInformations);
            toReturn.Types.Add(interfaceImplementation);

            this.loggingProvider.Info(
                $"Implementation of {databaseContractType.Name} generated.");

            return toReturn;
        }
    }
}