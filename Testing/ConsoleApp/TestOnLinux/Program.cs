using System;
using System.Threading.Tasks;

namespace TestOnLinux
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Hello async World!");

            try
            {
                while (true)
                {
                    Console.WriteLine(DateTime.UtcNow.ToString());
                    await Task.Delay(1000);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
