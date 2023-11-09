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

namespace GameHubClient
{
    public class Server
    {
        private HttpListener _httpListener;
        private CancellationTokenSource _cts;
        private Dictionary<int, WebSocket> _connectedClients = new Dictionary<int, WebSocket>();
        private List<Type> _messageTypes;

        public Server(string url)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
            _cts = new CancellationTokenSource();

            // Get all available messages types
            _messageTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.GetCustomAttribute<MessageAttribute>() != null)
                    .ToList();
        }

        public async Task StartAsync()
        {
            _httpListener.Start();
            Console.WriteLine("Game Server is listening...");

            while (true)
            {
                var context = await _httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                    int playerId = Guid.NewGuid().GetHashCode();
                    _connectedClients[playerId] = wsContext.WebSocket;

                    _ = Task.Run(async () => await HandleClient(playerId, wsContext.WebSocket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleClient(int playerId, WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                while (!result.CloseStatus.HasValue)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    HandleWebSocketMessage(message);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                }

                _connectedClients.Remove(playerId);
            }
            catch (Exception ex)
            {
                // Handle exceptions and disconnect client
                Console.WriteLine($"Error: {ex.Message}");
                _connectedClients.Remove(playerId);
            }
        }

        public void HandleWebSocketMessage(string msg)
        {
            // Parse the incoming message to determine the command type
            dynamic msgObject = Newtonsoft.Json.JsonConvert.DeserializeObject(msg);
            string commandType = msgObject.type;

            // Find the command type based on the custom attribute
            var commandTypeToExecute = _messageTypes.FirstOrDefault(type =>
                type.GetCustomAttribute<MessageAttribute>().MessageType == commandType);

            if (commandTypeToExecute != null)
            {
                // Create an instance of the command class and execute it
                IMessage message = Activator.CreateInstance(commandTypeToExecute, msg) as IMessage;
                message?.Handle(msgObject);
            }
            else
            {
                // Handle unknown command types
            }
        }

        public void Stop()
        {
            _httpListener.Stop();
            _cts?.Cancel();
        }
    }
}
