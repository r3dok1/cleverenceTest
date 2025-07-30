using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Test1.Compressor
{
    class Program
    {
        static string Compress(string s)
        {
            var sb = new StringBuilder();
            char prev = s[0];
            int count = 1;

            for (int i = 1; i < s.Length; i++)
            {
                if (s[i] == prev)
                    count++;
                else
                {
                    sb.Append(prev);
                    if (count > 1) sb.Append(count);
                    prev = s[i];
                    count = 1;
                }
            }
            sb.Append(prev);
            if (count > 1) sb.Append(count);

            return sb.ToString();
        }

        static bool IsValidInput(string input)
        {
            // Регулярное выражение: строка из одной или более строчных латинских букв
            return Regex.IsMatch(input, @"^[a-z]+$");
        }

        static void Main()
        {
            Console.WriteLine("Введите строку из строчных латинских букв (или 'exit' для выхода):");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "exit") break;

                if (!IsValidInput(input))
                {
                    Console.WriteLine("Ошибка: допустимы только строчные латинские буквы и непустая строка.");
                    continue;
                }

                string compressed = Compress(input);
                Console.WriteLine($"Результат компрессии: {compressed}");
            }

            Console.WriteLine("Работа завершена.");
        }
    }
}
