# InterSproc
[![Build status](https://ci.appveyor.com/api/projects/status/2j8ua2qxnhowajhp?svg=true)](https://ci.appveyor.com/project/mmiddleton3301/intersproc)

InterSproc is a C# class library used to provide simple and clean access to an SQL Server database's stored procedure layer.

## Quick Start Guide
1. **Include the NuGet package:**
   
   `Install-Package Meridian.InterSproc `
   
   
2. **Create an interface to describe your stored procedures.**
   
   This is called the **Database Contract**.
   
   For each stored procedure, write a matching method signature. For an "out of the box" experience, ensure that your method signure names match exactly the names of your stored procedures. This is not a requirement however, through the use of the `InterSprocContractMethodAttribute`.
   
   The arguments of the method signatures should match exactly the parameters of your stored procedure.
   
   Make sure that your interface is `public`.
   
   For example, assume you have the following stored procedure:
   
   `dbo.Read_Employee @Id INT, @Email NVARCHAR(256), @ApproverId INT`
   
   Your method signature would be:
   
   `Employee[] Read_Employee(int id, string email, int? approverId);`
   
   Alternatively, if your stored procedure is guarenteed to return a single result (or null), simply drop the array declaration:
   
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
    public class Company
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
   
   
5. **Use your methods!**

   Simply call the methods on your stub like you would any other class to interact with your database through your stored procedures.
   
   
## Example Project
I will be uploading an example project at some point in the future to demonstrate how to integrate with InterSproc and make use of some of its other features.
