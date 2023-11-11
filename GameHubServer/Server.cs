using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Reflection;
using System.Linq;
using Common.Attributes;
using Common.Messages;
using Common;
using Newtonsoft.Json;

namespace GameHubClient
{
    public class Server
    {
        private readonly HttpListener _httpListener;
        private readonly CancellationTokenSource _cts;
        private readonly List<Type> _messageTypes;
        private const string COMMON_ASSEMBLY_NAME = "Common";

        public Server(string url)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
            _cts = new CancellationTokenSource();

            // Get all available messages types
            Assembly commonAssembly = Assembly.Load(COMMON_ASSEMBLY_NAME);
            _messageTypes = commonAssembly
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.GetCustomAttribute<MessageAttribute>() != null)
                    .ToList();
        }

        public async Task StartAsync()
        {
            try
            {
                _httpListener.Start();
                Console.WriteLine("Game Server is listening...");

                while (true)
                {
                    var context = await _httpListener.GetContextAsync();

                    if (context.Request.IsWebSocketRequest)
                    {
                        var wsContext = await context.AcceptWebSocketAsync(subProtocol: null);

                        _ = Task.Run(async () => await HandleClient(wsContext.WebSocket));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"HTTP Listener error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during server execution: {ex.Message}");
            }
        }

        private async Task HandleClient(WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                while (!result.CloseStatus.HasValue)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    HandleWebSocketMessage(message, webSocket);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                }

                GameData.LogoutUser(webSocket);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
                GameData.LogoutUser(webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                GameData.LogoutUser(webSocket);
            }
        }

        public void HandleWebSocketMessage(string msg, WebSocket webSocket)
        {
            try
            {
                // Parse the incoming message to determine the command type
                dynamic msgObject = JsonConvert.DeserializeObject(msg);
                string commandType = msgObject.MsgType;

                // Find the command type based on the custom attribute
                var commandTypeToExecute = _messageTypes.FirstOrDefault(type =>
                    type.GetCustomAttribute<MessageAttribute>().MessageType == commandType);

                if (commandTypeToExecute != null)
                {
                    // Create an instance of the command class and execute it
                    IMessage message = Activator.CreateInstance(commandTypeToExecute) as IMessage;
                    message?.Handle(msgObject, webSocket);
                }
                else
                {
                    // Handle unknown command types
                    Console.WriteLine($"Unknown command type: {commandType}");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error handling incoming message: {ex.Message}");
            }
        }

        public void Stop()
        {
            GameData.LogoutAllUsers();
            _httpListener.Stop();
            _cts?.Cancel();
        }
    }
}
