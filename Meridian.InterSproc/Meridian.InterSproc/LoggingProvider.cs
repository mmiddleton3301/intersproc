// ----------------------------------------------------------------------------
// <copyright file="LoggingProvider.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using Meridian.InterSproc.Definitions;

    /// <summary>
    /// Implements <see cref="ILoggingProvider" />.
    /// This implementation used to contain NLog-specific instances -
    /// since version 1.0.5, the dependence on NLog has been removed.
    /// Therefore, it does absolutely nothing, and simply serves to prevent
    /// <see cref="NullReferenceException" />s being thrown.
    /// You can pass in your own concrete implementation of
    /// <see cref="ILoggingProvider" /> via the
    /// <see cref="SprocStubFactoryCreateOptions" />.
    /// </summary>
    internal class LoggingProvider : ILoggingProvider
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LoggingProvider" />
        /// class.
        /// </summary>
        internal LoggingProvider()
        {
            // Empty constructor.
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Debug(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void ILoggingProvider.Debug(string msg)
        {
            // Does nothing. Implement your own provider.
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Error(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void ILoggingProvider.Error(string msg)
        {
            // Does nothing. Implement your own provider.
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
        void ILoggingProvider.Error(string msg, Exception exception)
        {
            // Does nothing. Implement your own provider.
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Fatal(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void ILoggingProvider.Fatal(string msg)
        {
            // Does nothing. Implement your own provider.
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
        void ILoggingProvider.Fatal(string msg, Exception exception)
        {
            // Does nothing. Implement your own provider.
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Info(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void ILoggingProvider.Info(string msg)
        {
            // Does nothing. Implement your own provider.
        }

        /// <summary>
        /// Implements <see cref="ILoggingProvider.Warn(string)" />.
        /// </summary>
        /// <param name="msg">
        /// The message to log.
        /// </param>
        void ILoggingProvider.Warn(string msg)
        {
            // Does nothing. Implement your own provider.
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
        void ILoggingProvider.Warn(string msg, Exception exception)
        {
            // Does nothing. Implement your own provider.
        }
    }
}