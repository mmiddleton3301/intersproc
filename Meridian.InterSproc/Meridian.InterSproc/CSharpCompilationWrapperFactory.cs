// ----------------------------------------------------------------------------
// <copyright file="CSharpCompilationWrapperFactory.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Meridian.InterSproc.Definitions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements <see cref="ICSharpCompilationWrapperFactory" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CSharpCompilationWrapperFactory
        : ICSharpCompilationWrapperFactory
    {
        /// <summary>
        /// Implements
        /// <see cref="ICSharpCompilationWrapperFactory.Create(IFileInfoWrapper, SyntaxTree, IEnumerable{IMetadataReferenceWrapper})" />.
        /// </summary>
        /// <param name="destinationLocation">
        /// An instance of type <see cref="IFileInfoWrapper" />, describing
        /// where the stub assembly will be stored.
        /// This is used in naming the new stub assembly - the new stub
        /// assembly is not actually stored to disk in this method.
        /// </param>
        /// <param name="syntaxTree">
        /// An instance of <see cref="SyntaxTree" /> containing the parsed
        /// code which will be compiled.
        /// </param>
        /// <param name="metadataReferenceWrappers">
        /// A collection of instances of type
        /// <see cref="IMetadataReferenceWrapper" /> describing the
        /// dependencies for the new stub assembly.
        /// </param>
        /// <returns>
        /// An instance of type <see cref="ICSharpCompilationWrapper" />.
        /// </returns>
        public ICSharpCompilationWrapper Create(
            IFileInfoWrapper destinationLocation,
            SyntaxTree syntaxTree,
            IEnumerable<IMetadataReferenceWrapper> metadataReferenceWrappers)
        {
            ICSharpCompilationWrapper toReturn = null;

            IEnumerable<MetadataReference> references =
                metadataReferenceWrappers.Select(x => x.MetadataReference);

            CSharpCompilation cSharpCompilation = CSharpCompilation.Create(
                destinationLocation.Name,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary));

            toReturn = new CSharpCompilationWrapper(cSharpCompilation);

            return toReturn;
        }
    }
}