// ----------------------------------------------------------------------------
// <copyright file="Registry.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using StructureMap.Graph;

    /// <summary>
    /// Local registry implementation, inheriting from
    /// <see cref="StructureMap.Registry" />.
    /// Used by the main (static) entry point to the <c>InterSproc</c> class to
    /// create instances for hosts without DI.
    /// </summary>
    public class Registry : StructureMap.Registry
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Registry" /> class.
        /// </summary>
        public Registry()
        {
            this.Scan(DoScan);
        }

        /// <summary>
        /// Performs a scan to automatically map interfaces to concrete types.
        /// </summary>
        /// <param name="assemblyScanner">
        /// An instance of <see cref="IAssemblyScanner" />.
        /// </param>
        private static void DoScan(IAssemblyScanner assemblyScanner)
        {
            // Always create concrete instances based on usual DI naming
            // convention
            // i.e. Search for class name "Concrete" when "IConcrete" is
            //      requested.
            assemblyScanner.WithDefaultConventions();

            // Scan the calling assembly - refer to the Registry type to
            // be explicit about it being *this* assembly.
            assemblyScanner.AssemblyContainingType<Registry>();
        }
    }
}