namespace Meridian.InterSproc.Tests.Infrastructure
{
    using System;
    using Meridian.InterSproc.Definitions;
    using SysDiag = System.Diagnostics;

    public class DebugLogger : ILoggingProvider
    {
        public void Debug(string msg)
        {
            this.PipeToDebug(
                nameof(this.Debug),
                msg,
                null);
        }

        public void Error(string msg)
        {
            this.Error(msg, null);
        }

        public void Error(string msg, Exception exception)
        {
            this.PipeToDebug(
                nameof(this.Error),
                msg,
                exception);
        }

        public void Fatal(string msg)
        {
            this.Fatal(
                msg,
                null);
        }

        public void Fatal(string msg, Exception exception)
        {
            this.PipeToDebug(
                nameof(this.Fatal),
                msg,
                exception);
        }

        public void Info(string msg)
        {
            this.PipeToDebug(
                nameof(this.Info),
                msg,
                null);
        }

        public void Warn(string msg)
        {
            this.Warn(
                msg,
                null);
        }

        public void Warn(string msg, Exception exception)
        {
            this.PipeToDebug(
                nameof(this.Warn),
                msg,
                exception);
        }

        private void PipeToDebug(
            string category,
            string msg,
            Exception exception)
        {
            category = category.ToUpper();

            SysDiag.Debug.WriteLine(msg, category);

            if (exception != null)
            {
                SysDiag.Debug.WriteLine(exception.Message, category);
                SysDiag.Debug.WriteLine(exception.StackTrace, category);
            }
        }
    }
}