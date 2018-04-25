// ----------------------------------------------------------------------------
// <copyright file="CustomStubRegistry.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Reflection;

    /// <summary>
    /// A custom <c>StructureMap</c> <see cref="Registry" />, used in
    /// activating instances in generated stub assemblies.
    /// </summary>
    public class CustomStubRegistry : StructureMap.Registry
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CustomStubRegistry" />
        /// class.
        /// </summary>
        /// <param name="temporaryStubAssembly">
        /// The generated stub <see cref="Assembly" />.
        /// </param>
        public CustomStubRegistry(Assembly temporaryStubAssembly)
        {
            this.Scan((x) =>
            {
                // Just scan our temporary assembly.
                x.Assembly(temporaryStubAssembly);

                // Just register against the first implementation - there's
                // only one, after all.
                x.RegisterConcreteTypesAgainstTheFirstInterface();
            });
        }
    }
}