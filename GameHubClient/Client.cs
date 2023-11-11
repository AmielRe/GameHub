using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Common.Utils;
using Newtonsoft.Json;
using Common.Messages;
using Common.Enums;

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

                            LoginMessage login = new(ipAddress);

                            string loginMsg = JsonConvert.SerializeObject(login);

                            try
                            {
                                await CommunicationUtils.Send(webSocket, loginMsg);
                                playerId = CommunicationUtils.Receive(webSocket).Result;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Communication error: {0}", ex.Message);
                            }
                            break;
                        case 2:
                            if(playerId != null)
                            {
                                ResourceType chosenResourceType = PromptForResourceType();
                                int chosenResourceValue = PromptForResourceValue();
                                UpdateResourcesMessage updateResources = new(chosenResourceType, chosenResourceValue);

                                string updateResourcesMsg = JsonConvert.SerializeObject(updateResources);

                                try
                                {
                                    await CommunicationUtils.Send(webSocket, updateResourcesMsg);
                                    string newBalance = CommunicationUtils.Receive(webSocket).Result;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Communication error: {0}", ex.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Please login before trying to update a resource");
                            }
                            break;
                        case 3:
                            if (playerId != null)
                            {
                                ResourceType chosenResourceType = PromptForResourceType();
                                int chosenResourceValue = PromptForResourceValue(false);
                                int friendId = PromptForFriendId();

                                SendGiftMessage sendGift = new(friendId, chosenResourceType, chosenResourceValue);

                                string sendGiftMsg = JsonConvert.SerializeObject(sendGift);

                                try
                                {
                                    await CommunicationUtils.Send(webSocket, sendGiftMsg);
                                    string response = CommunicationUtils.Receive(webSocket).Result;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Communication error: {0}", ex.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Please login before trying to send a gift");
                            }
                            break;
                        case 4:
                            Console.WriteLine("Goodbye!");
                            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
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
                throw;
            }

            return webSocket;
        }

        static ResourceType PromptForResourceType()
        {
            Console.WriteLine("Choose a resource type:");

            // Dynamically display enum values
            int enumIndex = 1;
            foreach (var resourceType in Enum.GetNames(typeof(ResourceType)))
            {
                Console.WriteLine($"{enumIndex}. {resourceType}");
                enumIndex++;
            }

            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > enumIndex - 1)
            {
                Console.WriteLine($"Invalid choice. Please enter a number between 1 and {enumIndex - 1}.");
            }

            return (ResourceType)(choice - 1);
        }

        static int PromptForResourceValue(bool allowNegative = true)
        {
            Console.WriteLine("Enter the resource value:");
            int value;
            while (!int.TryParse(Console.ReadLine(), out value) || (!allowNegative && value < 0))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }

            return value;
        }

        static int PromptForFriendId()
        {
            Console.WriteLine("Enter the friend id:");
            int value;
            while (!int.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid non negative integer.");
            }

            return value;
        }
    }
}
