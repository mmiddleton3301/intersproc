// ----------------------------------------------------------------------------
// <copyright file="ContractMethodInformation.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
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
        [NonSerialized]
        private MethodInfo methodInfo;

        /// <summary>
        /// Gets or sets the original method <see cref="MethodInfo" />,
        /// describing the interface signature.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get
            {
                return this.methodInfo;
            }

            set
            {
                this.methodInfo = value;
            }
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
        /// Gets or sets a prefix to the method/stored procedure
        /// (e.g. <c>usp_</c>, <c>sp_</c>).
        /// </summary>
        public string Prefix
        {
            get;
            set;
        }

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
        /// Returns the full name of a sproc, including the schema, prefix and
        /// name.
        /// </summary>
        /// <returns>
        /// A full sproc name (e.g. <c>dbo.Read_Employees</c>.
        /// </returns>
        public string GetStoredProcedureFullName()
        {
            string toReturn = $"{this.Schema}.{this.Prefix}{this.Name}";

            return toReturn;
        }

        /// <summary>
        /// Overrides <see cref="object.ToString()" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" />  that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string toReturn = this.GetStoredProcedureFullName();

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