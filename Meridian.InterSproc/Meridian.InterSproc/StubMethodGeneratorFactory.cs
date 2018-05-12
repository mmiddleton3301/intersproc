// ----------------------------------------------------------------------------
// <copyright file="StubMethodGeneratorFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.CodeDom;
    using System.Diagnostics.CodeAnalysis;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="IStubMethodGeneratorFactory" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StubMethodGeneratorFactory : IStubMethodGeneratorFactory
    {
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubMethodGeneratorFactory" /> class.
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        public StubMethodGeneratorFactory(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubMethodGeneratorFactory.Create(CodeMemberField)" />.
        /// </summary>
        /// <param name="connectionStringMember">
        /// The connection string <see cref="CodeMemberField" /> for the parent
        /// stub's class.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="IStubMethodGenerator" />.
        /// </returns>
        public IStubMethodGenerator Create(CodeMemberField connectionStringMember)
        {
            IStubMethodGenerator toReturn = new StubMethodGenerator(
                this.loggingProvider,
                connectionStringMember);

            return toReturn;
        }
    }
}