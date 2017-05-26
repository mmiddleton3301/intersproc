namespace Meridian.InterSproc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class DatabaseContractHashProvider : IDatabaseContractHashProvider
    {
        private const string DefaultSchema = "dbo";

        private readonly ILoggingProvider loggingProvider;

        public DatabaseContractHashProvider(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        public byte[] GetContractHash<DatabaseContractType>()
            where DatabaseContractType : class
        {
            byte[] toReturn = null;

            // 1) Parse method declarations into ContractMethodInformation
            //    instances.
            Type type = typeof(DatabaseContractType);

            this.loggingProvider.Debug(
                $"Pulling back all methods for type {type.FullName}...");

            MethodInfo[] methodInfos = type.GetMethods();

            Type interSprocMethodAttributeType =
                typeof(InterSprocContractMethodAttribute);

            CustomAttributeData classLevelAttribute =
                type.CustomAttributes
                    .SingleOrDefault(x => x.AttributeType == interSprocMethodAttributeType);

            ContractMethodInformation[] contractMethodInformations =
                methodInfos
                    .Select(x => this.ConvertMethodInfoToContractMethodInformation(classLevelAttribute, x))
                    .ToArray();

            // 2) Use these instances to get a hash.
            toReturn = this.ConvertContractMethodInformationInstancesToHash(
                contractMethodInformations);
            
            return toReturn;
        }

        private ContractMethodInformation ConvertMethodInfoToContractMethodInformation(
            CustomAttributeData classLevelAttributes,
            MethodInfo methodInfo)
        {
            ContractMethodInformation toReturn = null;

            // Take into account the attributes.
            //    a) Method-level attributes take highest priority.
            //    b) Then interface-level attributes.
            //    c) Then name reflection/defaults.
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            Dictionary<Type, string> parameterInfosCasted = parameterInfos
                .ToDictionary(x => x.ParameterType, x => x.Name);

            // Start with the lowest... reflection/defaults.
            toReturn = new ContractMethodInformation()
            {
                Schema = DefaultSchema,
                Prefix = null,
                Name = methodInfo.Name,
                Parameters = parameterInfosCasted
            };

            // Then ammend based on class-level attribute (if there is one,
            // of course).
            if (classLevelAttributes != null)
            {
                this.AmmendContractMethodInformationWithAttributeData(
                    classLevelAttributes);
            }

            Type interSprocContractMethodAttributeType =
                typeof(InterSprocContractMethodAttribute);

            CustomAttributeData methodLevelAttribute =
                methodInfo.CustomAttributes
                    .SingleOrDefault(x => x.AttributeType == interSprocContractMethodAttributeType);

            // Finally, ammend based on the method-level attribute (if there
            // is one).
            if (methodLevelAttribute != null)
            {
                this.AmmendContractMethodInformationWithAttributeData(
                    methodLevelAttribute);
            }
               
            return toReturn;
        }

        private void AmmendContractMethodInformationWithAttributeData(
            CustomAttributeData customAttributeData)
        {

        }

        private byte[] ConvertContractMethodInformationInstancesToHash(
            ContractMethodInformation[] toConvert)
        {
            byte[] toReturn = null;

            return toReturn;
        }
    }
}
