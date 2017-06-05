// ----------------------------------------------------------------------------
// <copyright file="Create_EmployeeResult.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace SomeCorp.SimpleExample.Console.Model
{
    /// <summary>
    /// The return type used by
    /// <see cref="ISimpleExampleDbContract.Create_Employee(int?, System.DateTime, string, string, string, bool)" />. 
    /// </summary>
    public class Create_EmployeeResult
    {
        /// <summary>
        /// Gets or sets the <c>Id</c> column.
        /// </summary>
        public int Id
        {
            get;
            set;
        }
    }
}
