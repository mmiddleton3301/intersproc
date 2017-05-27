namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.CodeDom;
    using Meridian.InterSproc.Model;

    public interface IStubDatabaseContextGenerator
    {
        CodeTypeDeclaration CreateClass(
            Type type,
            ContractMethodInformation[] contractMethodInformations);
    }
}
