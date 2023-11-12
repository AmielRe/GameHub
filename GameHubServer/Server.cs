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
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace GameHubClient
{
    /// <summary>
    /// Represents a WebSocket server for handling game-related communication.
    /// </summary>
    public class Server
    {
        private readonly HttpListener _httpListener;
        private readonly CancellationTokenSource _cts;
        private readonly List<Type> _messageTypes;
        private const string COMMON_ASSEMBLY_NAME = "Common";

        /// <summary>
        /// Initializes a new instance of the Server class.
        /// </summary>
        /// <param name="url">The URL on which the server should listen.</param>
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
        }

        /// <summary>
        /// Starts the server and listens for WebSocket connections.
        /// </summary>
        public async Task StartAsync()
        {
            try
            {
                _httpListener.Start();
                Log.Information("Game Server is listening...");

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
                        // Respond with a 400 Bad Request for non-WebSocket requests
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        Log.Warning("Non web socket request denied");
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                Log.Error($"HTTP Listener error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error during server execution: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles WebSocket communication with a connected client.
        /// </summary>
        /// <param name="webSocket">The WebSocket associated with the connected client.</param>
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

                // Logout the user when the WebSocket connection is closed
                GameData.LogoutUser(webSocket);
            }
            catch (WebSocketException ex)
            {
                Log.Error($"WebSocket error: {ex.Message}");
                GameData.LogoutUser(webSocket);
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex.Message}");
                GameData.LogoutUser(webSocket);
            }
        }

        /// <summary>
        /// Handles incoming WebSocket messages.
        /// </summary>
        /// <param name="msg">The incoming WebSocket message.</param>
        /// <param name="webSocket">The WebSocket associated with the connected client.</param>
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
                    Log.Warning($"Unknown command type: {commandType}");
                }
            }
            catch (JsonException ex)
            {
                Log.Error($"Error deserializing JSON: {ex.Message}");
            }
            catch(Exception ex)
            {
                Log.Error($"Error handling incoming message: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the server and logs out all connected users.
        /// </summary>
        public void Stop()
        {
            GameData.LogoutAllUsers();
            _httpListener.Stop();
            _cts?.Cancel();
        }
    }
}
