// ----------------------------------------------------------------------------
// <copyright file="LoggingProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace SomeCorp.SimpleExample.Console
{
    using System;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="ILoggingProvider" />.
    /// </summary>
    public class LoggingProvider : ILoggingProvider
    {
        /// <summary>
        /// The default background colour for the console.
        /// </summary>
        private readonly ConsoleColor defaultBackgroundColor;

        /// <summary>
        /// THe default foreground colour for the console.
        /// </summary>
        private readonly ConsoleColor defaultForgroundColor;

        /// <summary>
        /// Initialises a new instance of the <see cref="LoggingProvider" />
        /// class.
        /// </summary>
        public LoggingProvider()
        {
            this.defaultBackgroundColor = Console.BackgroundColor;
            this.defaultForgroundColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Debug(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        public void Debug(string msg)
        {
            this.WriteToConsole(
                this.defaultBackgroundColor,
                this.defaultForgroundColor,
                msg,
                null);
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Error(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        public void Error(string msg)
        {
            this.Error(msg, null);
        }

        /// <summary>
        /// Implements
        /// <see cref="ILoggingProvider.Error(string, Exception)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        public void Error(string msg, Exception exception)
        {
            this.WriteToConsole(
                this.defaultBackgroundColor,
                ConsoleColor.Red,
                msg,
                exception);
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Fatal(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        public void Fatal(string msg)
        {
            this.Fatal(msg, null);
        }

        /// <summary>
        /// Implements
        /// <see cref="ILoggingProvider.Fatal(string, Exception)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        public void Fatal(string msg, Exception exception)
        {
            this.WriteToConsole(
                ConsoleColor.Red,
                ConsoleColor.White,
                msg,
                exception);
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Info(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        public void Info(string msg)
        {
            this.WriteToConsole(
                this.defaultBackgroundColor,
                ConsoleColor.Cyan,
                msg,
                null);
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Warn(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        public void Warn(string msg)
        {
            this.Warn(msg, null);
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Warn(string, Exception)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        public void Warn(string msg, Exception exception)
        {
            this.WriteToConsole(
                this.defaultBackgroundColor,
                ConsoleColor.Yellow,
                msg,
                exception);
        }

        /// <summary>
        /// Logs a message out to the console, in a standard format.
        /// </summary>
        /// <param name="backgroundColor">
        /// A <see cref="ConsoleColor" /> value, describing the background
        /// colour of the message.
        /// </param>
        /// <param name="foregroundColor">
        /// A <see cref="ConsoleColor" /> value, describing the foreground
        /// colour of the message.
        /// </param>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        private void WriteToConsole(
            ConsoleColor backgroundColor,
            ConsoleColor foregroundColor,
            string msg,
            Exception exception)
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;

            string msgToWrite =
                $"[{DateTime.Now.ToString("HH:mm:ss")}] => {msg}";

            Console.WriteLine(msgToWrite);

            if (exception != null)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}