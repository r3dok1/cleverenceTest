using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Test3.LogStandardizer
{
    class LogStandardizer
    {
        // Формат 1: 10.03.2025 15:14:49.523 INFORMATION Сообщение…
        static readonly Regex _format1 = new Regex(
            @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+" +
            @"(?<time>\d{2}:\d{2}:\d{2}\.\d+)\s+" +
            @"(?<level>INFORMATION|WARNING|ERROR|DEBUG)\s+" +
            @"(?<message>.+)$",
            RegexOptions.Compiled);

        // Формат 2: 2025-03-10 15:14:51.5882| INFO|11|Mobile…| Сообщение…
        static readonly Regex _format2 = new Regex(
            @"^(?<date>\d{4}-\d{2}-\d{2})\s+" +
            @"(?<time>\d{2}:\d{2}:\d{2}\.\d+)\|\s*" +
            @"(?<level>[A-Za-z]+)\|\d+\|(?<method>[\w\.]+)\|\s*" +
            @"(?<message>.+)$",
            RegexOptions.Compiled);

        static void ProcessFile(string inputPath)
        {
            var outputPath = "output.txt";
            var problemPath = "problems.txt";

            bool append = true; // переменная нужна только для избавления от магического була, т.е. облегчить чтение, можно было бы вывести в Define

            using var reader = new StreamReader(inputPath);
            using var writer = new StreamWriter(outputPath, append);
            using var problem = new StreamWriter(problemPath, append);

            Console.WriteLine();
            Console.WriteLine($"Старт обработки '{inputPath}'");
            Console.WriteLine("------------------------------------");
            
            string line;
            int lineNum = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNum++;
                try
                {
                    if (TryParse(line, out var stdLine))
                    {
                        writer.WriteLine(stdLine);
                        Console.WriteLine("Успех: " + stdLine);
                    }
                    else
                    {
                        problem.WriteLine(line);
                        Console.WriteLine("Неподходящий формат: " + line);
                    }
                }
                catch (Exception ex)
                {
                    problem.WriteLine($"[Ошибка: {ex.Message}] {line}");
                    Console.WriteLine($"Ошибка: {ex.Message} | {line}");
                }
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Готово! Результаты: '{outputPath}', ошибки: '{problemPath}'");
        }

        static bool TryParse(string line, out string result)
        {
            result = null;

            string MapLevel(string lvl) =>
                lvl.ToUpper() switch
                {
                    "INFORMATION" => "INFO",
                    "WARNING" => "WARN",
                    "ERROR" => "ERROR",
                    "DEBUG" => "DEBUG",
                    _ => lvl.ToUpper()
                };

            var m1 = _format1.Match(line);
            if (m1.Success)
            {
                var d = DateTime.ParseExact(m1.Groups["date"].Value, "dd.MM.yyyy", null);
                var date = d.ToString("dd-MM-yyyy");
                var time = m1.Groups["time"].Value;
                var lvl = MapLevel(m1.Groups["level"].Value);
                var msg = m1.Groups["message"].Value.Trim();

                result = $"{date}\t{time}\t{lvl}\tDEFAULT\t{msg}";
                return true;
            }

            var m2 = _format2.Match(line);
            if (m2.Success)
            {
                var parsedDate = DateTime.ParseExact(m2.Groups["date"].Value, "yyyy-MM-dd", null);
                var date = parsedDate.ToString("dd-MM-yyyy");
                var time = m2.Groups["time"].Value;
                var lvl = MapLevel(m2.Groups["level"].Value);
                var method = m2.Groups["method"].Value;
                var msg = m2.Groups["message"].Value.Trim();

                result = $"{date}\t{time}\t{lvl}\t{method}\t{msg}";
                return true;
            }

            return false;
        }
        
        static void Main()
        {
            while (true)
            {
                Console.Write("Путь к входному файлу (или 'exit' для выхода): ");
                string inputPath = Console.ReadLine()?.Trim();

                if (string.Equals(inputPath, "exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
                {
                    Console.WriteLine("Файл не найден или путь пуст. Нажмите любую клавишу и попробуйте снова.");
                    Console.ReadKey();
                    continue;
                }

                ProcessFile(inputPath);

                Console.WriteLine();
                Console.Write("Хотите обработать другой файл? (Y/N): ");
                var key = Console.ReadKey();
                if (key.KeyChar == 'Y' || key.KeyChar == 'y')
                {
                    Console.WriteLine();
                    continue;
                }
                break;
            }
        }
    }
}