using ImportUtility.Model;
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
    public static class ImportFunctions
    {
        public static async void StartImport(List<string> stringsList, int idTable)
        {
            Console.Clear();
            Console.WriteLine("Запущен процесс очистки пробелов, пожалуйста, подождите..");
            stringsList = await Task.Run(() => RemoveSpaces(stringsList));
            Console.Clear();
            if (stringsList.Count > 0)
            {
                Console.WriteLine("Очистка пробелов завершена успешно\n");
                switch (idTable)
                {
                    case 1:
                        Console.WriteLine("Запущен процесс проверки формата, пожалуйста, подождите..");
                        List<Position> positions = await Task.Run(() => CheckFormatToPositions(stringsList));
                        Console.Clear();
                        if (positions.Count > 0)
                        {
                            Console.WriteLine("Форматирование завершено успешно\n");
                            Console.WriteLine("Запущен процесс добавления данных в базу, пожалуйста, подождите..");
                            int info = await Task.Run(() => AddOrChangePosition(positions));
                            Console.Clear();
                            Console.WriteLine("Результат:");
                            Console.WriteLine($"Добавлено:{info} записей");
                        }
                        else
                            Session.badStrings.Insert(0, "Ошибка: Ни одна строка файла не прошла проверку на формат\nВыбранная таблица: Должности");
                        break;
                    case 2:
                        Console.WriteLine("Запущен процесс замены слов на индексы, пожалуйста, подождите..");
                        stringsList = await Task.Run(() => ReplaceWordsDepartments(stringsList));
                        Console.Clear();
                        if (stringsList.Count > 0)
                        {
                            Console.WriteLine("Замена слов завершено успешно\n");
                            Console.WriteLine("Запущен процесс проверки формата, пожалуйста, подождите..");
                            List<Department> departments = await Task.Run(() => CheckFormatToDepartments(stringsList));
                            Console.Clear();
                            if (departments.Count > 0)
                            {
                                Console.WriteLine("Форматирование завершено успешно\n");
                                Console.WriteLine("Запущен процесс добавления данных в базу, пожалуйста, подождите..");
                                int[] info = await Task.Run(() => AddOrChangeDepartment(departments));
                                Console.Clear();
                                Console.WriteLine("Результат:");
                                Console.WriteLine($"Добавлено:{info[0]} записей");
                                Console.WriteLine($"Изменено:{info[1]} записей");
                            }
                            else
                                Session.badStrings.Insert(0, "Ошибка: Ни одна строка файла не прошла проверку на формат\nВыбранная таблица: Подразделения");
                        }
                        else
                            Session.badStrings.Insert(0, "Ошибка: Ни одна строка файла не прошла проверку на формат\nВыбранная таблица: Подразделения");
                        break;
                    case 3:
                        Console.WriteLine("Запущен процесс замены слов на индексы, пожалуйста, подождите..");
                        stringsList = await Task.Run(() => ReplaceWordsEmployees(stringsList));
                        Console.Clear();
                        if (stringsList.Count > 0)
                        {
                            Console.WriteLine("Замена слов завершено успешно\n");
                            Console.WriteLine("Запущен процесс проверки формата, пожалуйста, подождите..");
                            List<Employee> employees = await Task.Run(() => CheckFormatToEmployees(stringsList));
                            Console.Clear();
                            if (employees.Count > 0)
                            {
                                Console.WriteLine("Форматирование завершено успешно\n");
                                Console.WriteLine("Запущен процесс добавления данных в базу, пожалуйста, подождите..");
                                int[] info = await Task.Run(() => AddOrChangeEmployee(employees));
                                Console.Clear();
                                Console.WriteLine("Результат:");
                                Console.WriteLine($"Добавлено:{info[0]} записей");
                                Console.WriteLine($"Изменено:{info[1]} записей");
                            }
                        }
                        break;
                }
            }
            else
                Session.badStrings.Add("Ошибка: Очистка пробелов вернула пустой лист");
            if (Session.badStrings.Count != 0)
            {
                Console.WriteLine("Внимание, в ходе выполнения операции Импорт произошли следующие ошибки:\n");
                foreach (var badStroke in Session.badStrings)
                {
                    Console.WriteLine(badStroke);
                }
            }
            Session.badStrings = new();
            Console.WriteLine("\nНажмите любую клавишу чтобы вернуться в меню..");
        }
        public static List<string> SelectFile()
        {
            List<string> fileStringsList = new();
            Console.WriteLine("Введите название файла:");
            string filename = Console.ReadLine();
            //string filename = "departments"; //departments employees jobtitle
            try
            {
                StreamReader sr = new($"{filename}.tsv");
                sr.ReadLine();

                //Чтение из файла
                while (!sr.EndOfStream)
                {
                    fileStringsList.Add(sr.ReadLine());
                }
                if (fileStringsList.Count == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Ошибка: Файл пустой");
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            return fileStringsList;
        }
        //Функция добавления в базу данных для таблицы Employees
        static int[] AddOrChangeEmployee(List<Employee> employees)
        {
            int amountAdded = 0;
            int amountChanged = 0;
            try
            {
                List<Employee> dbEmployees = Session.Context.Employees.ToList();
                foreach (var employee in employees)
                {
                    try
                    {
                        if (dbEmployees.Select(x => x.Fullname.ToUpper()).Contains(employee.Fullname.ToUpper()))
                        {
                            Session.Context.Employees.Where(x => x.Fullname.ToUpper() == employee.Fullname.ToUpper()).FirstOrDefault().IdDepartment = employee.IdDepartment;
                            Session.Context.Employees.Where(x => x.Fullname.ToUpper() == employee.Fullname.ToUpper()).FirstOrDefault().Login = employee.Login;
                            Session.Context.Employees.Where(x => x.Fullname.ToUpper() == employee.Fullname.ToUpper()).FirstOrDefault().Password = employee.Password;
                            Session.Context.Employees.Where(x => x.Fullname.ToUpper() == employee.Fullname.ToUpper()).FirstOrDefault().IdPosition = employee.IdPosition;
                            amountChanged++;
                        }
                        else
                        {
                            Session.Context.Employees.Add(employee);
                            amountAdded++;
                        }
                        Session.Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {employee.IdDepartment},{employee.Fullname},{employee.Login}..)");
                    }
                }

            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка {ex.Message}");
            }
            int[] info = new int[2];
            info[0] = amountAdded;
            info[1] = amountChanged;
            return info;
        }

        //Функция добавления в базу данных для таблицы Departments
        static int[] AddOrChangeDepartment(List<Department> departments)
        {
            int amountAdded = 0;
            int amountChanged = 0;
            try
            {
                List<Department> dbDepartments = Session.Context.Departments.ToList();
                foreach (var department in departments)
                {
                    try
                    {
                        if (dbDepartments.Select(x => x.Title.ToUpper()).Contains(department.Title.ToUpper()))
                        {
                            Session.Context.Departments.Where(x => x.Title.ToUpper() == department.Title.ToUpper()).FirstOrDefault().IdParentDepartment = department.IdParentDepartment;
                            Session.Context.Departments.Where(x => x.Title.ToUpper() == department.Title.ToUpper()).FirstOrDefault().IdDirector = department.IdDirector;
                            Session.Context.Departments.Where(x => x.Title.ToUpper() == department.Title.ToUpper()).FirstOrDefault().Phone = department.Phone;
                            amountChanged++;
                        }
                        else
                        {
                            Session.Context.Departments.Add(new Department
                            {
                                Title = department.Title,
                                IdParentDepartment = department.IdParentDepartment,
                                IdDirector = department.IdDirector,
                                Phone = department.Phone
                            });
                            amountAdded++;
                        }
                        Session.Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {department.Title},{department.IdParentDepartment},{department.IdDirector},{department.Phone})");
                    }
                }
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка {ex.Message}");
            }
            int[] info = new int[2];
            info[0] = amountAdded;
            info[1] = amountChanged;
            return info;
        }

        //Функция добавления в базу данных для таблицы Positions
        static int AddOrChangePosition(List<Position> positions)
        {
            int amountAdded = 0;
            try
            {
                List<Position> dbPositions = Session.Context.Positions.ToList();
                foreach (var position in positions)
                {
                    try
                    {
                        if (!dbPositions.Select(x => x.Title.ToUpper()).Contains(position.Title.ToUpper()))
                        {
                            Session.Context.Positions.Add(position);
                            amountAdded++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {position.Title})");
                    }
                    Session.Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка {ex.Message}");
            }
            return amountAdded;
        }

        //Функция заменяет слова на Id для таблицы Employees
        static List<string> ReplaceWordsEmployees(List<string> treatedStringsList)
        {
            List<string> formatedStringList = new();
            try
            {
                List<Position> positions = Session.Context.Positions.ToList();
                List<Department> departments = Session.Context.Departments.ToList();
                foreach (var stroke in treatedStringsList)
                {
                    string temp = stroke;
                    string[] words = temp.Split('\t');
                    try
                    {
                        if (departments.Select(x => x.Title.ToUpper()).Contains(words.First().ToUpper()))
                        {
                            int idDepartment = departments.Where(x => x.Title.ToUpper() == words.First().ToUpper()).Select(x => x.IdDepartment).FirstOrDefault();
                            temp = temp.Replace(words[0], idDepartment.ToString());
                        }
                        if (positions.Select(x => x.Title.ToUpper()).Contains(words.Last().ToUpper()))
                        {
                            int idPosition = positions.Where(x => x.Title.ToUpper() == words.Last().ToUpper()).Select(x => x.IdPosition).FirstOrDefault();
                            temp = temp.Replace(words[4], idPosition.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                    }
                    formatedStringList.Add(temp);
                }
                return formatedStringList;
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка: {ex.Message}");
                return new List<string>();
            }
        }
        //Функция заменяет слова на Id для таблицы Departments
        static List<string> ReplaceWordsDepartments(List<string> treatedStringsList)
        {
            List<string> formatedStringList = new();
            try
            {
                List<Department> departments = Session.Context.Departments.ToList();
                List<Employee> employees = Session.Context.Employees.ToList();
                foreach (var stroke in treatedStringsList)
                {
                    string temp = stroke;
                    string[] words = temp.Split('\t');
                    try
                    {
                        if (departments.Select(x => x.Title.ToUpper()).Contains(words[1].ToUpper()))
                        {
                            int idDepartment = departments.Where(x => x.Title.ToUpper() == words[1].ToUpper()).Select(x => x.IdDepartment).FirstOrDefault();
                            temp = temp.Replace(words[1], idDepartment.ToString());
                        }
                        if (employees.Select(x => x.Fullname.ToUpper()).Contains(words[2].ToUpper()))
                        {
                            int idDirector = employees.Where(x => x.Fullname.ToUpper() == words[2].ToUpper()).Select(x => x.IdEmployee).FirstOrDefault();
                            temp = temp.Replace(words[2], idDirector.ToString());
                        }
                        formatedStringList.Add(temp);
                    }
                    catch (Exception ex)
                    {
                        Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                    }
                }
                return formatedStringList;
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка: {ex.Message}");
                return new List<string>();
            }
        }
        //Функция проверяет данные на формат для таблицы Employees
        static List<Employee> CheckFormatToEmployees(List<string> treatedStringsList)
        {
            try
            {
                int amountProp = 0;
                Type type = typeof(Employee);
                PropertyInfo[] propertiesEmployee = type.GetProperties();
                foreach (var prop in propertiesEmployee)
                {
                    if (prop.GetGetMethod() != null && !prop.GetSetMethod().IsVirtual)
                    {
                        amountProp++;
                    }
                }
                amountProp--;
                List<Employee> employees = new();
                foreach (var stroke in treatedStringsList)
                {
                    string[] words = stroke.Split('\t');
                    if (words.Length == amountProp)
                    {
                        if (words[0] == "0")
                        {
                            try
                            {
                                employees.Add(new Employee
                                {
                                    IdDepartment = null,
                                    Fullname = words[1],
                                    Login = words[2],
                                    Password = words[3],
                                    IdPosition = int.Parse(words[4])
                                });
                            }
                            catch (Exception ex)
                            {
                                Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                            }
                        }
                        else
                        {
                            try
                            {
                                employees.Add(new Employee
                                {
                                    IdDepartment = int.Parse(words[0]),
                                    Fullname = words[1],
                                    Login = words[2],
                                    Password = words[3],
                                    IdPosition = int.Parse(words[4])
                                });
                            }
                            catch (Exception ex)
                            {
                                Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                            }
                        }
                    }
                    else
                        Session.badStrings.Add($"Ошибка: Строка имеет неверный формат\n(строка: {stroke})");
                }
                return employees;
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка: {ex.Message}");
                return new List<Employee>();
            }
        }
        //Функция проверяет данные на формат для таблицы Departments
        static List<Department> CheckFormatToDepartments(List<string> treatedStringsList)
        {
            try
            {
                int amountProp = 0;
                Type type = typeof(Department);
                PropertyInfo[] propertiesDepartment = type.GetProperties();
                foreach (var prop in propertiesDepartment)
                {
                    if (prop.GetGetMethod() != null && !prop.GetSetMethod().IsVirtual)
                    {
                        amountProp++;
                    }
                }
                amountProp--;
                List<Department> departments = new();
                foreach (var stroke in treatedStringsList)
                {
                    string[] words = stroke.Split('\t');
                    if (Regex.IsMatch(words.Last(), @"\d+")) //Если последнее слово содержит цифры
                    {
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (words[i] == "0")
                                words[i] = null;
                        }

                        if (words.Length == amountProp)
                        {
                            if (words[1] == null)
                            {
                                try
                                {
                                    departments.Add(new Department
                                    {
                                        Title = words[0],
                                        IdParentDepartment = null,
                                        IdDirector = ToNullableInt(words[2]),
                                        Phone = words[3]
                                    });
                                }
                                catch (Exception ex)
                                {
                                    Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                                }
                            }
                            else
                            {
                                try
                                {
                                    departments.Add(new Department
                                    {
                                        Title = words[0],
                                        IdParentDepartment = int.Parse(words[1]),
                                        IdDirector = int.Parse(words[2]),
                                        Phone = words[3]
                                    });
                                }
                                catch
                                {
                                    try
                                    {
                                        departments.Add(new Department
                                        {
                                            Title = words[0],
                                            IdParentDepartment = int.Parse(words[1]),
                                            IdDirector = null,
                                            Phone = words[3]
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                                    }
                                }
                            }
                        }
                        else
                            Session.badStrings.Add($"Ошибка: Строка имеет неверный формат\n(строка: {stroke})");
                    }
                    else
                        Session.badStrings.Add($"Ошибка: Строка имеет неверный формат\n(строка: {stroke})");
                }
                return departments;
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка: {ex.Message}");
                return new List<Department>();
            }
        }
        static int? ToNullableInt(this string s)
        {
            if (int.TryParse(s, out int i)) return i;
            return null;
        }
        //Функция проверяет данные на формат для таблицы Positions
        static List<Position> CheckFormatToPositions(List<string> treatedStringsList)
        {
            //Используя рефлекшн, подсчитаем количество свойств у класса
            int amountProp = 0;
            Type type = typeof(Position);
            PropertyInfo[] propertiesPosition = type.GetProperties();
            foreach (var prop in propertiesPosition)
            {
                if (prop.GetGetMethod() != null && !prop.GetSetMethod().IsVirtual)
                {
                    amountProp++;
                }
            }
            amountProp--; //Вычитаем 1 свойство, т.к. это ID, а он AutoIncrement
            try
            {
                List<Position> positions = new();
                foreach (var stroke in treatedStringsList)
                {
                    string[] words = stroke.Split('\t');
                    if (words.Length == amountProp)
                        try
                        {
                            positions.Add(new Position { Title = words[0] });
                        }
                        catch (Exception ex)
                        {
                            Session.badStrings.Add($"Ошибка: {ex.Message}\n(строка: {stroke})");
                        }
                    else
                        Session.badStrings.Add($"Ошибка: Строка имеет неверный формат\n(строка: {stroke})");
                }
                return positions;
            }
            catch (Exception ex)
            {
                Session.badStrings.Add($"Ошибка: {ex.Message}");
                return new List<Position>();
            }
        }
        //Функция убирает лишние пустоты в строках
        static List<string> RemoveSpaces(List<string> rawList)
        {
            List<string> treatedStringsList = new();
            foreach (var stroke in rawList)
            {
                if (!String.IsNullOrWhiteSpace(stroke))
                {
                    string temp = stroke;
                    int indexNextTab = temp.IndexOf('\t');
                    string treatedStroke = String.Empty;
                    while (indexNextTab >= 0)
                    {
                        string word;
                        if (indexNextTab != 0)
                        {
                            word = temp.Substring(0, indexNextTab);
                            word = $"{Regex.Replace(word.Trim(), @"\s+", " ")}";
                            temp = temp.Remove(0, indexNextTab + 1);
                        }
                        else
                        {
                            word = "0";
                            temp = temp.Remove(0, 1);
                        }
                        treatedStroke += $"{word}\t";
                        indexNextTab = temp.IndexOf('\t');
                    }
                    temp = $"{Regex.Replace(temp, @"\s+", " ")}";
                    treatedStroke += $"{temp}";
                    treatedStringsList.Add(treatedStroke);
                }
            }
            return treatedStringsList;
        }
    }
}
