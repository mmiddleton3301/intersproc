namespace Meridian.InterSproc.Tests.Infrastructure
{
    [InterSprocContract(Schema = "hr", Prefix = "usp_")]
    public interface IInterfaceWideOverrideOnlyContract
    {
        void FirstStoredProcedure();

        void SecondStoredProcedure(int someValue);
    }
}