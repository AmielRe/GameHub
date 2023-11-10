using Common.Attributes;
using System.Net.WebSockets;

namespace Common.Messages
{
    public abstract class Message : IMessage
    {
        public string MsgType {  get; }

        public Message()
        {
            // Extract MessageType from the attribute
            var messageTypeAttribute = (MessageAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MessageAttribute));
            if (messageTypeAttribute != null)
            {
                MsgType = messageTypeAttribute.MessageType;
            }
            else
            {
                MsgType = string.Empty;
            }
        }

        public abstract void Handle(WebSocket returnWebSocket);
        public abstract void InitializeParams(dynamic message);
    }
}
