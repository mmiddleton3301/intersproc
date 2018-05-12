// ----------------------------------------------------------------------------
// <copyright
//      file="DatabaseContractHashProvider.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IDatabaseContractHashProvider" />. 
    /// </summary>
    public class DatabaseContractHashProvider : IDatabaseContractHashProvider
    {
        /// <summary>
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="DatabaseContractHashProvider" /> class. 
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </param>
        public DatabaseContractHashProvider(ILoggingProvider loggingProvider)
        {
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Implements
        /// <see cref="IDatabaseContractHashProvider.GetContractHash(ContractMethodInformation[])" />. 
        /// </summary>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances.
        /// </param>
        /// <returns>
        /// A base-64 encoded SHA-1 hash, describing the uniqueness of the
        /// values passed in via
        /// <paramref name="contractMethodInformations" />.
        /// </returns>
        public string GetContractHash(
            ContractMethodInformation[] contractMethodInformations)
        {
            string toReturn = null;

            byte[] hashBytes = null;

            // Got our instances. Let's do a hash. First, get the bytes
            // of the array.
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this.loggingProvider.Debug(
                    $"Serialising {contractMethodInformations.Length} " +
                    $"instance(s) to an array of bytes...");

                binaryFormatter.Serialize(
                    memoryStream,
                    contractMethodInformations);

                memoryStream.Position = 0;

                // Get our serialised ContractMethodInformation instances,
                // and then do the hash with 'em.
                hashBytes = new byte[memoryStream.Length];

                memoryStream.Read(hashBytes, 0, hashBytes.Length);
            }

            this.loggingProvider.Info(
                $"{hashBytes.Length} byte(s) total. Hashing bytes...");

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                hashBytes = sha1.ComputeHash(hashBytes);
            }

            toReturn = Convert.ToBase64String(hashBytes);

            // Ensure that the hash is filename-safe.
            toReturn = toReturn.Replace('/', '_');
            toReturn = toReturn.Replace('+', '-');

            this.loggingProvider.Info($"Hash generated: \"{toReturn}\".");

            return toReturn;
        }
    }
}