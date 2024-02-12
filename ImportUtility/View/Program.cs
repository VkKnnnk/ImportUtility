using ImportUtility.View_Model;
using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace ImportUtility
{
    class Program
    {
        #region Constants
        private const string CLEAR = "clear";
        private const string CD = "cd";
        private const string DIR = "dir";
        private const string HELP = "help";
        private const string IMPORT = "import";
        private const string OUTPUT = "output";
        private const string TABLE_D = "d";
        private const string TABLE_E = "e";
        private const string TABLE_P = "p";
        #endregion
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { "help" };
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
                                    //Вывод иерархии подразделений
                                    //Output()
                                    break;
                                }
                            case 2:
                                {
                                    //Вывод иерархии подразделений подразделения
                                    //Output(id)
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `output` не может содержать {args.Length} параметр(ов). Введите `help` для получения информации о доступных командах");
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
                                        ImportFunctions.Import(args[1], args[2]);
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
                default:
                    {
                        Console.WriteLine($"Неизвестная команда `{args[0]}`.\nВведите `help` для получения информации о доступных командах.");
                        break;
                    }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Title = $"ImportUtility:{Directory.GetCurrentDirectory()}";
            Console.Write(">");
            args = Console.ReadLine().Split(' ');
            Main(args);
        }
    }
}

