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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Meridian.InterSproc.Model;

    /// <summary>
    /// Implements <see cref="IStubAssemblyManager" />.
    /// </summary>
    public class StubAssemblyManager : IStubAssemblyManager
    {
        private const string TemporaryStubAssemblyName =
            "Temporary_{0}.isa";

        private readonly IAssemblyWrapperFactory assemblyWrapperFactory;
        private readonly IFileInfoWrapperFactory fileInfoWrapperFactory;
        private readonly ILoggingProvider loggingProvider;
        private readonly IStubAssemblyGenerator stubAssemblyGenerator;
        private readonly IDirectoryInfoWrapper temporaryAssemblyLocation;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="StubAssemblyManager" /> class.
        /// </summary>
        /// <param name="assemblyWrapperFactory">
        /// An instance of type <see cref="IAssemblyWrapperFactory" />.
        /// </param>
        /// <param name="directoryInfoWrapperFactory">
        /// An instance of type <see cref="IDirectoryInfoWrapperFactory" />.
        /// </param>
        /// <param name="fileInfoWrapperFactory">
        /// An instance of type <see cref="IFileInfoWrapperFactory" />.
        /// </param>
        /// <param name="loggingProvider">
        /// An instance of type <see cref="ILoggingProvider" />.
        /// </param>
        /// <param name="stubAssemblyGenerator">
        /// An instance of type <see cref="IStubAssemblyGenerator" />.
        /// </param>
        public StubAssemblyManager(
            IAssemblyWrapperFactory assemblyWrapperFactory,
            IDirectoryInfoWrapperFactory directoryInfoWrapperFactory,
            IFileInfoWrapperFactory fileInfoWrapperFactory,
            ILoggingProvider loggingProvider,
            IStubAssemblyGenerator stubAssemblyGenerator)
        {
            IAssemblyWrapper executing =
                assemblyWrapperFactory.GetExecutingAssembly();

            IFileInfoWrapper fileInfoWrapper = fileInfoWrapperFactory
                .Create(executing.Location);

            string parentDirectoryPath = fileInfoWrapper.ParentDirectoryPath;
            this.temporaryAssemblyLocation =
                directoryInfoWrapperFactory.Create(parentDirectoryPath);

            this.assemblyWrapperFactory = assemblyWrapperFactory;
            this.fileInfoWrapperFactory = fileInfoWrapperFactory;
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

            IEnumerable<IFileInfoWrapper> temporaryAssemblies =
                this.temporaryAssemblyLocation.GetFiles(wildcardAssem);

            this.loggingProvider.Info(
                $"{temporaryAssemblies.Count()} temporary assemblies found. " +
                $"Deleting each file in turn...");

            IFileInfoWrapper sourceFileSearch = null;
            foreach (IFileInfoWrapper fileInfoWrapper in temporaryAssemblies)
            {
                this.loggingProvider.Debug(
                    $"Deleting \"{fileInfoWrapper.FullName}\"...");

                // Unload it first - if it's in the bin dir, then it'll get
                // loaded by the host app by default.
                fileInfoWrapper.Delete();

                this.loggingProvider.Info(
                    $"Deleted \"{fileInfoWrapper.FullName}\". Searching for " +
                    $"corresponding source file...");

                sourceFileSearch = this.fileInfoWrapperFactory
                    .Create($"{fileInfoWrapper.FullName}.cs");

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
        /// <see cref="IStubAssemblyManager.GenerateStubAssembly{TDatabaseContractType}(string, IEnumerable{ContractMethodInformation})" />.
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
        /// An instance of <see cref="IAssemblyWrapper" />.
        /// </returns>
        public IAssemblyWrapper GenerateStubAssembly<TDatabaseContractType>(
            string contractHashStr,
            IEnumerable<ContractMethodInformation> contractMethodInformations)
            where TDatabaseContractType : class
        {
            IAssemblyWrapper toReturn = null;

            this.loggingProvider.Debug(
                "Constructing destination filename for new Stub Assembly...");

            string destinationFilename = string.Format(
                CultureInfo.InvariantCulture,
                TemporaryStubAssemblyName,
                contractHashStr);

            string destinationLocationStr =
                $"{this.temporaryAssemblyLocation.FullName}\\" +
                $"{destinationFilename}";

            IFileInfoWrapper destinationLocation =
                this.fileInfoWrapperFactory.Create(destinationLocationStr);

            this.loggingProvider.Info(
                $"Destination for new stub assembly is: " +
                $"\"{destinationLocation.FullName}\".");

            toReturn =
                this.stubAssemblyGenerator.Create<TDatabaseContractType>(
                    destinationLocation,
                    contractMethodInformations);

            this.loggingProvider.Info(
                $"Returning {nameof(IAssemblyWrapper)} -> " +
                $"\"{toReturn.FullName}\".");

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
        /// An instance of type <see cref="IAssemblyWrapper" /> if found,
        /// otherwise null.
        /// </returns>
        public IAssemblyWrapper GetValidStubAssembly(string contractHashStr)
        {
            IAssemblyWrapper toReturn = null;

            string searchFilename = string.Format(
                CultureInfo.InvariantCulture,
                TemporaryStubAssemblyName,
                contractHashStr);

            string requiredStubAssemblyPath =
                $"{this.temporaryAssemblyLocation.FullName}\\{searchFilename}";
            IFileInfoWrapper fileInfoWrapper =
                this.fileInfoWrapperFactory.Create(requiredStubAssemblyPath);

            this.loggingProvider.Debug(
                $"Looking for cached stub assembly at " +
                $"\"{fileInfoWrapper.FullName}\"...");

            if (fileInfoWrapper.Exists)
            {
                this.loggingProvider.Info(
                    $"\"{fileInfoWrapper.FullName}\" exists. Attempting to " +
                    $"load as {nameof(IAssemblyWrapper)}...");

                toReturn = this.assemblyWrapperFactory
                    .LoadFile(fileInfoWrapper.FullName);

                this.loggingProvider.Info(
                    $"Existing stub assembly loaded: " +
                    $"\"{toReturn.FullName}\".");
            }
            else
            {
                this.loggingProvider.Info(
                    $"No file at \"{fileInfoWrapper.FullName}\". Returning " +
                    $"null.");
            }

            return toReturn;
        }
    }
}