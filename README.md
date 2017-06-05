# InterSproc
[![Build status](https://ci.appveyor.com/api/projects/status/2j8ua2qxnhowajhp?svg=true)](https://ci.appveyor.com/project/mmiddleton3301/intersproc)

InterSproc is a C# class library used to provide simple and clean access to an SQL Server database's stored procedure layer.

## Why "InterSproc" and not $INSERT_ORM_HERE$ (e.g. Entity Framework, NHibernate, etc)?
InterSproc is for stored procedures only. InterSproc is **not** an ORM.

The argument as to whether ORMs (Object-Relational Mapping frameworks) act as a good foundation to new projects is as old as time itself - and I'm not going to turn this README into an essay describing the pros and cons of each side of the argument (maybe one day I'll start a blog?).

If, however, you are a developer like myself, and you still like to separate your .NET binaries from your tables through a layer of Stored Procedures, then InterSproc may be for you.

InterSproc is a lightweight class library used to provide simple and clean access to an SQL Server database's stored procedures. All you need to do is create an interface describing your stored procedures, and InterSproc does the rest.

## Quick Start Guide
1. **Include the NuGet package:**
   
   `Install-Package Meridian.InterSproc `
   
   
2. **Create an interface to describe your stored procedures.**
   
   This is called the **Database Contract**.
   
   For each stored procedure, write a matching method signature. For an "out of the box" experience, ensure that your method signature names match exactly the names of your stored procedures. This is not a requirement however, through the use of the `InterSprocContractMethodAttribute`, this behaviour can be overridden.
   
   The arguments of the method signatures should match exactly the parameters of your stored procedure.
   
   Make sure that your interface is `public`.
   
   For example, assume you have the following stored procedure:
   
   `dbo.Read_Employee @Id INT, @Email NVARCHAR(256), @ApproverId INT`
   
   Your method signature would be:
   
   `Employee[] Read_Employee(int id, string email, int? approverId);`
   
   Alternatively, if your stored procedure is guaranteed to return a single result (or null), simply drop the array declaration:
   
   `Employee[] Read_Employee(int id);`
   
   If your stored procedure returns no results, simply declare your function's return type as `void`:
   
   `void Update_Employee(int id, string email);`
   
3. **Create your return types**
    
    Your return type models simply need to contain properties that match the columns of the result set returned by the stored procedure.
    
    For example, assume your stored procedure returns the following columns:
    
    ```
    Id          Email
    ----------- ------------------
    3           bob@smith.com
    4           nobody@example.com
    5           boss@smith.com
    6           joe@smith.com
    ```
    
    Then your corresponding model needs to be structured as such:
    
    ```
    public class Employee
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
    ```
    
    
4. **Create a stub instance from your contract**
   
   Simply call the static `Create` method on `SprocStubFactory` to create your instance.
   
   ```
   IEmployeeContract databaseStub =
     Meridian.InterSproc.SprocStubFactory.Create<IEmployeeContract>(
        Properties.Settings.Default.ConnStr);
   ```
   
   
5. **Use your methods to access your stored procedures!**

   Simply call the methods on your stub like you would any other class to interact with your database through your stored procedures.
   
   
## Example Project
The [SomeCorp SimpleExample console applicaiton and database project](https://github.com/mmiddleton3301/intersproc/tree/master/SomeCorp.SimpleExample) is available for cloning, and provides examples/a sandbox in which to get used to the InterSproc library.
