// ----------------------------------------------------------------------------
// <copyright file="Create_EmployeeResult.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
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
