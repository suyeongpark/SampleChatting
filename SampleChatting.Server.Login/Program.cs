using System;
using System.Threading.Tasks;

namespace SampleChatting.Server.Login
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task task = MainAsync(args);
            task.Wait();
        }

        async static Task MainAsync(string[] args)
        {
            Console.WriteLine("Hello World Async!");

            TcpServer server = new TcpServer();
            server.OnMessage += (msg) =>
            {
                Console.WriteLine(msg);
            };
            await server.StartListen();
        }
    }
}
