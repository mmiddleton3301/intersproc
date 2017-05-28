// ----------------------------------------------------------------------------
// <copyright file="StubInstanceProvider.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using StructureMap;

    /// <summary>
    /// Implements <see cref="IStubInstanceProvider" />. 
    /// </summary>
    public class StubInstanceProvider : IStubInstanceProvider
    {
        /// <summary>
        /// Implements
        /// <see cref="IStubInstanceProvider.GetInstance{DatabaseContractType}(Assembly, string)" />. 
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="temporaryStubAssembly">
        /// The recently generated temporary stub <see cref="Assembly" />. 
        /// </param>
        /// <param name="connStr">
        /// An SQL database connection string.
        /// </param>
        /// <returns>
        /// A concerete instance of
        /// <typeparamref name="DatabaseContractType" />.
        /// </returns>
        public DatabaseContractType GetInstance<DatabaseContractType>(
            Assembly temporaryStubAssembly,
            string connStr)
            where DatabaseContractType : class
        {
            DatabaseContractType toReturn = null;

            CustomStubRegistry customStubRegistry =
                new CustomStubRegistry(temporaryStubAssembly);

            Container container = new Container(customStubRegistry);

            IStubImplementationSettingsProvider stubImplementationSettingsProvider =
                new StubImplementationSettingsProvider(connStr);

            toReturn = container
                .With(stubImplementationSettingsProvider)
                .GetInstance<DatabaseContractType>();

            return toReturn;
        }
    }
}