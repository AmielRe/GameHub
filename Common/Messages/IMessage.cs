using System.Net.WebSockets;

namespace Common.Messages
{
    /// <summary>
    /// Represents an interface for handling messages in the WebSocket communication.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Handles the incoming message and performs necessary actions.
        /// </summary>
        /// <param name="message">The dynamic message received.</param>
        /// <param name="returnWebSocket">The WebSocket to send a response if needed.</param>
        void Handle(dynamic message, WebSocket returnWebSocket);

        /// <summary>
        /// Initializes the parameters of the message from the dynamic input.
        /// </summary>
        /// <param name="message">The dynamic message received.</param>
        void InitializeParams(dynamic message);

        /// <summary>
        /// Processes the message and sends a response using the provided WebSocket.
        /// </summary>
        /// <param name="returnWebSocket">The WebSocket to send a response.</param>
        void ProcessAndRespond(WebSocket returnWebSocket);
    }
}
