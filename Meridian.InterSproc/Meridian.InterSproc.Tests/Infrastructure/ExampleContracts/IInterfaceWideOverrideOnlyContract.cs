namespace Meridian.InterSproc.Tests.Infrastructure.ExampleContracts
{
    [InterSprocContract(Schema = "hr", Prefix = "usp_")]
    public interface IInterfaceWideOverrideOnlyContract
    {
        void FirstStoredProcedure();

        void SecondStoredProcedure(int someValue);
    }
}