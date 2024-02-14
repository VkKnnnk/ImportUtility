using ImportUtility.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportUtility.View_Model
{
    public class ImportFunctions
    {
        #region Constants
        private const string TABLE_D = "d";
        private const string TABLE_E = "e";
        private const string TABLE_P = "p";
        #endregion
        #region Properties
        private static Dictionary<string, Department> DepartmentsDict { get; set; }
        private static Dictionary<string, Employee> EmployeesDict { get; set; }
        private static Dictionary<string, Position> PositionsDict { get; set; }
        #endregion
        #region Parse methods
        public static void ParseDataFromFile(string filename, string type)
        {
            string parsePattern;
            string groupPattern;

            if (!File.Exists(filename))
                throw new FileNotFoundException($"Файла `{filename}` не существует");

            using (UnkCompanyDBContext dBContext = new UnkCompanyDBContext())
            {
                if (!dBContext.Database.CanConnect())
                    throw new Exception("Отсутствует подключение к базе данных");

                switch (type)
                {
                    case TABLE_D:
                        {
                            DepartmentsDict = dBContext.Departments.ToDictionary(x => x.Title.ToLower());
                            EmployeesDict = dBContext.Employees.ToDictionary(x => x.Fullname.ToLower());

                            groupPattern = "$1\t$2\t$3\t$4";
                            parsePattern = @"(\S[-\w ]+\S) *\t *([-\w ]+\S)? *\t *([-\w ]+\S)? *\t *([-\d ()]+\S)";
                            break;
                        }
                    case TABLE_E:
                        {
                            DepartmentsDict = dBContext.Departments.ToDictionary(x => x.Title.ToLower());
                            PositionsDict = dBContext.Positions.ToDictionary(x => x.Title.ToLower());

                            groupPattern = "$1\t$2\t$3\t$4\t$5";
                            parsePattern = @"(\S[-\w ]+\S)? *\t *([-\w ]+\S) *\t *([^\t]+) *\t *([^\t]+) *\t *([-\w ]+\S)";
                            break;
                        }
                    case TABLE_P:
                        {
                            groupPattern = "$1";
                            parsePattern = @"(\S[-\w ]+\S)";
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(type);
                }

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new(fs))
                    {
                        sr.ReadLine();
                        while (!sr.EndOfStream)
                        {
                            string data = sr.ReadLine();
                            if (String.IsNullOrWhiteSpace(data))
                                continue;

                            Match parseMatch = Regex.Match(data, parsePattern);
                            if (!parseMatch.Success)
                                continue;

                            string[] parsedData = Regex.Replace(
                                Regex.Replace(parseMatch.Value, parsePattern, groupPattern),
                                @" +", " ").Split('\t');

                            object mappedData = MapDataToIds(parsedData, type);
                            if (mappedData is null)
                                continue;

                            UpdateOrAddDBData(mappedData, type, dBContext);
                        }
                    }
                }
                DisplayDepartments(dBContext);
                DisplayEmployees(dBContext);
                DisplayPositions(dBContext);

                dBContext.SaveChanges();
            }
        }

        private static object MapDataToIds(string[] data, string type)
        {
            switch (type)
            {
                case TABLE_D:
                    {
                        int? parsedIdParrent = null;
                        int? parsedIdDirector = null;
                        if (!string.IsNullOrEmpty(data[1]))
                            if (DepartmentsDict.ContainsKey(data[1].ToLower()))
                                parsedIdParrent = DepartmentsDict[data[1].ToLower()].IdDepartment;
                            else
                                return null;

                        if (EmployeesDict.ContainsKey(data[2].ToLower()))
                            parsedIdDirector = EmployeesDict[data[2].ToLower()].IdEmployee;

                        return new Department
                        {
                            Title = data[0],
                            IdParentDepartment = parsedIdParrent,
                            IdDirector = parsedIdDirector,
                            Phone = data[3]
                        };
                    }
                case TABLE_E:
                    {
                        int? parsedIdDepartment = null;
                        if (PositionsDict.ContainsKey(data[4].ToLower()))
                            data[4] = PositionsDict[data[4].ToLower()].IdPosition.ToString();
                        else
                            return null;

                        if (DepartmentsDict.ContainsKey(data[0].ToLower()))
                            parsedIdDepartment = DepartmentsDict[data[0].ToLower()].IdDepartment;

                        return new Employee
                        {
                            IdDepartment = parsedIdDepartment,
                            Fullname = data[1],
                            Login = data[2],
                            Password = data[3],
                            IdPosition = int.Parse(data[4])
                        };
                    }
                case TABLE_P:
                    {
                        return new Position { Title = data[0] };
                    }
            }
            return null;
        }
        private static void UpdateOrAddDBData(object data, string type, UnkCompanyDBContext dBContext)
        {
            switch (type)
            {
                case TABLE_D:
                    {
                        List<Department> contextDepartments = dBContext.Departments.ToList();
                        Department departmentFromData = data as Department;

                        if (contextDepartments.Any(x =>
                            x.Title.ToLower() == departmentFromData.Title.ToLower() &&
                            x.IdParentDepartment == departmentFromData.IdParentDepartment))
                        {
                            Department contextDepartment = contextDepartments.
                                FirstOrDefault(x => x.Title.ToLower() == departmentFromData.Title.ToLower() &&
                                x.IdParentDepartment == departmentFromData.IdParentDepartment);

                            if (contextDepartment.Phone != departmentFromData.Phone)
                                contextDepartment.Phone = departmentFromData.Phone;

                            else if (contextDepartment.IdDirector != departmentFromData.IdDirector)
                                contextDepartment.IdDirector = departmentFromData.IdDirector;
                        }
                        else
                            dBContext.Departments.Add(departmentFromData);
                        break;
                    }
                case TABLE_E:
                    {
                        List<Employee> contextEmployees = dBContext.Employees.ToList();
                        Employee employeeFromData = data as Employee;

                        if (contextEmployees.Any(x => x.Fullname.ToLower() == employeeFromData.Fullname.ToLower()))
                        {
                            Employee contextEmployee = contextEmployees.
                                FirstOrDefault(x => x.Fullname.ToLower() == employeeFromData.Fullname.ToLower());

                            if (contextEmployee.IdDepartment != employeeFromData.IdDepartment)
                                contextEmployee.IdDepartment = employeeFromData.IdDepartment;

                            if (contextEmployee.Login != employeeFromData.Login)
                                contextEmployee.Login = employeeFromData.Login;

                            if (contextEmployee.Password != employeeFromData.Password)
                                contextEmployee.Password = employeeFromData.Password;

                            if (contextEmployee.IdPosition != employeeFromData.IdPosition)
                                contextEmployee.IdPosition = employeeFromData.IdPosition;
                        }
                        else
                            dBContext.Employees.Add(employeeFromData);
                        break;
                    }
                case TABLE_P:
                    {
                        List<Position> contextPositions = dBContext.Positions.ToList();
                        Position positionFromData = data as Position;

                        if (!contextPositions.Any(x => x.Title.ToLower() == positionFromData.Title.ToLower()))
                            dBContext.Positions.Add(new Position { Title = positionFromData.Title });
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(type);
            }
        }
        #endregion
        #region Display methods
        public static void DisplayDepartments(UnkCompanyDBContext dBContext)
        {
            dBContext.Employees.Load();
            List<Department> departments = dBContext.Departments.ToList();
            var changesDepartment = dBContext.ChangeTracker.Entries<Department>();

            var modifiedDepartmentsDict = changesDepartment.
                Where(x => x.State == EntityState.Modified).
                Select(x => x.Entity).ToDictionary(x => x.IdDepartment);

            var addedDepartments = changesDepartment.
                Where(x => x.State == EntityState.Added).
                Select(x => x.Entity).ToList();

            string displayFormat = "{0,-5} {1,-20} {2,-20} {3,-30} {4,-20}";
            Console.WriteLine("\nТаблица Подразделения");
            Console.WriteLine(displayFormat, "ID", "Название", "Родитель", "Директор", "Телефон");
            foreach (Department department in departments)
            {
                string parent = "NULL";
                if (department.IdParentDepartmentNavigation is not null)
                    parent = department.IdParentDepartmentNavigation.Title;

                string director = "NULL";
                if (department.IdDirectorNavigation is not null)
                    director = department.IdDirectorNavigation.Fullname;

                if (modifiedDepartmentsDict.ContainsKey(department.IdDepartment))
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(displayFormat, department.IdDepartment,
                    department.Title, parent, director, department.Phone);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Department addedDepartment in addedDepartments)
            {
                string parent = "NULL";
                if (addedDepartment.IdParentDepartmentNavigation is not null)
                    parent = addedDepartment.IdParentDepartmentNavigation.Title;

                string director = "NULL";
                if (addedDepartment.IdDirectorNavigation is not null)
                    director = addedDepartment.IdDirectorNavigation.Fullname;

                Console.WriteLine(displayFormat, addedDepartment.IdDepartment,
                    addedDepartment.Title, parent, director, addedDepartment.Phone);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void DisplayEmployees(UnkCompanyDBContext dBContext)
        {
            dBContext.Departments.Load();
            dBContext.Positions.Load();
            List<Employee> employees = dBContext.Employees.ToList();

            var changesEmployee = dBContext.ChangeTracker.Entries<Employee>();
            var modifiedEmployeesDict = changesEmployee.
                Where(x => x.State == EntityState.Modified).
                Select(x => x.Entity).ToDictionary(x => x.IdEmployee);

            var addedEmployees = changesEmployee.
                Where(x => x.State == EntityState.Added).
                Select(x => x.Entity).ToList();

            string displayFormat = "{0,-5} {1,-20} {2,-30} {3,-10} {4,-20} {5, -20}";
            Console.WriteLine("\nТаблица Сотрудники");
            Console.WriteLine(displayFormat, "ID", "Подразделение", "ФИО", "Логин", "Пароль", "Должность");
            foreach (Employee employee in employees)
            {
                string department = "NULL";
                if (employee.IdDepartmentNavigation is not null)
                    department = employee.IdDepartmentNavigation.Title;

                string position = "NULL";
                if (employee.IdPositionNavigation is not null)
                    position = employee.IdPositionNavigation.Title;

                if (modifiedEmployeesDict.ContainsKey(employee.IdEmployee))
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(displayFormat,
                    employee.IdEmployee, department, employee.Fullname,
                    employee.Login, employee.Password, position);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Employee addedEmployee in addedEmployees)
            {
                string department = "NULL";
                if (addedEmployee.IdDepartmentNavigation is not null)
                    department = addedEmployee.IdDepartmentNavigation.Title;

                string position = "NULL";
                if (addedEmployee.IdPositionNavigation is not null)
                    position = addedEmployee.IdPositionNavigation.Title;

                Console.WriteLine(displayFormat,
                    addedEmployee.IdEmployee, department, addedEmployee.Fullname,
                    addedEmployee.Login, addedEmployee.Password, position);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void DisplayPositions(UnkCompanyDBContext dBContext)
        {
            List<Position> positions = dBContext.Positions.ToList();

            var changesPosition = dBContext.ChangeTracker.Entries();
            var addedPositions = changesPosition.
                Where(x => x.State == EntityState.Added).
                Select(x => x.Entity).ToList();

            string displayFormat = "{0,-5} {1,-20}";
            Console.WriteLine("\nТаблица Должности");
            Console.WriteLine(displayFormat, "ID", "Название");
            foreach (Position position in positions)
            {
                Console.WriteLine(displayFormat, position.IdPosition, position.Title);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Position addedPosition in addedPositions)
            {
                Console.WriteLine(displayFormat, addedPosition.IdPosition, addedPosition.Title);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void DisplayDepartmentsAsHierarchy()
        {

        }
        #endregion
    }
}