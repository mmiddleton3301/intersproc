# SomeCorp SimpleExample
## Overview
This project exists to simply demonstrate how to use InterSproc on a basic level. This project does not include any N-Tiered layering or any of that good stuff - this example even injects a connection string via a constant. Therefore, it should not be taken as a good example of how develop an application ;)

It also includes an example database which you can deploy to your own machine. Follow the instructions in this readme to get this sandbox project up and running on your development environment!

## How to set up the SimpleExample solution

1. First, deploy the example database to an SQL instance.
   
   a. Ensure that the database target platform matches your development server.

      * Right-click the **SomeCorp.SimpleExample.Database** project, and click **Properties**;

      * Set the **target platform** to match your development server.
          
          **NOTE:** Although not tested with anything but the platform I used to create this project (**Microsoft Azure SQL Database**), the project should deploy on anything from SQL Server 2012 upwards.

   b. Publish the database project to your development server.
      
      Right-click the **SomeCorp.SimpleExample.Database** project, and click **Publish...**.
   
   c. Make a note of the connection string to your new database!

2. Complete the constant **SomeCorp.SimpleExample.Console.Program.ConnStr** with your own connection string.

   That should be it - (optionally) add breakpoints and execute the code.
