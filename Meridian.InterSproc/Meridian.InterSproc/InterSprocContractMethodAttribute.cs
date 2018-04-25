// ----------------------------------------------------------------------------
// <copyright file="InterSprocContractMethodAttribute.cs" company="MTCS">
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
    /// Can only be used on method declarations.
    /// Allows the <see cref="Name" />,
    /// <see cref="InterSprocBaseAttribute.Schema" /> and
    /// <see cref="InterSprocBaseAttribute.Prefix" /> to be overridden on an
    /// individual stored procedure-level basis.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterSprocContractMethodAttribute : InterSprocBaseAttribute
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