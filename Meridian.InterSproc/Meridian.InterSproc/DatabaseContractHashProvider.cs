namespace Meridian.InterSproc
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class DatabaseContractHashProvider : IDatabaseContractHashProvider
    {
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
            //    instances. Take into account the attributes.
            //    a) Method-level attributes take highest priority.
            //    b) Then interface-level attributes.
            //    c) Then name reflection/defaults.
            Type type = typeof(DatabaseContractType);

            this.loggingProvider.Debug(
                $"Pulling back all methods for type {type.FullName}...");

            MethodInfo[] methodInfos = type.GetMethods();

            this.loggingProvider.Info(
                $"{methodInfos.Length} method(s) detected. Converting " +
                $"{nameof(MethodInfo)} instances into " +
                $"{nameof(ContractMethodInformation)} instances...");

            ContractMethodInformation[] contractMethodInformations =
                methodInfos
                    .Select(this.ConvertMethodInfoToContractMethodInformation)
                    .ToArray();

            // 2) Use these instances to get a hash.
            toReturn = this.ConvertContractMethodInformationInstancesToHash(
                contractMethodInformations);
            
            return toReturn;
        }

        private ContractMethodInformation ConvertMethodInfoToContractMethodInformation(MethodInfo methodInfo)
        {
            ContractMethodInformation toReturn = null;

            return toReturn;
        }

        private byte[] ConvertContractMethodInformationInstancesToHash(ContractMethodInformation[] toConvert)
        {
            byte[] toReturn = null;

            return toReturn;
        }
    }
}
