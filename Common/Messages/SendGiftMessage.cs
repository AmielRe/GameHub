using Common.Attributes;
using Common.Enums;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("SendGift")]
    public class SendGiftMessage : Message
    {
        public int playerId;
        public int friendPlayerId;
        public ResourceType resourceType;
        public int resourceValue;

        public SendGiftMessage() { }

        public override void InitializeParams(dynamic message)
        {
            throw new NotImplementedException();
        }

        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            throw new NotImplementedException();
        }
    }
}
