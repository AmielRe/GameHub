using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Common.Utils;
using Newtonsoft.Json;
using Common.Messages;

namespace GameHubClient
{
    public class Client
    {
        static async Task Main(string[] args)
        {
            ClientWebSocket webSocket = Connect("ws://localhost:8080/").Result;
            string playerId = null;

            while (true)
            {
                //Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Update Resources");
                Console.WriteLine("3. SendGift");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice (1-4): ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            string ipAddress = NetworkUtils.GetIPv4Address();
                            LoginMessage login = new LoginMessage(ipAddress);

                            string loginMsg = JsonConvert.SerializeObject(login);

                            await CommunicationUtils.Send(webSocket, loginMsg);

                            playerId = CommunicationUtils.Receive(webSocket).Result;
                            break;
                        case 2:
                            if(playerId != null)
                            {
                                UpdateResourcesMessage updateResources = new UpdateResourcesMessage();

                                string updateResourcesMsg = JsonConvert.SerializeObject(updateResources);

                                await CommunicationUtils.Send(webSocket, updateResourcesMsg);
                            }
                            else
                            {
                                Console.WriteLine("Please login before trying to update a resource");
                            }
                            break;
                        case 3:
                            if (playerId != null)
                            {
                                SendGiftMessage sendGift = new SendGiftMessage();

                                string sendGiftMsg = JsonConvert.SerializeObject(sendGift);

                                await CommunicationUtils.Send(webSocket, sendGiftMsg);
                            }
                            else
                            {
                                Console.WriteLine("Please login before trying to send a gift");
                            }
                            break;
                        case 4:
                            Console.WriteLine("Goodbye!");
                            webSocket.Abort();
                            return; // Exit the program
                        default:
                            Console.WriteLine("Invalid choice. Please select a valid option (1-4).");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        public static async Task<ClientWebSocket> Connect(string uri)
        {
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();

                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to: {0} - {1}", uri, ex.Message);
            }

            return webSocket;
        }
    }
}
