using Common.Attributes;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("SendGift")]
    public class SendGiftMessage : Message
    {
        public SendGiftMessage() { }
        override public void Handle(WebSocket returnWebSocket)
        {
            throw new NotImplementedException();
        }

        public override void InitializeParams(dynamic message)
        {
            throw new NotImplementedException();
        }
    }
}
