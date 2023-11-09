using System.Net.WebSockets;
using System.Text;

namespace Common.Utils
{
    public static class CommunicationUtils
    {
        public static async Task Send(ClientWebSocket webSocket, string message)
        {
            byte[] buffer = new UTF8Encoding().GetBytes(message);

            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);

            Console.WriteLine("Sent:     " + message);
        }

        public static async Task<string> Receive(ClientWebSocket webSocket)
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
                Console.WriteLine("Receive:  " + Encoding.UTF8.GetString(buffer).TrimEnd('\0'));
                return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            }
        }
    }
}
