using Common.Attributes;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : Message
    {
        public UpdateResourcesMessage() { }

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
