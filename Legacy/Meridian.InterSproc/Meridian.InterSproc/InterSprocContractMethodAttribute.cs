// ----------------------------------------------------------------------------
// <copyright
//      file="InterSprocContractMethodAttribute.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;

    /// <summary>
    /// Can only be used on method declarations.
    /// Allows the <see cref="Name" />,
    /// <see cref="InterSprocAttributeBase.Schema" /> and
    /// <see cref="InterSprocAttributeBase.Prefix" /> to be overridden on an
    /// individual stored procedure-level basis.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterSprocContractMethodAttribute : InterSprocAttributeBase
    {
        /// <summary>
        /// Gets or sets the actual name of the stored procedure. 
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}