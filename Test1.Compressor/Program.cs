using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Test1.Compressor
{
    class Program
    {
        static string Compress(string s)
        {
            if (!IsValidInputToCompress(s))
                return "Данная строка не поддерживается для компрессии " + s;
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
        public static string Decompress(string compressed)
        {
            StringBuilder result = new StringBuilder();
            int index = 0;
            if (!IsValidInputToDecompress(compressed))
                return "Данная строка не поддерживается для декомпрессии "+ compressed;

            while (index < compressed.Length)
            {
                char currentChar = compressed[index];
                // Есть ли следом цифра?
                if (index + 1 < compressed.Length && char.IsDigit(compressed[index + 1]))
                {
                    // Число может содержать несколько цифр
                    string numberStr = "";
                    while (index + 1 < compressed.Length && char.IsDigit(compressed[index + 1]))
                    {
                        numberStr += compressed[index + 1];
                        index++;
                    }
                    int repeatCount = int.Parse(numberStr); // преобразование строки в целое число
                    result.Append(new string(currentChar, repeatCount)); 
                }
                else
                {
                    result.Append(currentChar); // одиночная буква
                }
                index++;
            }

            return result.ToString();
        }

        static bool IsValidInputToCompress(string input)
        {
            // Регулярное выражение: строка из одной или более строчных латинских букв
            return Regex.IsMatch(input, @"^[a-z]+$");
        }
        
        static bool IsValidInputToDecompress(string input)
        {
            // Регулярное выражение: строка из одной или более строчных латинских букв и цифр, для работы без повторов одинаковых букв используется отрицательная ретроспективная проверка.
            return Regex.IsMatch(input, @"^(?!.*(.)(\1))[a-z0-9]+$");
        }
        private static void CompressInput()
        {
            Console.Write("Введите строку для компрессии: ");
            string input = Console.ReadLine().Trim();
            string compressed = Compress(input);
            Console.WriteLine($"\nСжатая строка: {compressed}");
        }

        private static void DecompressInput()
        {
            Console.Write("Введите сжатую строку для декомпрессии: ");
            string input = Console.ReadLine().Trim();
            string decompressed = Decompress(input);
            Console.WriteLine($"\nРасжатая строка: {decompressed}");
        }

        static void Main()
        {
            Console.WriteLine("Введите строку из строчных латинских букв (или 'exit' для выхода):");
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Компрессия");
                Console.WriteLine("2. Декомпрессия");
                Console.WriteLine("3. Выход");
                var choice = Console.ReadKey(true).KeyChar;

                switch (choice)
                {
                    case '1':
                        CompressInput();
                        break;
                    case '2':
                        DecompressInput();
                        break;
                    case '3':
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\nНеверный выбор.");
                        break;
                }
            }

            Console.WriteLine("Работа завершена.");
        }
    }
}
