using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameHubClient
{
    public class Server
    {
        private HttpListener _httpListener;
        private CancellationTokenSource _cts;
        private Dictionary<int, WebSocket> _connectedClients = new Dictionary<int, WebSocket>();

        public Server(string url)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
            _cts = new CancellationTokenSource();
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
                    HandleMessage(message);

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

        private void HandleMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void Stop()
        {
            _httpListener.Stop();
            _cts?.Cancel();
        }
    }
}
