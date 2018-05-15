namespace Meridian.InterSproc.Tests.Infrastructure.ExampleContracts
{
    using Meridian.InterSproc.Tests.Infrastructure.Models;
    using System.Collections.Generic;

    public interface IEmployeeContract
    {
        Read_EmployeeResult GetEmployeeById(int id);

        IEnumerable<Read_EmployeeResult> SearchEmployees(
            string firstName,
            string lastName);

        int CountEmployees(
            string firstName,
            string lastName);

        Read_EmployeeResult[] GetManagerEmployees(int managerId);
    }
}