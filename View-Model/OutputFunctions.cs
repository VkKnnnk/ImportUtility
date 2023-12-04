using ConsoleTables;
using ImportUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportUtility.View_Model
{
    public static class OutputFunctions
    {
        static void FindChildren(int lvl, Department department)
        {
            OutputLevelDepartments(lvl, department);
            lvl++;
            var children = Session.Context.Departments.Where(x => x.IdParentDepartment == department.IdDepartment).OrderBy(x => x.Title).ToList();
            foreach (var child in children)
            {
                FindChildren(lvl, child);
            }
        }
        static List<Department> FindParents(int lvl, Department department, List<Department> parents)
        {
            lvl++;
            Department parent = Session.Context.Departments.Where(x => x.IdDepartment == department.IdParentDepartment).FirstOrDefault();
            if (parent is not null)
            {
                parents.Add(parent);
                FindParents(lvl, parent, parents);
            }
            return parents;
        }
        static void OutputLevelDepartments(int level, Department department)
        {
            Employee director = new();
            Console.Write("=");
            for (int i = 0; i < level; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine($" {department.Title} ID={department.IdDepartment}");
            if (department.IdDirector is not null)
            {
                director = Session.Context.Employees.Where(x => x.IdEmployee == department.IdDirector).FirstOrDefault();
                for (int i = 0; i < level; i++)
                {
                    Console.Write(" ");
                }
                Position posDirector = Session.Context.Positions.Where(x => x.IdPosition == director.IdPosition).FirstOrDefault();
                Console.WriteLine($"* {director.Fullname} ID={director.IdEmployee} ({posDirector.Title} ID={posDirector.IdPosition})");
            }
            if (Session.Context.Employees.Select(x => x.IdDepartment).Contains(department.IdDepartment))
            {
                List<Employee> employees = Session.Context.Employees.Where(x => x.IdDepartment == department.IdDepartment).ToList();
                if (employees.Contains(director))
                    employees.Remove(director);
                foreach (var emp in employees)
                {
                    for (int i = 0; i < level; i++)
                    {
                        Console.Write(" ");
                    }
                    Position posEmp = Session.Context.Positions.Where(x => x.IdPosition == emp.IdPosition).FirstOrDefault();
                    Console.WriteLine($"- {emp.Fullname} ID={emp.IdEmployee} ({posEmp.Title})");
                }
            }
        }
        public static void StartOutput(int idTable)
        {
            try
            {
                switch (idTable)
                {
                    case 1:
                        Console.WriteLine("Процесс вывода запущен, пожалуйста, подождите..");
                        Task outputPosTask = Task.Run(() => OutputTablePositions());
                        outputPosTask.Wait();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Выберите, что вы хотите вывести:");
                        Console.WriteLine("1 - Таблица Подразделения");
                        Console.WriteLine("2 - Иерархический список подразделений");
                        ConsoleKey selectedTypeTableOption = Console.ReadKey().Key;
                        switch (selectedTypeTableOption)
                        {
                            case ConsoleKey.D1:
                                Console.Clear();
                                Console.WriteLine("Процесс вывода запущен, пожалуйста, подождите..");
                                Task outputDepTableTask = Task.Run(() => OutputTableDepartments());
                                outputDepTableTask.Wait();
                                break;
                            case ConsoleKey.D2:
                                Console.Clear();
                                Console.WriteLine("Выберите, что вы хотите вывести:");
                                Console.WriteLine("1 - Вывести иерархию всех подразделений");
                                Console.WriteLine("2 - Вывести родительские подразделения конкректного подразделения");
                                ConsoleKey selectedTypeListOption = Console.ReadKey().Key;
                                Console.Clear();
                                switch (selectedTypeListOption)
                                {
                                    case ConsoleKey.D1:
                                        Console.Clear();
                                        Console.WriteLine("Процесс вывода запущен, пожалуйста, подождите..\n");
                                        List<Department> mainDepartments = Session.Context.Departments.Where(x => x.IdParentDepartment == null).ToList();
                                        Task outputDepListTask;
                                        Console.Clear();
                                        Console.WriteLine("Иерархия всех подразделений");
                                        foreach (var mainDepartment in mainDepartments)
                                        {
                                            outputDepListTask = Task.Run(() => FindChildren(0, mainDepartment));
                                            outputDepListTask.Wait();
                                        }
                                        break;
                                    case ConsoleKey.D2:
                                        string inputIdDepartment = String.Empty;
                                        int number;
                                        
                                        while (!int.TryParse(inputIdDepartment, out number))
                                        {
                                            Console.WriteLine("Введите 'Id' подразделения:");
                                            inputIdDepartment = Console.ReadLine();
                                            if (int.TryParse(inputIdDepartment, out number))
                                            {
                                                if (Session.Context.Departments.ToList().Count > 0)
                                                {
                                                    if (!Session.Context.Departments.Select(x => x.IdDepartment).Contains(int.Parse(inputIdDepartment)))
                                                    {
                                                        Console.Clear();
                                                        Console.WriteLine($"Такого 'Id' {inputIdDepartment} среди подразделений нет, введите заново...");
                                                        inputIdDepartment = string.Empty;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Console.Clear();
                                                Console.WriteLine("'Id' подразделения может содержать только числа, введите заново..\n");
                                            }
                                        }
                                        Console.Clear();
                                        Console.WriteLine("Процесс вывода запущен, пожалуйста, подождите..\n");
                                        Department child = Session.Context.Departments.Where(x => x.IdDepartment == int.Parse(inputIdDepartment)).FirstOrDefault();
                                        Console.Clear();
                                        Console.WriteLine("Родители выбранного подразделения");
                                        List<Department> parents = FindParents(0, child, new List<Department>());
                                        parents.Reverse();
                                        int level = 0;
                                        foreach (var parent in parents)
                                        {
                                            level++;
                                            for (int i = 0; i < level; i++)
                                            {
                                                Console.Write("=");
                                            }
                                            Console.WriteLine($" {parent.Title} ID={parent.IdDepartment}");
                                        }
                                        OutputLevelDepartments(parents.Count, Session.Context.Departments.Where(x => x.IdDepartment == int.Parse(inputIdDepartment)).FirstOrDefault());
                                        break;
                                }
                                break;
                        }
                        break;
                    case 3:
                        Console.WriteLine("Процесс вывода запущен, пожалуйста, подождите..");
                        Task outputEmpTask = Task.Run(() => OutputTableEmployees());
                        outputEmpTask.Wait();
                        break;
                }
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка: {ex.Message}");
            }
            if (Session.badStrings.Count != 0)
            {
                Console.WriteLine("\nВнимание, в ходе выполнения операции Вывод произошли следующие ошибки:");
                foreach (var badStroke in Session.badStrings)
                {
                    Console.WriteLine(badStroke);
                }
            }
            Session.badStrings = new();
            Console.WriteLine("\nНажмите любую клавишу чтобы вернуться в меню..");
            Console.ReadKey();
        }
        static void OutputTablePositions()
        {
            Type type = typeof(Position);
            PropertyInfo[] positionProperties = type.GetProperties();
            List<Position> positions = Session.Context.Positions.ToList();
            var tablePos = new ConsoleTable(positionProperties[0].Name, positionProperties[1].Name);
            Console.Clear();
            Console.WriteLine("Таблица Должности");
            foreach (var position in positions)
            {
                tablePos.AddRow(position.IdPosition, position.Title);
            }
            tablePos.Write();
        }
        static void OutputTableDepartments()
        {
            Type type = typeof(Department);
            PropertyInfo[] departmentProperties = type.GetProperties();
            List<Department> departments = Session.Context.Departments.ToList();
            var tableDep = new ConsoleTable(departmentProperties[0].Name, departmentProperties[1].Name, departmentProperties[2].Name, departmentProperties[3].Name, departmentProperties[4].Name);
            Console.Clear();
            Console.WriteLine("Таблица Подразделения");
            foreach (var department in departments)
            {
                object nameParentDep = department.IdParentDepartment;
                object nameDirector = department.IdDirector;
                if (department.IdParentDepartment is not null)
                {
                    nameParentDep = departments.Where(x => x.IdDepartment == department.IdParentDepartment).Select(x => x.Title).FirstOrDefault();
                }
                else
                    nameParentDep = "null";
                if (department.IdDirector is not null)
                {
                    nameDirector = Session.Context.Employees.Where(x => x.IdEmployee == department.IdDirector).Select(x => x.Fullname).FirstOrDefault();
                }
                else
                    nameDirector = "null";
                tableDep.AddRow(department.IdDepartment, department.Title, nameParentDep.ToString(), nameDirector.ToString(), department.Phone);
            }
            tableDep.Write();

        }
        static void OutputTableEmployees()
        {
            Type type = typeof(Employee);
            PropertyInfo[] employeeProperties = type.GetProperties();
            List<Employee> employees = Session.Context.Employees.ToList();
            var tableEmp = new ConsoleTable(employeeProperties[0].Name, employeeProperties[1].Name, employeeProperties[2].Name, employeeProperties[3].Name, employeeProperties[4].Name, employeeProperties[5].Name);
            Console.Clear();
            Console.WriteLine("Таблица Сотрудники");
            foreach (var employee in employees)
            {
                string nameDep = Session.Context.Departments.Where(x => x.IdDepartment == employee.IdDepartment).Select(x => x.Title).FirstOrDefault();
                string namePos = Session.Context.Positions.Where(x => x.IdPosition == employee.IdPosition).Select(x => x.Title).FirstOrDefault();
                tableEmp.AddRow(employee.IdEmployee, nameDep, employee.Fullname, employee.Login, employee.Password, namePos);
            }
            tableEmp.Write();
        }
    }
}
