using System;
using System.Collections.Generic;

#nullable disable

namespace ImportUtility.Model
{
    public partial class Employee
    {
        public Employee()
        {
            Departments = new HashSet<Department>();
        }

        public int IdEmployee { get; set; }
        public int? IdDepartment { get; set; }
        public string Fullname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int IdPosition { get; set; }

        public virtual Department IdDepartmentNavigation { get; set; }
        public virtual Position IdPositionNavigation { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
