using System;

namespace ImportUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args[0].ToUpper())
            {
                case "HELP":
                    {
                        Console.WriteLine("Команды, доступные в программе:\n");
                        Console.WriteLine("Выполнить импорт:");
                        Console.WriteLine("\t{0,-35} Импорт данных из файла в базу данных.", "import [имя_файла] [тип_импорта]");
                        Console.WriteLine("\t\t-- {0,-24} Обозначает имя файла, из которого будет производиться импорт.", "[имя_файла]");
                        Console.WriteLine("\t\t-- {0,-24} Обозначает, в какую таблицу программе импортировать данные.", "[тип_импорта]");
                        Console.WriteLine("\t\t{0,-28}(`D` - таблица подразделения, `E` - таблица сотрудники, `P` таблица должности)", "");
                        Console.WriteLine();
                        Console.WriteLine("Выполнить вывод:");
                        Console.WriteLine("\t{0,-35} Вывод иерархии подразделений.", "output");
                        Console.WriteLine("\t{0,-35} Вывод иерархии подразделений для конкректного подразделения.", "output [id_department]");
                        Console.WriteLine("\t\t-- {0,-24} Обозначает, иерархию какого подразделения вывести.", "[id_department]");
                        Console.WriteLine();
                        break;
                    }
                case "OUTPUT":
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
                case "IMPORT":
                    {
                        switch (args.Length)
                        {
                            case 3:
                                {
                                    switch (args[3].ToUpper())
                                    {
                                        case "D":
                                            {
                                                //Импорт подразделения
                                                break;
                                            }
                                        case "E":
                                            {
                                                //Импорт сотрудника
                                                break;
                                            }
                                        case "P":
                                            {
                                                //Импорт должности
                                                break;
                                            }
                                        default:
                                            {
                                                Console.WriteLine($"Неизвестный параметр `{args[3]}` для команды `import`. Введите `help` для получения информации о доступных командах");
                                                break;
                                            }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"Команда `import` не может содержать {args.Length} параметр(ов). Введите `help` для получения информации о доступных командах");
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Неизвестная команда. Введите `help` для получения информации о доступных командах.");
                        break;
                    }
            }
        }
    }
}

