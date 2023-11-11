using Common.Attributes;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("SendGift")]
    public class SendGiftMessage : Message
    {
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
