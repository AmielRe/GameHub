using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Utils
{
    /// <summary>
    /// Utility class for WebSocket communication.
    /// </summary>
    public static class CommunicationUtils
    {
        /// <summary>
        /// Sends a message over the specified WebSocket connection.
        /// </summary>
        /// <param name="webSocket">The WebSocket connection.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="WebSocketException">Thrown if a WebSocket communication error occurs during send.</exception>
        public static async Task Send(WebSocket webSocket, string message)
        {
            try
            {
                byte[] buffer = new UTF8Encoding().GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                // Handle WebSocket communication errors during send
                throw new WebSocketException($"WebSocket communication error during send: {ex.Message}");
            }
        }

        /// <summary>
        /// Receives a message from the specified WebSocket connection.
        /// </summary>
        /// <param name="webSocket">The WebSocket connection.</param>
        /// <returns>The received message as a string.</returns>
        /// <exception cref="WebSocketException">Thrown if a WebSocket communication error occurs during receive.</exception>
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
                // Handle WebSocket communication errors during receive
                throw new WebSocketException($"WebSocket communication error during receive: {ex.Message}");
            }
        }
    }
}
