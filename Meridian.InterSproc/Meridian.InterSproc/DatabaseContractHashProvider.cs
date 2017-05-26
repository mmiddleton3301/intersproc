namespace Meridian.InterSproc
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    public class DatabaseContractHashProvider : IDatabaseContractHashProvider
    {
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
                binaryFormatter.Serialize(
                    memoryStream,
                    contractMethodInformations);

                memoryStream.Position = 0;

                // Get our serialised ContractMethodInformation instances,
                // and then do the hash with 'em.
                hashBytes = new byte[memoryStream.Length];

                memoryStream.Read(hashBytes, 0, hashBytes.Length);
            }

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                hashBytes = sha1.ComputeHash(hashBytes);
            }

            toReturn = Convert.ToBase64String(hashBytes);

            toReturn = toReturn.Replace('/', '_');
            toReturn = toReturn.Replace('+', '-');

            return toReturn;
        }
    }
}
