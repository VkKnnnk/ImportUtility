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
        private const string CLEAR_C = "clear";
        private const string CD_C = "cd";
        private const string DIR_C = "dir";
        private const string HELP_C = "help";
        private const string IMPORT_C = "import";
        private const string OUTPUT_C = "output";
        private const string DATABASE_C = "database";
        #endregion
        static void Main(string[] args)
        {
            RunСommandInterpreter(args);
        }
        private static void RunСommandInterpreter(string[] commandLineArgs)
        {
            int amountArgs = commandLineArgs.Length;
            if (amountArgs == 0)
            {
                commandLineArgs = new string[] { HELP_C };
                amountArgs = commandLineArgs.Length;
            }

            string selectedCommand = commandLineArgs[0].ToLower();
            int amountParams = amountArgs - 1;

            switch (selectedCommand)
            {
                case HELP_C:
                    {
                        switch (amountParams)
                        {
                            case 0:
                                ShowHelpCommand();
                                break;
                            case 1:
                                {
                                    string selectedSubCommand = commandLineArgs[1].ToLower();
                                    HandleSelectedCommand(selectedSubCommand);
                                    break;
                                }
                            default:
                                Console.WriteLine($"Команда `{HELP_C}` может иметь только один параметр");
                                break;
                        }
                        break;
                    }
                case CD_C:
                    {
                        switch (amountParams)
                        {
                            case 0:
                                GoToRootDirectory();
                                break;
                            case 1:
                                {
                                    string selectedDirectory = commandLineArgs[1].ToLower();
                                    switch (selectedDirectory)
                                    {
                                        case "..":
                                            GoToParentDirectory();
                                            break;

                                        default:
                                            SetCurrentDirectory(selectedDirectory);
                                            break;
                                    }
                                    break;
                                }
                            default:
                                Console.WriteLine($"Команда `{CD_C}` не может содержать {amountParams} параметр(ов).\nВведите `{HELP_C} {CD_C}` для получения информации о доступных параметрах");
                                break;
                        }
                        break;
                    }
                case DIR_C:
                    PrintFilesInCurrentDirectory();
                    break;
                case OUTPUT_C:
                    {
                        switch (amountParams)
                        {
                            case 0:
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
                            case 1:
                                {
                                    string selectedDepartment = commandLineArgs[1];
                                    try
                                    {
                                        DataParser.DisplayDepartmentsAsHierarchy(selectedDepartment);
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
                                Console.WriteLine($"Команда `{OUTPUT_C}` не может содержать {amountParams} параметр(ов).\nВведите `{HELP_C} {OUTPUT_C}` для получения информации о доступных параметрах");
                                break;
                        }
                        break;
                    }
                case IMPORT_C:
                    {
                        switch (amountParams)
                        {
                            case 2:
                                {
                                    string selectedFilename = commandLineArgs[1];
                                    string selectedTypeImport = commandLineArgs[2].ToLower();
                                    try
                                    {
                                        DataParser.ParseDataFromFile(selectedFilename, selectedTypeImport);
                                    }
                                    catch (FileNotFoundException ex)
                                    {
                                        Console.WriteLine($"\n{ex.Message} в директории `{Path.GetFileName(Directory.GetCurrentDirectory())}`");
                                    }
                                    catch (ArgumentOutOfRangeException ex)
                                    {
                                        Console.WriteLine($"\nНеизвестный параметр для типа импорта `{ex.ParamName}`" +
                                        $"\nВведите `{HELP_C} {IMPORT_C}` для получения информации о доступных параметрах");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nОшибка:");
                                        Console.WriteLine(ex.Message);
                                    }
                                    break;
                                }
                            default:
                                Console.WriteLine($"Команда `{IMPORT_C}` не может содержать {amountParams} параметр(ов)." +
                                    $"\nВведите `{HELP_C} {IMPORT_C}` для получения информации о доступных параметрах");
                                break;
                        }
                        break;
                    }
                case CLEAR_C:
                    {
                        Console.Clear();
                        break;
                    }
                case DATABASE_C:
                    {
                        switch (amountParams)
                        {
                            case 0:
                                GetConnectionString();
                                break;
                            case 2:
                                {
                                    string selectedSubCommand = commandLineArgs[1].ToLower();
                                    string selectedConnectionString = commandLineArgs[2];
                                    switch (selectedSubCommand)
                                    {
                                        case "set":
                                            SetDBConnection(selectedConnectionString);
                                            break;
                                        default:
                                            Console.WriteLine($"Неизвестный параметр `{selectedSubCommand}` для команды `{DATABASE_C}`.\nВведите `{HELP_C} {DATABASE_C}` для получения информации о доступных параметрах команды.");
                                            break;
                                    }
                                    break;
                                }
                            default:
                                Console.WriteLine($"Команда `{DATABASE_C}` не может содержать {amountParams} параметр(ов).\nВведите `{HELP_C} {DATABASE_C}` для получения информации о доступных командах");
                                break;
                        }
                        break;
                    }
                default:
                    Console.WriteLine($"Неизвестная команда `{selectedCommand}`.\nВведите `{HELP_C}` для получения информации о доступных командах.");
                    break;
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Title = $"ImportUtility:{Directory.GetCurrentDirectory()}";
            Console.Write(">");
            string input = Console.ReadLine();
            commandLineArgs = Regex.Split(input, @"\s(?=(?:[^""]*""[^""]*"")*[^""]*$)")
                                .Where(s => !String.IsNullOrEmpty(s))
                                .Select(s => s.Trim('"'))
                                .ToArray();
            RunСommandInterpreter(commandLineArgs);
        }
        #region Сommand Interpreter methods
        private static void SetDBConnection(string inputConnectionString)
        {
            if (!String.IsNullOrWhiteSpace(inputConnectionString))
            {
                AppSettings appsettings = new AppSettings
                {
                    ConnectionStrings = new AppSettings.ConnectionStringInfo
                    {
                        DefaultConnection = inputConnectionString
                    }
                };

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
        private static void GetConnectionString()
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
            {
                Console.WriteLine($"Файл конфигурации отсутствует в директории {Path.GetFileName(Directory.GetCurrentDirectory())}");
                Console.WriteLine($"\nПерейдите в директорию, где уже создан файл конфигурации.\n" +
                    $"Или создайте новый экземпляр в любой директории, используя команду: `{DATABASE_C} set <\"строка подключения\">`");

            }
        }
        private static void ShowHelpCommand()
        {
            Console.WriteLine("\nКоманды, доступные в программе:\n");
            Console.WriteLine("\t{0,-15} Импорт данных из файла в БД", IMPORT_C);
            Console.WriteLine("\t{0,-15} Вывод иерархии подразделений", OUTPUT_C);
            Console.WriteLine("\t{0,-15} Показать доступные команды", HELP_C);
            Console.WriteLine("\t{0,-15} Изменить рабочую директорию", CD_C);
            Console.WriteLine("\t{0,-15} Показать файлы/директории в директории", DIR_C);
            Console.WriteLine("\t{0,-15} Очистить консоль", CLEAR_C);
            Console.WriteLine("\t{0,-15} Подключение к базе данных", DATABASE_C);
            Console.WriteLine($"\nВведите `{HELP_C} <command>` в консоли, чтобы узнать о параметрах команды");
        }
        private static void HandleSelectedCommand(string command)
        {
            switch (command)
            {
                case CD_C:
                    {
                        Console.WriteLine($"\nКоманда `{CD_C}`:\n");
                        Console.WriteLine("\t{0,-15} Перейти в корневую директорию системы", "<>");
                        Console.WriteLine("\t{0,-15} Подняться по директории выше", "<..>");
                        Console.WriteLine("\t{0,-15} Перейти в директорию по заданному пути", "<путь>");
                        break;
                    }
                case DIR_C:
                    {
                        Console.WriteLine($"\nКоманда `{DIR_C}`:\n");
                        Console.WriteLine("\t{0,-15} Отобразить все файлы/директории директории", "<>");
                        break;
                    }
                case IMPORT_C:
                    {
                        Console.WriteLine($"\nКоманда `{IMPORT_C}`:\n");
                        Console.WriteLine("\t{0,-30} Импортировать из файла в таблицу", "<имя_файла> <тип_импорта>");
                        Console.WriteLine("\nПояснение:\n");
                        Console.WriteLine("<имя_файла> означает имя файла и его расширение");
                        Console.WriteLine("<тип_импорта> означает таблицу, в которую мы импортируем:\n");
                        Console.WriteLine("`D` - таблица подразделения");
                        Console.WriteLine("`E` - таблица сотрудники");
                        Console.WriteLine("`P` - таблица должности");
                        break;
                    }
                case OUTPUT_C:
                    {
                        Console.WriteLine($"\nКоманда `{OUTPUT_C}`:\n");
                        Console.WriteLine("\t{0,-15} Вывести иерархию всех подразделений", "<>");
                        Console.WriteLine("\t{0,-15} Вывести цепочку родительских подразделений определенного подразделения", "<id>");
                        break;
                    }
                case CLEAR_C:
                    {
                        Console.WriteLine($"\nКоманда `{CLEAR_C}`:\n");
                        Console.WriteLine("\t{0,-15} Очистить консоль", "<>");
                        break;
                    }
                case DATABASE_C:
                    {
                        Console.WriteLine($"\nКоманда `{DATABASE_C}`:\n");
                        Console.WriteLine("\t{0,-30} Посмотреть строку подключения", "<>");
                        Console.WriteLine("\t{0,-30} Установить строку подключения", "<set> <\"строка подключения\">");
                        break;
                    }
                case HELP_C:
                    {
                        Console.WriteLine($"\nКоманда `{HELP_C}`:\n");
                        Console.WriteLine("\t{0,-15} Вывести список доступных команд", "<>");
                        Console.WriteLine("\t{0,-15} Показать параметры команды", "<command>");
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Неизвестная команда `{command}`.\nВведите `{HELP_C}` для получения информации о доступных командах.");
                        break;
                    }
            }
        }
        private static void GoToRootDirectory()
        {
            if (Directory.GetCurrentDirectory() == Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()))
                Console.WriteLine("Вы уже находитесь в корневой директории системы");
            else
                Directory.SetCurrentDirectory(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()));
        }
        private static void GoToParentDirectory()
        {
            if (Directory.GetCurrentDirectory() == Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()))
                Console.WriteLine("Вы уже находитесь в корневой директории системы");
            else
                Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
        }
        private static void SetCurrentDirectory(string directory)
        {
            if (Directory.Exists(directory))
                Directory.SetCurrentDirectory(directory);
            else
                Console.WriteLine($"Не удалось найти директорию `{directory}` в директории '{Path.GetFileName(Directory.GetCurrentDirectory())}'");
        }
        private static void PrintFilesInCurrentDirectory()
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                Console.WriteLine(Path.GetFileName(file));
            }
            foreach (string directory in Directory.GetDirectories(Directory.GetCurrentDirectory()))
            {
                Console.WriteLine(Path.GetFileName(directory));
            }
        }
        #endregion
    }
}

