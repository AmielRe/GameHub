using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Common.Utils;
using Newtonsoft.Json;
using Common.Messages;
using Common.Enums;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;

namespace GameHubClient
{
    /// <summary>
    /// Represents a client for interacting with a game server via WebSocket.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The entry point of the client application.
        /// </summary>
        /// <param name="args">Command-line arguments (unused).</param>
        static async Task Main(string[] args)
        {
            // Configure logger
            Log.Logger = new LoggerConfiguration()
                            // add a logging target for warnings and higher severity  logs
                            // structured in JSON format
                            .WriteTo.File(new JsonFormatter(),
                                          "important.json",
                                          restrictedToMinimumLevel: LogEventLevel.Warning)
                            // add a rolling file for all logs
                            .WriteTo.File("all-.logs",
                                          rollingInterval: RollingInterval.Day)
                            // set default minimum level
                            .MinimumLevel.Debug()
                            .CreateLogger();

            // Establish WebSocket connection to the game server
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
                                // Send login message to the server
                                await CommunicationUtils.Send(webSocket, loginMsg);

                                // Receive and set the player ID from the server
                                playerId = CommunicationUtils.Receive(webSocket).Result;
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Communication error: {0}", ex.Message);
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
                                    // Send resource update message to the server
                                    await CommunicationUtils.Send(webSocket, updateResourcesMsg);

                                    // Receive and display the new resource balance from the server
                                    string newBalance = CommunicationUtils.Receive(webSocket).Result;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Communication error: {0}", ex.Message);
                                }
                            }
                            else
                            {
                                Log.Error("Please login before trying to update a resource");
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
                                    // Send gift message to the server
                                    await CommunicationUtils.Send(webSocket, sendGiftMsg);

                                    // Receive and display the server's response
                                    string response = CommunicationUtils.Receive(webSocket).Result;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error("Communication error: {0}", ex.Message);
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

        /// <summary>
        /// Establishes a WebSocket connection to the specified URI.
        /// </summary>
        /// <param name="uri">The URI of the WebSocket server.</param>
        /// <returns>A connected WebSocket instance.</returns>
        public static async Task<ClientWebSocket> Connect(string uri)
        {
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();

                // Connect to the WebSocket server
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to connect to: {0} - {1}", uri, ex.Message);
                throw;
            }

            return webSocket;
        }

        /// <summary>
        /// Prompts the user to choose a resource type from the available options.
        /// </summary>
        /// <returns>The chosen resource type.</returns>
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

        /// <summary>
        /// Prompts the user to enter a resource value.
        /// </summary>
        /// <param name="allowNegative">Specifies whether negative values are allowed.</param>
        /// <returns>The entered resource value.</returns>
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

        /// <summary>
        /// Prompts the user to enter a friend's ID.
        /// </summary>
        /// <returns>The entered friend's ID.</returns>
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
