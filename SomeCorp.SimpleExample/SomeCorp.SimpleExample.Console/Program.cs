﻿// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="MTCS (Matt Middleton)">
// Copyright (c) Meridian Technology Consulting Services (Matt Middleton).
// All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace SomeCorp.SimpleExample.Console
{
    using System;
    using System.Linq;
    using Meridian.InterSproc;

    /// <summary>
    /// Contains the main entry point to the application,
    /// <see cref="Program.Main()" />. 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The id of the director, in the <c>SimpleExample</c> database.
        /// </summary>
        private const int DirectorId = 88;

        /// <summary>
        /// Place your connection string in this variable.
        /// </summary>
        private const string ConnStr =
            "internal=(local);" +
            "Initial Catalog=SimpleExample;" +
            "Persist Security Info=True;" +
            "User ID=sa;" +
            "Password=passwordGoesHere";

        /// <summary>
        /// The main entry point to the application.
        /// </summary>
        private static void Main()
        {
            ISimpleExampleDbContract stub =
                SprocStubFactory.Create<ISimpleExampleDbContract>(ConnStr);

            // Read the director.
            Model.Read_EmployeeResult theDirector =
                stub.Read_Employee(DirectorId, null, null, null)
                .Single();

            // Then the management team.
            Model.Read_EmployeeResult[] managementTeam =
                stub.Read_Employee(null, theDirector.Id, null, null);

            // Create a new employee.
            Model.Create_EmployeeResult newEmployee =
                stub.Create_Employee(
                    theDirector.Id,
                    DateTime.UtcNow,
                    "Jane",
                    "Bloggs",
                    "jane.bloggs@somecorp.co.uk",
                    false);

            string newEmail =
                $"{theDirector.FirstName}.{theDirector.LastName}@" +
                $"somecorp.co.uk"
                .ToLower();

            // Update an existing employee.
            stub.Update_Employee(
                theDirector.Id,
                newEmail,
                null);

            theDirector = stub.Read_Employee(DirectorId, null, null, null)
                .Single();

            // Delete an employee.
            stub.Delete_Employee(9);

            Model.Read_EmployeeResult[] search = stub.Read_Employee(
                9,
                null,
                null,
                null);

            search = stub.Read_Employee(
                null,
                null,
                "Max",
                null);
        }
    }
}