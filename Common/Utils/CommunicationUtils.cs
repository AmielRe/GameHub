using System.Net.WebSockets;
using System.Text;

namespace Common.Utils
{
    public static class CommunicationUtils
    {
        public static async Task Send(WebSocket webSocket, string message)
        {
            try
            {
                byte[] buffer = new UTF8Encoding().GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                throw new WebSocketException($"WebSocket communication error during send: {ex.Message}");
            }
        }

        public static async Task<string> Receive(WebSocket webSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(buffer, 0, result.Count).TrimEnd('\0');
                }
            }
            catch (WebSocketException ex)
            {
                throw new WebSocketException($"WebSocket communication error during receive: {ex.Message}");
            }
        }

    }
}
