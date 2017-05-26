namespace Meridian.InterSproc.Model
{
    using System;
    using System.Collections.Generic;

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
    }
}
