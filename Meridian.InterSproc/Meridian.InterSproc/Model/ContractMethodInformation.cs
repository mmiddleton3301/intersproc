namespace Meridian.InterSproc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public Dictionary<Type, string> Parameters
        {
            get;
            set;
        }

        public override string ToString()
        {
            string toReturn = $"{this.Schema}.{this.Prefix}{this.Name}";

            string[] paramsStr = this.Parameters
                .Select(x => $"{x.Key.Name} {x.Value}")
                .ToArray();

            string paramList = string.Join(", ", paramsStr);

            toReturn = $"{toReturn}({paramList})";

            return toReturn;
        }
    }
}
