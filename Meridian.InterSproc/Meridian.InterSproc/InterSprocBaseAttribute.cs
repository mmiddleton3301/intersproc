// ----------------------------------------------------------------------------
// <copyright file="InterSprocBaseAttribute.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System;

    /// <summary>
    /// Abstract base type for all custom <c>InterSproc</c> attributes.
    /// </summary>
    public abstract class InterSprocBaseAttribute : Attribute
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