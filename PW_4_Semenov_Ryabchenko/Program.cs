using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace PW_4_Semenov_Ryabchenko
{
    class Program
    {
        private static Queue<string> expressions = new Queue<string>(); // Списки для хранения последних 3 выражений  
        private static Queue<double> results = new Queue<double>();
        private const int maxMemorySize = 3;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Установка кодировки  
            Console.WriteLine("Добро пожаловать в калькулятор!");
            Console.WriteLine("Нажмите 'e' для выхода.");
            LoadMemory(); // Загружаем прошлые выражения и результаты  
            int expressionCount = 1; // Счетчик выражений  
            while (true)
            {
                Console.Write($"Введите {expressionCount} выражение: ");
                string userInput = Console.ReadLine();
                // Проверяем нажатие 'E' для выхода  
                if (userInput.Equals("e", StringComparison.OrdinalIgnoreCase) ||
                (string.IsNullOrEmpty(userInput) && Console.KeyAvailable &&
                Console.ReadKey(true).Key == ConsoleKey.Escape))
                {
                    SaveMemory(); // Сохраняем память перед выходом  
                    break; // Выход из цикла  
                }
                if (!string.IsNullOrEmpty(userInput))
                {
                    try
                    {
                        double result = EvaluateExpression(userInput);
                        Console.WriteLine($"Результат: {result}");
                        SaveResult(userInput, result); // Сохраняем выражение и результат в памяти  
                    }
                    catch (DivideByZeroException)
                    {
                        Console.WriteLine("Ошибка: Деление на ноль недопустимо.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                    expressionCount++; // Увеличение счетчика выражений  
                }
            }
        }
        static double EvaluateExpression(string expression)
        {
            var dataTable = new DataTable(); // Используем DataTable для вычисления выражения  
            if (expression.Contains("/0") && !expression.Contains("/0.")) // Обработка деления на ноль  
            {
                throw new DivideByZeroException();
            }
            return Convert.ToDouble(dataTable.Compute(expression, string.Empty));
        }
        static void SaveResult(string expression, double result)
        {
            if (expressions.Count >= maxMemorySize)
            {
                expressions.Dequeue(); // Удаляем самое старое выражение  
                results.Dequeue();     // Удаляем самый старый результат  
            }
            expressions.Enqueue(expression);
            results.Enqueue(result);
        }
        static void LoadMemory()
        {
            // Загрузка предыдущих выражений и результатов (если они есть)  
            try
            {
                string[] savedData = File.ReadAllLines("memory.txt");
                for (int i = 0; i < savedData.Length; i += 2)
                {
                    if (i + 1 < savedData.Length)
                    {
                        expressions.Enqueue(savedData[i]);
                        if (double.TryParse(savedData[i + 1], out double result))
                        {
                            results.Enqueue(result);
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка при загрузке результата: '{savedData[i + 1]}' не является числом.");
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // Файл не найден - ничего не делаем  
            }
        }
        static void SaveMemory()
        {
            // Сохраняем последние выражения и результаты в файл   
            using (StreamWriter writer = new StreamWriter("memory.txt"))
            {
                // Сохраняем выражения  
                foreach (var expr in expressions)
                {
                    writer.WriteLine(expr);
                }
                // Сохраняем результаты  
                foreach (var res in results)
                {
                    writer.WriteLine(res);
                }
            }
        }
    }
}