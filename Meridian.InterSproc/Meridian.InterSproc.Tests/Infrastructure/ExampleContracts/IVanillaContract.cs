namespace Meridian.InterSproc.Tests.Infrastructure.ExampleContracts
{
    public interface IVanillaContract
    {
        void FirstStoredProcedure();

        void SecondStoredProcedure(int someValue);
    }
}