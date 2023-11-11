using Common.Attributes;
using Common.Enums;
using Common.Utils;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : Message
    {
        public int playerId;
        public ResourceType resourceType;
        public int resourceValue;

        public UpdateResourcesMessage() { }

        public UpdateResourcesMessage(int playerId, ResourceType resourceType, int resourceValue)
        {
            this.playerId = playerId;
            this.resourceType = resourceType;
            this.resourceValue = resourceValue;
        }

        public override void InitializeParams(dynamic message)
        {
            playerId = message.playerId;
            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            int? newBalance = GameData.UpdateResource(returnWebSocket, resourceType, resourceValue);

            if (!newBalance.HasValue)
            {
                newBalance = 0;
            }

            _ = CommunicationUtils.Send(returnWebSocket, newBalance.Value.ToString());
        }
    }
}
