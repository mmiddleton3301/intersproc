// ----------------------------------------------------------------------------
// <copyright file="StubAssemblyManager.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IStubAssemblyManager" />.
    /// </summary>
    public class StubAssemblyManager : IStubAssemblyManager
    {
        private const string TemporaryStubAssemblyName =
            "Temporary_{0}.isa";

        private readonly ILoggingProvider loggingProvider;
        private readonly IStubAssemblyGenerator stubAssemblyGenerator;
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
                new FileInfo(assemlyExecutionLocation).Directory;

            this.loggingProvider = loggingProvider;
            this.stubAssemblyGenerator = stubAssemblyGenerator;
        }

        /// <summary>
        /// Implements
        /// <see cref="IStubAssemblyManager.CleanupTemporaryAssemblies()" />.
        /// </summary>
        public void CleanupTemporaryAssemblies()
        {
            string wildcardAssem = string.Format(
                CultureInfo.InvariantCulture,
                TemporaryStubAssemblyName,
                "*");

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
        /// <see cref="IStubAssemblyManager.GenerateStubAssembly{TDatabaseContractType}(string, ContractMethodInformation[])" />.
        /// </summary>
        /// <typeparam name="TDatabaseContractType">
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
        public Assembly GenerateStubAssembly<TDatabaseContractType>(
            string contractHashStr,
            ContractMethodInformation[] contractMethodInformations)
            where TDatabaseContractType : class
        {
            Assembly toReturn = null;

            this.loggingProvider.Debug(
                "Constructing destination filename for new Stub Assembly...");

            string destinationFilename = string.Format(
                CultureInfo.InvariantCulture,
                TemporaryStubAssemblyName,
                contractHashStr);

            FileInfo destinationLocation = new FileInfo(
                $"{this.temporaryAssemblyLocation.FullName}\\{destinationFilename}");

            this.loggingProvider.Info(
                $"Destination for new stub assembly is: " +
                $"\"{destinationLocation.FullName}\".");

            toReturn =
                this.stubAssemblyGenerator.Create<TDatabaseContractType>(
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
                CultureInfo.InvariantCulture,
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