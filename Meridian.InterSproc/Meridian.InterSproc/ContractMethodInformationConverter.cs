// ----------------------------------------------------------------------------
// <copyright file="ContractMethodInformationConverter.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
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
        private const string DefaultSchema = "dbo";

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
        /// <typeparam name="TDatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <returns>
        /// An array of <see cref="ContractMethodInformation" /> instances.
        /// </returns>
        public ContractMethodInformation[] GetContractMethodInformationFromContract<TDatabaseContractType>()
            where TDatabaseContractType : class
        {
            ContractMethodInformation[] toReturn = null;

            // 1) Parse method declarations into ContractMethodInformation
            //    instances.
            Type databaseContractType = typeof(TDatabaseContractType);

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
                MethodInfo = methodInfo,
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