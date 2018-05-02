namespace Meridian.InterSproc.Tests.Infrastructure
{
    public interface IVanillaContract
    {
        void FirstStoredProcedure();

        void SecondStoredProcedure(int someValue);
    }
}