// ----------------------------------------------------------------------------
// <copyright file="StubAssemblyManager.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IStubAssemblyManager" />. 
    /// </summary>
    public class StubAssemblyManager : IStubAssemblyManager
    {
        /// <summary>
        /// The format of a stub assembly filename.
        /// </summary>
        private const string TemporaryStubAssemblyName = 
            "Temporary_{0}.isa";

        /// <summary>
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// An instance of <see cref="IStubAssemblyGenerator" />. 
        /// </summary>
        private readonly IStubAssemblyGenerator stubAssemblyGenerator;

        /// <summary>
        /// An instance of <see cref="DirectoryInfo" />, describing where
        /// temporary assemblies are stored (typically the binary execution
        /// directory).
        /// </summary>
        private readonly DirectoryInfo temporaryAssemblyLocation;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubAssemblyManager" /> class. 
        /// </summary>
        /// <param name="loggingProvider">
        /// An instance of <see cref="ILoggingProvider" />. 
        /// </param>
        /// <param name="stubAssemblyGenerator">
        /// An instance of <see cref="IStubAssemblyGenerator" />. 
        /// </param>
        public StubAssemblyManager(
            ILoggingProvider loggingProvider,
            IStubAssemblyGenerator stubAssemblyGenerator)
        {
            Assembly executing = Assembly.GetExecutingAssembly();

            string assemlyExecutionLocation =
                $"{executing.Location}";

            this.temporaryAssemblyLocation =
                (new FileInfo(assemlyExecutionLocation)).Directory;

            this.loggingProvider = loggingProvider;
            this.stubAssemblyGenerator = stubAssemblyGenerator;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubAssemblyManager.CleanupTemporaryAssemblies()" />. 
        /// </summary>
        public void CleanupTemporaryAssemblies()
        {
            string wildcardAssem =
                string.Format(TemporaryStubAssemblyName, "*");

            this.loggingProvider.Debug(
                $"Clearing up any stray stub assemblies. Searching for " +
                $"\"{wildcardAssem}\" in " +
                $"\"{this.temporaryAssemblyLocation.FullName}\"...");

            FileInfo[] temporaryAssemblies =
                this.temporaryAssemblyLocation.GetFiles(wildcardAssem);

            this.loggingProvider.Info(
                $"{temporaryAssemblies.Length} temporary assemblies found. " +
                $"Deleting each file in turn...");

            FileInfo sourceFileSearch = null;
            foreach (FileInfo fileInfo in temporaryAssemblies)
            {
                this.loggingProvider.Debug(
                    $"Deleting \"{fileInfo.FullName}\"...");

                // Unload it first - if it's in the bin dir, then it'll get
                // loaded by the host app by default.
                fileInfo.Delete();

                this.loggingProvider.Info(
                    $"Deleted \"{fileInfo.FullName}\". Searching for " +
                    $"corresponding source file...");

                sourceFileSearch = new FileInfo(fileInfo.Name + ".cs");

                if (sourceFileSearch.Exists)
                {
                    this.loggingProvider.Info(
                        $"\"{sourceFileSearch.FullName}\" exists. " +
                        $"Deleting...");

                    sourceFileSearch.Delete();

                    this.loggingProvider.Info(
                        $"Deleted \"{sourceFileSearch.FullName}\".");
                }
            }
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubAssemblyManager.GenerateStubAssembly{DatabaseContractType}(string, ContractMethodInformation[])" />. 
        /// </summary>
        /// <typeparam name="DatabaseContractType">
        /// The database contract interface type.
        /// </typeparam>
        /// <param name="contractHashStr">
        /// A hash of the contract about to be generated.
        /// </param>
        /// <param name="contractMethodInformations">
        /// An array of <see cref="ContractMethodInformation" /> instances. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" />. 
        /// </returns>
        public Assembly GenerateStubAssembly<DatabaseContractType>(
            string contractHashStr,
            ContractMethodInformation[] contractMethodInformations)
            where DatabaseContractType : class
        {
            Assembly toReturn = null;

            this.loggingProvider.Debug(
                "Constructing destination filename for new Stub Assembly...");

            string destinationFilename = string.Format(
                TemporaryStubAssemblyName,
                contractHashStr);

            FileInfo destinationLocation = new FileInfo(
                $"{temporaryAssemblyLocation.FullName}\\{destinationFilename}");

            this.loggingProvider.Info(
                $"Destination for new stub assembly is: " +
                $"\"{destinationLocation.FullName}\".");

            toReturn =
                this.stubAssemblyGenerator.Create<DatabaseContractType>(
                    destinationLocation,
                    contractMethodInformations);

            this.loggingProvider.Info(
                $"Returning {nameof(Assembly)} -> \"{toReturn.FullName}\".");

            return toReturn;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubAssemblyManager.GetValidStubAssembly(string)" />. 
        /// </summary>
        /// <param name="contractHashStr">
        /// A hash of the database contract to look for.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Assembly" /> if found, otherwise null. 
        /// </returns>
        public Assembly GetValidStubAssembly(string contractHashStr)
        {
            Assembly toReturn = null;

            string searchFilename = string.Format(
                TemporaryStubAssemblyName,
                contractHashStr);

            FileInfo fileInfo = new FileInfo(
                $"{this.temporaryAssemblyLocation.FullName}\\{searchFilename}");

            this.loggingProvider.Debug(
                $"Looking for cached stub assembly at " +
                $"\"{fileInfo.FullName}\"...");

            if (fileInfo.Exists)
            {
                this.loggingProvider.Info(
                    $"\"{fileInfo.FullName}\" exists. Attempting to load as " +
                    $"{nameof(Assembly)}...");

                toReturn = Assembly.LoadFile(fileInfo.FullName);

                this.loggingProvider.Info(
                    $"Existing stub assembly loaded: " +
                    $"\"{toReturn.FullName}\".");
            }
            else
            {
                this.loggingProvider.Info(
                    $"No file at \"{fileInfo.FullName}\". Returning null.");
            }

            return toReturn;
        }
    }
}