// ----------------------------------------------------------------------------
// <copyright
//      file="InterSprocAttributeBase.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;

    /// <summary>
    /// Abstract base type for all custom <c>InterSproc</c> attributes.
    /// </summary>
    public abstract class InterSprocAttributeBase : Attribute
    {
        /// <summary>
        /// Gets or sets an optional prefix for the stored procedure name.
        /// </summary>
        public string Prefix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the schema that the stored procedure belongs to.
        /// </summary>
        public string Schema
        {
            get;
            set;
        }
    }
}