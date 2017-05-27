namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    public interface IStubCommonGenerator
    {
        CodeStatement[] GenerateMethodBody(
            Type returnType,
            Action<List<CodeStatement>> bodyBuilderAction);
    }
}
