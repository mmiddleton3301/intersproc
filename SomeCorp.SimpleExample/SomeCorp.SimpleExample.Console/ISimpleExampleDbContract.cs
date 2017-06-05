﻿// ----------------------------------------------------------------------------
// <copyright
//      file="ISimpleExampleDbContract.cs"
//      company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace SomeCorp.SimpleExample.Console
{
    using System;
    using Meridian.InterSproc;

    /// <summary>
    /// Describes the stored procedures available in the <c>SimpleExample</c>
    /// database.
    /// </summary>
    [InterSprocContract(Prefix = "usp_")]
    public interface ISimpleExampleDbContract
    {
        /// <summary>
        /// Executes the stored procedure <c>dbo.Create_Employee</c>.
        /// </summary>
        /// <param name="managerId">
        /// Provides the parameter <c>@ManagerId</c>.
        /// </param>
        /// <param name="hireDate">
        /// Provides the parameter <c>@HireDate</c>.
        /// </param>
        /// <param name="firstName">
        /// Provides the parameter <c>@FirstName</c>.
        /// </param>
        /// <param name="lastName">
        /// Provides the parameter <c>@LastName</c>.
        /// </param>
        /// <param name="email">
        /// Provides the parameter <c>@Email</c>.
        /// </param>
        /// <param name="homeBased">
        /// Provides the parameter <c>@HomeBased</c>.
        /// </param>
        /// <returns>
        /// A single instance of <see cref="Model.Create_EmployeeResult" />. 
        /// </returns>
        Model.Create_EmployeeResult Create_Employee(
            int? managerId,
            DateTime hireDate,
            string firstName,
            string lastName,
            string email,
            bool homeBased);

        /// <summary>
        /// Executes the stored procedure <c>dbo.Read_Employee</c>.
        /// </summary>
        /// <param name="id">
        /// Provides the parameter <c>@Id</c>.
        /// </param>
        /// <param name="managerId">
        /// Provides the parameter <c>@ManagerId</c>.
        /// </param>
        /// <param name="firstName">
        /// Provides the parameter <c>@FirstName</c>.
        /// </param>
        /// <param name="lastName">
        /// Provides the parameter <c>@LastName</c>.
        /// </param>
        /// <returns>
        /// An array of <see cref="Model.Create_EmployeeResult" /> instances. 
        /// </returns>
        Model.Read_EmployeeResult[] Read_Employee(
            int? id,
            int? managerId,
            string firstName,
            string lastName);

        /// <summary>
        /// Executes the stored procedure <c>dbo.Update_Employee</c>.
        /// </summary>
        /// <param name="id">
        /// Provides the parameter <c>@Id</c>.
        /// </param>
        /// <param name="email">
        /// Provides the parameter <c>@Email</c>.
        /// </param>
        /// <param name="homeBased">
        /// Provides the parameter <c>@HomeBased</c>.
        /// </param>
        void Update_Employee(
            int id,
            string email,
            bool? homeBased);

        /// <summary>
        /// Executes the stored procedure <c>dbo.Delete_Employee</c>.
        /// </summary>
        /// <param name="id">
        /// Provides the parameter <c>@Id</c>.
        /// </param>
        void Delete_Employee(
            int id);
    }
}