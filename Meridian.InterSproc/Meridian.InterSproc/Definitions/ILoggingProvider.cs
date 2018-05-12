// ----------------------------------------------------------------------------
// <copyright file="ILoggingProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Definitions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Describes the operations of the logging provider.
    /// </summary>
    public interface ILoggingProvider
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void Debug(string msg);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1716",
            Justification = "Cannot be renamed easily, as consuming applications may implement method currently. Additionally, method name makes sense given the context of the interface, and is standard practice for loggers.")]
        void Error(string msg);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1716",
            Justification = "Cannot be renamed easily, as consuming applications may implement method currently. Additionally, method name makes sense given the context of the interface, and is standard practice for loggers.")]
        void Error(string msg, Exception exception);

        /// <summary>
        /// Logs a fatal event.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void Fatal(string msg);

        /// <summary>
        /// Logs a fatal event.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        void Fatal(string msg, Exception exception);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void Info(string msg);

        /// <summary>
        /// Logs a message as a warning.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void Warn(string msg);

        /// <summary>
        /// Logs a message as a warning.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        void Warn(string msg, Exception exception);
    }
}