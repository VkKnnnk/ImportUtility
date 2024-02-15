using System.Collections.Generic;

#nullable disable

namespace ImportUtility.Model
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
            InverseIdParentDepartmentNavigation = new HashSet<Department>();
        }

        public int IdDepartment { get; set; }
        public string Title { get; set; }
        public int? IdParentDepartment { get; set; }
        public int? IdDirector { get; set; }
        public string Phone { get; set; }

        public virtual Employee IdDirectorNavigation { get; set; }
        public virtual Department IdParentDepartmentNavigation { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Department> InverseIdParentDepartmentNavigation { get; set; }
    }
}
