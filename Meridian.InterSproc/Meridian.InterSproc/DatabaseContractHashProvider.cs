namespace Meridian.InterSproc
{
    using System;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;

    internal class DatabaseContractHashProvider : IDatabaseContractHashProvider
    {
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

            MethodInfo[] methodInfos = type.GetMethods();

            foreach (MethodInfo mi in methodInfos)
            {
                // Do.
            }

            // 2) Use these instances to get a hash.
            return toReturn;
        }
    }
}
