// ----------------------------------------------------------------------------
// <copyright
//      file="ContractMethodInformation.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc.Model
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Model designed to contain information on a database contract, derived
    /// from the structure and custom attributes that may or may not be
    /// present.
    /// </summary>
    [Serializable]
    public class ContractMethodInformation
    {
        /// <summary>
        /// Gets or sets the database schema that the method/stored procedure
        /// belongs to.
        /// </summary>
        public string Schema
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a prefix to the method/stored procedure
        /// (e.g. <c>usp_</c>, <c>sp_</c>).
        /// </summary>
        public string Prefix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the stored procedure (if different from
        /// the method).
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original method <see cref="MethodInfo" />, 
        /// describing the interface signature.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Overrides <see cref="object.ToString()" />. 
        /// </summary>
        /// <returns>
        /// A <see cref="string" />  that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string toReturn = $"{this.Schema}.{this.Prefix}{this.Name}";

            string[] paramsStr = this.MethodInfo
                .GetParameters()
                .Select(x => $"{x.ParameterType.Name} {x.Name}")
                .ToArray();

            string paramList = string.Join(", ", paramsStr);

            toReturn = $"{toReturn}({paramList})";

            return toReturn;
        }
    }
}