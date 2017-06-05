// ----------------------------------------------------------------------------
// <copyright file="Read_EmployeeResult.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace SomeCorp.SimpleExample.Console.Model
{
    using System;

    /// <summary>
    /// The return type used by
    /// <see cref="ISimpleExampleDbContract.Read_Employee(int?, int?, string, string)" />. 
    /// </summary>
    public class Read_EmployeeResult
    {
        /// <summary>
        /// Gets or sets the <c>Id</c> column.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>ManagerId</c> column.
        /// </summary>
        public int? ManagerId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>HireDate</c> column.
        /// </summary>
        public DateTime HireDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>FirstName</c> column.
        /// </summary>
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>LastName</c> column.
        /// </summary>
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>Email</c> column.
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <c>HomeBased</c> column
        /// is true or not.
        /// </summary>
        public bool HomeBased
        {
            get;
            set;
        }
    }
}
