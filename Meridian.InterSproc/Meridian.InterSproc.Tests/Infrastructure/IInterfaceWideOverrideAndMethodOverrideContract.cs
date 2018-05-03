namespace Meridian.InterSproc.Tests.Infrastructure
{
    [InterSprocContract(Schema = "sec", Prefix = "mysp_")]
    public interface IInterfaceWideOverrideAndMethodOverrideContract
    {
        void FirstStoredProcedure();

        void SecondStoredProcedure(int someValue);

        [InterSprocContractMethod(Schema = "dbo", Prefix = "")]
        void OutsideOfSchemaStoredProcedure(bool someValue, int anotherValue);

        [InterSprocContractMethod(Name = "ThirdStoredProcedure")]
        void NameOverriddenStoredProcedure(string value1, string value2, string value3);
    }
}