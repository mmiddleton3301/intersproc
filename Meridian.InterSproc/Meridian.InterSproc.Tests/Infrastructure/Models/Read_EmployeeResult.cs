namespace Meridian.InterSproc.Tests.Infrastructure.Models
{
    using System;

    public class Read_EmployeeResult
    {
        public int Id
        {
            get;
            set;
        }

        public int? ManagerId
        {
            get;
            set;
        }

        public DateTime HireDate
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public bool HomeBased
        {
            get;
            set;
        }
    }
}