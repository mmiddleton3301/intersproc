// ----------------------------------------------------------------------------
// <copyright file="InterSprocContractAttribute.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Can only be used on interfaces/data contracts.
    /// Allows for the specification of the
    /// <see cref="InterSprocBaseAttribute.Schema" /> and/or the naming
    /// <see cref="InterSprocBaseAttribute.Prefix" /> to be applied across
    /// all stored procedures mapped by the interface.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class InterSprocContractAttribute : InterSprocBaseAttribute
    {
        // Inherits everything it needs.
    }
}