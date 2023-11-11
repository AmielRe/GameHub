using Common.Attributes;
using System.Net.WebSockets;

namespace Common.Messages
{
    /// <summary>
    /// Represents the base class for messages exchanged in the WebSocket communication.
    /// </summary>
    public abstract class Message : IMessage
    {
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public string MsgType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        protected Message()
        {
            // Extract MessageType from the attribute
            var messageTypeAttribute = (MessageAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MessageAttribute));
            MsgType = messageTypeAttribute?.MessageType ?? string.Empty;
        }

        /// <summary>
        /// Handles the incoming message and performs necessary actions.
        /// </summary>
        /// <param name="message">The dynamic message received.</param>
        /// <param name="returnWebSocket">The WebSocket to send a response if needed.</param>
        public void Handle(dynamic message, WebSocket returnWebSocket)
        {
            InitializeParams(message);
            ProcessAndRespond(returnWebSocket);
        }

        /// <summary>
        /// Initializes the parameters of the message from the dynamic input.
        /// </summary>
        /// <param name="message">The dynamic message received.</param>
        public abstract void InitializeParams(dynamic message);

        /// <summary>
        /// Processes the message and sends a response using the provided WebSocket.
        /// </summary>
        /// <param name="returnWebSocket">The WebSocket to send a response.</param>
        public abstract void ProcessAndRespond(WebSocket returnWebSocket);
    }
}
