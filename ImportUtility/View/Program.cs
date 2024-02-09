using ImportUtility.View_Model;
using System;
using System.Collections.Generic;

namespace ImportUtility
{
    class Program
    {
        static bool exit = false;
        static void Main()
        {
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("||||||||||||||-----МЕНЮ-----||||||||||||||");
                Console.WriteLine("I - Импорт");
                Console.WriteLine("O - Вывод");
                Console.WriteLine("E - Выход");
                Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||");
                Console.WriteLine("Выберите пункт меню:");
                ConsoleKey selectedMenuOption = Console.ReadKey().Key;
                Console.Clear();
                switch (selectedMenuOption)
                {
                    //Выбран импорт
                    case ConsoleKey.I:
                        Console.Clear();
                        List<string> stringsList = new();
                        while (stringsList.Count == 0)
                        {
                            stringsList = ImportFunctions.SelectFile();
                        }
                        Console.Clear();
                        Console.WriteLine($"Файл прочитан успешно ({stringsList.Count} строк)\n");
                        ConsoleKey selectedImportTableOption = new();
                        while (selectedImportTableOption != ConsoleKey.D1 && selectedImportTableOption != ConsoleKey.D2 && selectedImportTableOption != ConsoleKey.D3)
                        {
                            Console.Clear();
                            Console.WriteLine("Выберите таблицу, в которую хотите импортировать данные");
                            Console.WriteLine("1 - Должности");
                            Console.WriteLine("2 - Подразделения");
                            Console.WriteLine("3 - Сотрудники");
                            selectedImportTableOption = Console.ReadKey().Key;
                        }
                        Console.Clear();
                        switch (selectedImportTableOption)
                        {
                            case ConsoleKey.D1:
                                ImportFunctions.StartImport(stringsList, 1);
                                break;
                            case ConsoleKey.D2:
                                ImportFunctions.StartImport(stringsList, 2);
                                break;
                            case ConsoleKey.D3:
                                ImportFunctions.StartImport(stringsList, 3);
                                break;
                        }
                        break;
                    case ConsoleKey.O:
                        //Выбран вывод
                        ConsoleKey selectedOutputTableOption = new();
                        while (selectedOutputTableOption != ConsoleKey.D1 && selectedOutputTableOption != ConsoleKey.D2 && selectedOutputTableOption != ConsoleKey.D3)
                        {
                            Console.Clear();
                            Console.WriteLine("Выберите таблицу");
                            Console.WriteLine("1 - Должности");
                            Console.WriteLine("2 - Подразделения");
                            Console.WriteLine("3 - Сотрудники");
                            selectedOutputTableOption = Console.ReadKey().Key;
                        }
                        Console.Clear();
                        switch (selectedOutputTableOption)
                        {
                            case ConsoleKey.D1:
                                OutputFunctions.StartOutput(1);
                                break;
                            case ConsoleKey.D2:
                                OutputFunctions.StartOutput(2);
                                break;
                            case ConsoleKey.D3:
                                OutputFunctions.StartOutput(3);
                                break;
                        }
                        break;
                    case ConsoleKey.E:
                        exit = true;
                        break;
                }
            }
        }
    }
}
