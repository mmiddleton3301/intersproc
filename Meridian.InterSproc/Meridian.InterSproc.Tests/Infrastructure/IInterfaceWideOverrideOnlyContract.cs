namespace Meridian.InterSproc.Tests.Infrastructure
{
    [InterSprocContract(Prefix = "usp_", Schema = "hr")]
    public interface IInterfaceWideOverrideOnlyContract
    {
        void FirstStoredProcedure();

        void SecondStoredProcedure(int someValue);
    }
}