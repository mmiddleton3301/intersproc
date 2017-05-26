namespace Meridian.InterSproc
{
    using System;

    public abstract class InterSprocAttributeBase : Attribute
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
    }
}
