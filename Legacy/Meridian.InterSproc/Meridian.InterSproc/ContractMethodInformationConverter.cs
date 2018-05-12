// ----------------------------------------------------------------------------
// <copyright
//      file="ContractMethodInformationConverter.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IContractMethodInformationConverter" />.
    /// </summary>
    public class ContractMethodInformationConverter
        : IContractMethodInformationConverter
    {
        /// <summary>
        /// The default schema for SQL server databases.
        /// </summary>
        private const string DefaultSchema = "dbo";

        /// <summary>
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="ContractMethodInformationConverter" /> class. 
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </param>
        public ContractMethodInformationConverter(
            ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements
        /// <see cref="IContractMethodInformationConverter.GetContractMethodInformationFromContract{DatabaseContractType}()" />.
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <returns>
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </returns>
        public ContractMethodInformation[] GetContractMethodInformationFromContract<DatabaseContractType>()
            where DatabaseContractType : class
        {
            ContractMethodInformation[] toReturn = null;

            // 1) Parse method declarations into ContractMethodInformation
            //    instances.
            Type databaseContractType = typeof(DatabaseContractType);

            this.loggingProvider.Debug(
                $"Pulling back all {nameof(MethodInfo)}s from type " +
                $"{databaseContractType.Name}...");

            MethodInfo[] methodInfos = databaseContractType.GetMethods();

            this.loggingProvider.Info(
                $"{methodInfos.Length} method(s) pulled back from " +
                $"{databaseContractType.Name}.");

            Type interSprocMethodAttributeType =
                typeof(InterSprocContractAttribute);

            this.loggingProvider.Debug(
                $"Looking for the {interSprocMethodAttributeType.Name} at " +
                $"the interface level...");

            CustomAttributeData classLevelAttribute =
                databaseContractType.CustomAttributes
                    .SingleOrDefault(x => x.AttributeType == interSprocMethodAttributeType);

            if (classLevelAttribute != null)
            {
                this.loggingProvider.Info(
                    $"{interSprocMethodAttributeType.Name} present.");
            }
            else
            {
                this.loggingProvider.Debug(
                    $"No {interSprocMethodAttributeType.Name} is present.");
            }

            toReturn = methodInfos
                .Select(x => this.ConvertMethodInfoToContractMethodInformation(classLevelAttribute, x))
                .ToArray();

            this.loggingProvider.Info(
                $"Returning {toReturn.Length} " +
                $"{nameof(ContractMethodInformation)} instance(s).");

            return toReturn;
        }

        /// <summary>
        /// Ammends an instance of <see cref="ContractMethodInformation" />
        /// with information based on the input
        /// <see cref="CustomAttributeData" />.
        /// </summary>
        /// <param name="contractMethodInformation">
        /// An instance of <see cref="ContractMethodInformation" />.
        /// </param>
        /// <param name="customAttributeData">
        /// An instance of <see cref="CustomAttributeData" /> that may or may
        /// not contain data to ammend the input
        /// <paramref name="contractMethodInformation" />. 
        /// </param>
        private void AmmendContractMethodInformationWithAttributeData(
            ContractMethodInformation contractMethodInformation,
            CustomAttributeData customAttributeData)
        {
            this.ExtractCustomAttributeValue(
                customAttributeData,
                nameof(InterSprocContractAttribute.Schema),
                x =>
                {
                    contractMethodInformation.Schema = x;
                });

            this.ExtractCustomAttributeValue(
                customAttributeData,
                nameof(InterSprocContractAttribute.Prefix),
                x =>
                {
                    contractMethodInformation.Prefix = x;
                });

            this.ExtractCustomAttributeValue(
                customAttributeData,
                nameof(InterSprocContractMethodAttribute.Name),
                x =>
                {
                    contractMethodInformation.Name = x;
                });
        }

        /// <summary>
        /// Composes a <see cref="ContractMethodInformation" /> instance
        /// according to the naming priorities of:
        /// a) Method level attributes;
        /// b) Interface level attributes;
        /// c) Information extracted from the name of the method.
        /// </summary>
        /// <param name="interfaceLevelAttribute">
        /// The interface-level <see cref="ContractMethodInformation" />
        /// instance (if there is one).
        /// </param>
        /// <param name="methodInfo">
        /// A <see cref="MethodInfo" /> instance describing the current method
        /// being converted.
        /// </param>
        /// <returns>
        /// An instance of <see cref="ContractMethodInformation" />. 
        /// </returns>
        private ContractMethodInformation ConvertMethodInfoToContractMethodInformation(
            CustomAttributeData interfaceLevelAttribute,
            MethodInfo methodInfo)
        {
            ContractMethodInformation toReturn = null;

            // Take into account the attributes.
            //    a) Method-level attributes take highest priority.
            //    b) Then interface-level attributes.
            //    c) Then name reflection/defaults.
            // Start with the lowest... reflection/defaults.
            this.loggingProvider.Debug(
                $"Constructing {nameof(ContractMethodInformation)} instance " +
                $"with basic {nameof(MethodInfo)}...");

            toReturn = new ContractMethodInformation()
            {
                Schema = DefaultSchema,
                Prefix = null,
                Name = methodInfo.Name,
                MethodInfo = methodInfo
            };

            this.loggingProvider.Debug($"{nameof(toReturn)} = {toReturn}.");

            // Then ammend based on class-level attribute (if there is one,
            // of course).
            if (interfaceLevelAttribute != null)
            {
                this.loggingProvider.Debug(
                    $"Ammending {nameof(ContractMethodInformation)} based " +
                    $"on the interface-level " +
                    $"{nameof(CustomAttributeData)}...");

                this.AmmendContractMethodInformationWithAttributeData(
                    toReturn,
                    interfaceLevelAttribute);

                this.loggingProvider.Debug(
                    $"{nameof(toReturn)} = {toReturn}.");
            }

            Type interSprocContractMethodAttributeType =
                typeof(InterSprocContractMethodAttribute);

            this.loggingProvider.Debug(
                $"Looking for the " +
                $"{interSprocContractMethodAttributeType.Name} at the " +
                $"interface level...");

            CustomAttributeData methodLevelAttribute =
                methodInfo.CustomAttributes
                    .SingleOrDefault(x => x.AttributeType == interSprocContractMethodAttributeType);

            // Finally, ammend based on the method-level attribute (if there
            // is one).
            if (methodLevelAttribute != null)
            {
                this.loggingProvider.Info(
                    $"{interSprocContractMethodAttributeType.Name} present.");

                this.AmmendContractMethodInformationWithAttributeData(
                    toReturn,
                    methodLevelAttribute);

                this.loggingProvider.Debug(
                    $"{nameof(toReturn)} = {toReturn}.");
            }
            else
            {
                this.loggingProvider.Debug(
                    $"No {interSprocContractMethodAttributeType.Name} is " +
                    $"present.");
            }

            return toReturn;
        }

        /// <summary>
        /// Pulls back a <see cref="string" /> value from the input
        /// <paramref name="customAttributeData" /> with a particular name
        /// (<paramref name="valueName"/>).
        /// If not null, then <paramref name="setMethod" /> is executed,
        /// injecting the extracted <see cref="string" /> value.
        /// </summary>
        /// <param name="customAttributeData">
        /// An instance of <see cref="CustomAttributeData" /> to extract values
        /// from.
        /// </param>
        /// <param name="valueName">
        /// The name of the value to extract.
        /// </param>
        /// <param name="setMethod">
        /// An instance of <see cref="Action{string}" /> which is invoked if
        /// the value extracted is not null. 
        /// </param>
        private void ExtractCustomAttributeValue(
            CustomAttributeData customAttributeData,
            string valueName,
            Action<string> setMethod)
        {
            this.loggingProvider.Debug(
                $"Checking for value \"{valueName}\" in the " +
                $"{nameof(CustomAttributeData)}...");

            CustomAttributeNamedArgument arg = customAttributeData
                .NamedArguments
                .SingleOrDefault(x => x.MemberName == valueName);

            if (arg.TypedValue.Value != null)
            {
                this.loggingProvider.Debug(
                    $"\"{valueName}\" value extracted: " +
                    $"\"{arg.TypedValue.Value}\". Calling " +
                    $"{nameof(setMethod)}...");

                setMethod(arg.TypedValue.Value as string);

                this.loggingProvider.Debug(
                    $"{nameof(setMethod)} invoked with value " +
                    $"\"{arg.TypedValue.Value}\".");
            }
            else
            {
                this.loggingProvider.Debug(
                    $"No value found for \"{valueName}\".");
            }
        }
    }
}