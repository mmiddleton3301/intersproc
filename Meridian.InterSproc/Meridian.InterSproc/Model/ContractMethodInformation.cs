namespace Meridian.InterSproc.Model
{
    using System;
    using System.Linq;
    using System.Reflection;

    [Serializable]
    public class ContractMethodInformation
    {
        public string Schema
        {
            get;
            set;
        }

        public string Prefix
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public MethodInfo MethodInfo
        {
            get;
            set;
        }

        public override string ToString()
        {
            string toReturn = $"{this.Schema}.{this.Prefix}{this.Name}";

            string[] paramsStr = this.MethodInfo
                .GetParameters()
                .Select(x => $"{x.ParameterType.Name} {x.Name}")
                .ToArray();

            string paramList = string.Join(", ", paramsStr);

            toReturn = $"{toReturn}({paramList})";

            return toReturn;
        }
    }
}
