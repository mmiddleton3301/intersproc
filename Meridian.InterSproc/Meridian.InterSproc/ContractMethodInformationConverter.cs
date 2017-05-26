namespace Meridian.InterSproc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class ContractMethodInformationConverter : IContractMethodInformationConverter
    {
        private const string DefaultSchema = "dbo";

        public ContractMethodInformation[] GetContractMethodInformationFromContract<DatabaseContractType>() where DatabaseContractType : class
        {
            ContractMethodInformation[] toReturn = null;

            // 1) Parse method declarations into ContractMethodInformation
            //    instances.
            Type type = typeof(DatabaseContractType);

            MethodInfo[] methodInfos = type.GetMethods();

            Type interSprocMethodAttributeType =
                typeof(InterSprocContractAttribute);

            CustomAttributeData classLevelAttribute =
                type.CustomAttributes
                    .SingleOrDefault(x => x.AttributeType == interSprocMethodAttributeType);

            toReturn = methodInfos
                    .Select(x => this.ConvertMethodInfoToContractMethodInformation(classLevelAttribute, x))
                    .ToArray();

            return toReturn;
        }

        private ContractMethodInformation ConvertMethodInfoToContractMethodInformation(
            CustomAttributeData classLevelAttribute,
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
                MethodInfo = methodInfo
            };

            // Then ammend based on class-level attribute (if there is one,
            // of course).
            if (classLevelAttribute != null)
            {
                this.AmmendContractMethodInformationWithAttributeData(
                    toReturn,
                    classLevelAttribute);
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
                    toReturn,
                    methodLevelAttribute);
            }

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

        private void ExtractCustomAttributeValue(
            CustomAttributeData customAttributeData,
            string valueName,
            Action<string> setMethod)
        {
            CustomAttributeNamedArgument arg = customAttributeData
                .NamedArguments
                .SingleOrDefault(x => x.MemberName == valueName);

            if (arg.TypedValue.Value != null)
            {
                setMethod(arg.TypedValue.Value as string);
            }
        }
    }
}
