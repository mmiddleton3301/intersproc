namespace Meridian.InterSproc
{
    using System;
    using System.CodeDom.Compiler;

    public class StubGenerationException : Exception
    {
        private const string ExceptionMessageTemplate =
            "{0} error(s) were raised during the compilation of the stub " +
            "assembly.";

        public StubGenerationException(
            CompilerError[] compilerErrors)
            : base(string.Format(
                ExceptionMessageTemplate,
                compilerErrors.Length))
        {
            this.CompilerErrors = compilerErrors;
        }

        public CompilerError[] CompilerErrors
        {
            get;
            private set;
        }
    }
}
