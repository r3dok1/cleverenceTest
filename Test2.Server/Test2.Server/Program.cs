using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test2.Server
{
    public static class Server
    {
        private static int count = 0;
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public static int GetCount()
        {
            locker.EnterReadLock();
            try
            {
                return count;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            locker.EnterWriteLock();
            try
            {
                count += value;
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Старт симуляции...");

            // Запускаем 10 читателей
            for (int i = 0; i < 10; i++)
            {
                int readerId = i + 1;
                Task.Run(() =>
                {
                    while (true)
                    {
                        int value = Server.GetCount();
                        Console.WriteLine($"[Читатель {readerId}] прочитал count = {value}");
                        Thread.Sleep(new Random().Next(200, 800)); // симуляция задержки
                    }
                });
            }

            // Запускаем 2 писателя
            for (int i = 0; i < 2; i++)
            {
                int writerId = i + 1;
                Task.Run(() =>
                {
                    while (true)
                    {
                        int delta = new Random().Next(1, 5);
                        Server.AddToCount(delta);
                        Console.WriteLine($"[Писатель {writerId}] увеличил count на {delta}");
                        Thread.Sleep(new Random().Next(1000, 2000)); // симуляция задержки между записями
                    }
                });
            }

            Console.WriteLine("Нажмите Enter для завершения...");
            Console.ReadLine();
        }
    }
}