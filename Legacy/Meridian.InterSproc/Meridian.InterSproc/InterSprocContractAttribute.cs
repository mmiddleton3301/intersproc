// ----------------------------------------------------------------------------
// <copyright
//      file="InterSprocContractAttribute.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;

    /// <summary>
    /// Can only be used on interfaces/data contracts.
    /// Allows for the specification of the
    /// <see cref="InterSprocAttributeBase.Schema" /> and/or the naming
    /// <see cref="InterSprocAttributeBase.Prefix" /> to be applied across
    /// all stored procedures mapped by the interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class InterSprocContractAttribute : InterSprocAttributeBase
    {
        // Inherits everything it needs.
    }
}