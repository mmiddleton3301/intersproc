namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.CodeDom;
    using Meridian.InterSproc.Model;

    public interface IStubImplementationGenerator
    {

        CodeTypeDeclaration CreateClass(
            Type type,
            CodeTypeReference dataContextType,
            ContractMethodInformation[] contractMethodInformations,
            CodeMemberMethod[] dataContextMethods);
    }
}
