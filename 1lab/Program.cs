using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Необходимо указать аргумент для выполнения задачи.");
            Console.WriteLine("Возможные аргументы: temperature, palindrome, rabbits, csv");
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "temperature":
                ConvertTemperatureTask();
                break;
            case "palindrome":
                PalindromeTask();
                break;
            case "rabbits":
                RabbitReproductionTask();
                break;
            case "csv":
                FileStatisticsTask();
                break;
            default:
                Console.WriteLine("Некорректный аргумент. Возможные варианты: temperature, palindrome, rabbits, csv");
                break;
        }
    }

    static void ConvertTemperatureTask()
    {
        double variable;
        double value;
        Console.Write("Введите значение температуры: ");
        string write = Console.ReadLine();
        if (!double.TryParse(write,out variable)){
            Console.WriteLine("Ошибка ввода данных.Введите число.");
            return;
        }
        else {
           value = Convert.ToDouble(write);
        }

        // double value = Convert.ToDouble(Console.ReadLine());
        

        Console.Write("Введите текущую шкалу (C, K, F): ");
        string fromScale = Console.ReadLine().ToUpper();

        Console.Write("Введите шкалу для перевода (C, K, F): ");
        string toScale = Console.ReadLine().ToUpper();

        double result = ConvertTemperature(value, fromScale, toScale);

        if (double.IsNaN(result))
        {
            Console.WriteLine("Ошибка: введены некорректные шкалы.");
        }
        else
        {
            Console.WriteLine($"Результат: {result} {toScale}");
        }
    }

    static double ConvertTemperature(double value, string fromScale, string toScale)
    {
        if (fromScale == toScale)
        {
            return value; // Если шкалы совпадают, возвращаем исходное значение
        }

        double celsius;

        switch (fromScale)
        {
            case "C":
                celsius = value;
                break;
            case "K":
                celsius = value - 273.15;
                break;
            case "F":
                celsius = (value - 32) * 5 / 9;
                break;
            default:
                return double.NaN;
        }

        double result;

        switch (toScale)
        {
            case "C":
                result = celsius;
                break;
            case "K":
                result = celsius + 273.15;
                break;
            case "F":
                result = (celsius * 9 / 5) + 32;
                break;
            default:
                return double.NaN;
        }

        return result;
    }

    static void PalindromeTask()
    {
        Console.Write("Введите слово: ");
        string input = Console.ReadLine();

        if (IsPalindrome(input))
        {
            Console.WriteLine("Слово является палиндромом.");
        }
        else
        {
            Console.WriteLine("Слово не является палиндромом.");
        }
    }

    static bool IsPalindrome(string word)
    {
        word = word.ToLower().Replace(" ", "");

        int length = word.Length;
        for (int i = 0; i < length / 2; i++)
        {
            if (word[i] != word[length - i - 1])
            {
                return false;
            }
        }
        return true;
    }

    static void RabbitReproductionTask()
    {
        Console.Write("Введите количество месяцев: ");
        int months = Convert.ToInt32(Console.ReadLine());

        if (months < 1)
        {
            Console.WriteLine("Некорректное количество месяцев.");
        }
        else
        {
            long rabbitPairs = CalculateRabbitPairs(months);
            Console.WriteLine($"Количество пар кроликов через {months} месяцев: {rabbitPairs}");
        }
    }

    static long CalculateRabbitPairs(int months)
    {
        if (months == 1 || months == 2)
        {
            return 1;
        }

        long previous = 1, current = 1, next = 0;
        for (int i = 3; i <= months; i++)
        {
            next = previous + current;
            previous = current;
            current = next;
        }

        return current;
    }

    static void FileStatisticsTask()
    {
        Console.Write("Введите путь к CSV файлу: ");
        string filePath = Console.ReadLine();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не найден.");
            return;
        }

        List<double> data = ReadCsvData(filePath);

        if (data.Count == 0)
        {
            Console.WriteLine("Файл не содержит данных или данные некорректны.");
            return;
        }

        Console.WriteLine("Выберите статистическую операцию:");
        Console.WriteLine("a. Максимум");
        Console.WriteLine("b. Минимум");
        Console.WriteLine("c. Среднее значение");
        Console.WriteLine("d. Исправленная выборочная дисперсия");
        Console.Write("Введите выбор (a, b, c или d): ");
        string operation = Console.ReadLine();

        switch (operation)
        {
            case "a":
                Console.WriteLine($"Максимум: {data.Max()}");
                break;
            case "b":
                Console.WriteLine($"Минимум: {data.Min()}");
                break;
            case "c":
                Console.WriteLine($"Среднее значение: {data.Average()}");
                break;
            case "d":
                double variance = CalculateSampleVariance(data);
                Console.WriteLine($"Исправленная выборочная дисперсия: {variance}");
                break;
            default:
                Console.WriteLine("Некорректный выбор.");
                break;
        }
    }

    static List<double> ReadCsvData(string filePath)
{
    List<double> data = new List<double>();

    using (var reader = new StreamReader(filePath))
    {

        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            if (values.Length >= 2)
            {
                if (double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    data.Add(number);
                }
            }
        }
    }

    return data;
}

    static double CalculateSampleVariance(List<double> data)
    {
        double mean = data.Average();
        double sumOfSquares = data.Sum(x => Math.Pow(x - mean, 2));
        return sumOfSquares / (data.Count - 1);
    }
}
