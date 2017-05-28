// ----------------------------------------------------------------------------
// <copyright file="CustomStubRegistry.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Reflection;
    using StructureMap;

    /// <summary>
    /// A custom <c>StructureMap</c> <see cref="Registry" />, used in
    /// activating instances in generated stub assemblies. 
    /// </summary>
    public class CustomStubRegistry : Registry
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