using ImportUtility.Model;
using ImportUtility.View_Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImportUtility
{
    class Program
    {
        #region Commands constants
        private const string CLEAR = "clear";
        private const string CD = "cd";
        private const string DIR = "dir";
        private const string HELP = "help";
        private const string IMPORT = "import";
        private const string OUTPUT = "output";
        private const string DB_CONNECTION = "dbconnection";
        #endregion
        static void Main(string[] args)
        {
            RunСommandInterpreter(args);
        }
        private static void RunСommandInterpreter(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { HELP };

            switch (args[0].ToLower())
            {
                case HELP:
                    {
                        switch (args.Length)
                        {
                            case 1:
                                {
                                    Console.WriteLine("\nКоманды, доступные в программе:\n");
                                    Console.WriteLine("\t{0,-15} Импорт данных из файла в БД", IMPORT);
                                    Console.WriteLine("\t{0,-15} Вывод иерархии подразделений", OUTPUT);
                                    Console.WriteLine("\t{0,-15} Показать доступные команды", HELP);
                                    Console.WriteLine("\t{0,-15} Изменить рабочую директорию", CD);
                                    Console.WriteLine("\t{0,-15} Показать файлы/директории в директории", DIR);
                                    Console.WriteLine("\t{0,-15} Очистить консоль", CLEAR);
                                    Console.WriteLine("\t{0,-15} Подключение к базе данных", DB_CONNECTION);
                                    Console.WriteLine($"\nВведите `{HELP} <command>` в консоли, чтобы узнать о параметрах команды");
                                    break;
                                }
                            case 2:
                                {
                                    switch (args[1].ToLower())
                                    {
                                        case CD:
                                            {
                                                Console.WriteLine($"\nКоманда `{CD}`:\n");
                                                Console.WriteLine("\t{0,-15} Перейти в корневую директорию системы", "<>");
                                                Console.WriteLine("\t{0,-15} Подняться по директории выше", "<..>");
                                                Console.WriteLine("\t{0,-15} Перейти в директорию по заданному пути", "<путь>");
                                                break;
                                            }
                                        case DIR:
                                            {
                                                Console.WriteLine($"\nКоманда `{DIR}`:\n");
                                                Console.WriteLine("\t{0,-15} Отобразить все файлы/директории директории", "<>");
                                                break;
                                            }
                                        case IMPORT:
                                            {
                                                Console.WriteLine($"\nКоманда `{IMPORT}`:\n");
                                                Console.WriteLine("\t{0,-15} Импортировать из файла в таблицу", "<имя_файла> <тип_импорта>");
                                                Console.WriteLine("\n<имя_файла> означает имя файла и его расширение");
                                                Console.WriteLine("<тип_импорта> означает таблицу, в которую мы импортируем:\n");
                                                Console.WriteLine("`D` - таблица подразделения");
                                                Console.WriteLine("`E` - таблица сотрудники");
                                                Console.WriteLine("`P` - таблица должности");
                                                break;
                                            }
                                        case OUTPUT:
                                            {
                                                Console.WriteLine($"\nКоманда `{OUTPUT}`:\n");
                                                Console.WriteLine("\t{0,-15} Вывести иерархию всех подразделений", "<>");
                                                Console.WriteLine("\t{0,-15} Вывести цепочку родительских подразделений определенного подразделения", "<id>");
                                                break;
                                            }
                                        case CLEAR:
                                            {
                                                Console.WriteLine($"\nКоманда `{CLEAR}`:\n");
                                                Console.WriteLine("\t{0,-15} Очистить консоль", "<>");
                                                break;
                                            }
                                        case DB_CONNECTION:
                                            {
                                                Console.WriteLine($"\nКоманда `{DB_CONNECTION}`:\n");
                                                Console.WriteLine("\t{0,-30} Посмотреть строку подключения", "<>");
                                                Console.WriteLine("\t{0,-30} Установить строку подк", "<set> <\"строка_подключения\">");
                                                break;
                                            }
                                        default:
                                            {
                                                Console.WriteLine($"Неизвестная команда `{args[1]}`.\nВведите `{HELP}` для получения информации о доступных командах.");
                                                break;
                                            }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `{HELP}` может иметь только один параметр");
                                    break;
                                }
                        }
                        break;
                    }
                case CD:
                    {
                        switch (args.Length)
                        {
                            case 1:
                                {
                                    if (Directory.GetCurrentDirectory() == Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()))
                                        Console.WriteLine("Вы уже находитесь в корневой директории системы");
                                    else
                                        Directory.SetCurrentDirectory(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()));
                                    break;
                                }
                            case 2:
                                {
                                    switch (args[1].ToLower())
                                    {
                                        case "..":
                                            {
                                                if (Directory.GetCurrentDirectory() == Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()))
                                                    Console.WriteLine("Вы уже находитесь в корневой директории системы");
                                                else
                                                    Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
                                                break;
                                            }
                                        default:
                                            {
                                                if (Directory.Exists(args[1]))
                                                    Directory.SetCurrentDirectory(args[1]);
                                                else
                                                    Console.WriteLine($"Не удалось найти директорию `{args[1]}` в директории '{Path.GetFileName(Directory.GetCurrentDirectory())}'");
                                                break;
                                            }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `{CD}` не может содержать {args.Length - 1} параметр(ов).\nВведите `{HELP} {CD}` для получения информации о доступных параметрах");
                                    break;
                                }
                        }
                        break;
                    }
                case DIR:
                    {
                        foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                        {
                            Console.WriteLine(Path.GetFileName(file));
                        }
                        foreach (string directory in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                        {
                            Console.WriteLine(Path.GetFileName(directory));
                        }
                        break;
                    }
                case OUTPUT:
                    {
                        switch (args.Length)
                        {
                            case 1:
                                {
                                    try
                                    {
                                        DataParser.DisplayDepartmentsAsHierarchy();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nОшибка:");
                                        Console.WriteLine(ex.Message);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    try
                                    {
                                        DataParser.DisplayDepartmentsAsHierarchy(args[1]);
                                    }
                                    catch (FormatException ex)
                                    {
                                        Console.WriteLine($"\nВведенные данные неккоректны. {ex.Message}");
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        Console.WriteLine($"\n{ex.Message}");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nОшибка:");
                                        Console.WriteLine(ex.Message);
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `{OUTPUT}` не может содержать {args.Length - 1} параметр(ов).\nВведите `{HELP} {OUTPUT}` для получения информации о доступных параметрах");
                                    break;
                                }
                        }
                        break;
                    }
                case IMPORT:
                    {
                        switch (args.Length)
                        {
                            case 3:
                                {
                                    try
                                    {
                                        DataParser.ParseDataFromFile(args[1], args[2]);
                                    }
                                    catch (FileNotFoundException ex)
                                    {
                                        Console.WriteLine($"\n{ex.Message} в директории `{Path.GetFileName(Directory.GetCurrentDirectory())}`");
                                    }
                                    catch (ArgumentOutOfRangeException ex)
                                    {
                                        Console.WriteLine($"\nНеизвестный параметр для типа импорта `{ex.ParamName}`" +
                                        $"\nВведите `{HELP} {IMPORT}` для получения информации о доступных параметрах");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nОшибка:");
                                        Console.WriteLine(ex.Message);
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `{IMPORT}` не может содержать {args.Length - 1} параметр(ов)." +
                                        $"\nВведите `{HELP} {IMPORT}` для получения информации о доступных параметрах");
                                    break;
                                }
                        }
                        break;
                    }
                case CLEAR:
                    {
                        Console.Clear();
                        break;
                    }
                case DB_CONNECTION:
                    {
                        switch (args.Length)
                        {
                            case 1:
                                {
                                    if (File.Exists("appsettings.json"))
                                    {
                                        Console.WriteLine("\nТекущая строка подключения:");
                                        using (UnkCompanyDBContext dbContext = new())
                                        {
                                            if (dbContext.Database.CanConnect())
                                                Console.ForegroundColor = ConsoleColor.Green;
                                            else
                                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                        }
                                        using (FileStream fs = new("appsettings.json", FileMode.Open, FileAccess.Read))
                                        {
                                            using (StreamReader sr = new(fs))
                                            {
                                                var appsettings = JsonConvert.DeserializeObject<AppSettings>(sr.ReadToEnd());
                                                Console.WriteLine(appsettings.ConnectionStrings.DefaultConnection);
                                            }
                                        }
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                    else
                                        Console.WriteLine("Строка подключения отсутсвует");
                                    break;
                                }
                            case 3:
                                {
                                    switch (args[1].ToLower())
                                    {
                                        case "set":
                                            {
                                                SetDBConnection(args[2]);
                                                break;
                                            }
                                        default:
                                            {
                                                Console.WriteLine($"Неизвестный параметр `{args[1]}` для команды `{DB_CONNECTION}`.\nВведите `{HELP} {DB_CONNECTION}` для получения информации о доступных параметрах команды.");
                                                break;
                                            }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `{DB_CONNECTION}` не может содержать {args.Length} параметр(ов).\nВведите `{HELP} {DB_CONNECTION}` для получения информации о доступных командах");
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Неизвестная команда `{args[0]}`.\nВведите `{HELP}` для получения информации о доступных командах.");
                        break;
                    }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Title = $"ImportUtility:{Directory.GetCurrentDirectory()}";
            Console.Write(">");
            string input = Console.ReadLine();
            args = Regex.Split(input, @"\s(?=(?:[^""]*""[^""]*"")*[^""]*$)")
                                .Where(s => !String.IsNullOrEmpty(s))
                                .Select(s => s.Trim('"'))
                                .ToArray();
            RunСommandInterpreter(args);
        }
        private static void SetDBConnection(string inputConnection)
        {
            AppSettings appsettings = new();
            appsettings.ConnectionStrings = new AppSettings.ConnectionStringInfo { DefaultConnection = inputConnection };

            using (FileStream fs = new("appsettings.json", FileMode.Truncate))
            {
                using (StreamWriter sw = new(fs))
                {
                    string json = JsonConvert.SerializeObject(appsettings);
                    sw.Write(json);
                }
            }
        }
    }
}

