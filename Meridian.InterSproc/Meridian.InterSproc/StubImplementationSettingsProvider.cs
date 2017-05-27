namespace Meridian.InterSproc
{
    using Meridian.InterSproc.Definitions;

    public class StubImplementationSettingsProvider
        : IStubImplementationSettingsProvider
    {
        public StubImplementationSettingsProvider(string connStr)
        {
            this.ConnStr = connStr;
        }

        public string ConnStr
        {
            get;
            private set;
        }
    }
}
