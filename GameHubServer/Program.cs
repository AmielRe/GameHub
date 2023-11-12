using Common;
using GameHubClient;
using System;

namespace GameHubServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Server websocketServer = new($"http://{Constants.SERVER_HOST}:{Constants.SERVER_PORT}/");

            _ = websocketServer.StartAsync();

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();

            websocketServer.Stop();
        }
    }
}
