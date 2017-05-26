namespace Meridian.InterSproc
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterSprocContractMethodAttribute : InterSprocAttributeBase
    {
        public string Name
        {
            get;
            set;
        }
    }
}
