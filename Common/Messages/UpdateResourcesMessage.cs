using Common.Attributes;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : Message
    {
        public UpdateResourcesMessage() { }

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
