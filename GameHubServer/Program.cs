using GameHubClient;
using System;

namespace GameHubServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Server websocketServer = new Server("http://localhost:8080/");

            _ = websocketServer.StartAsync();

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();

            websocketServer.Stop();
        }
    }
}
