using Common.Attributes;
using Common.Enums;
using Common.Models;
using Common.Utils;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : Message
    {
        public ResourceType resourceType;
        public int resourceValue;

        public UpdateResourcesMessage() { }

        public UpdateResourcesMessage(ResourceType resourceType, int resourceValue)
        {
            this.resourceType = resourceType;
            this.resourceValue = resourceValue;
        }

        public override void InitializeParams(dynamic message)
        {
            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            PlayerState? playerState = GameData.GetUserByWebSocket(returnWebSocket) ?? throw new Exception("Player was not found!");

            int newBalance = playerState.UpdateResource(resourceType, resourceValue);

            _ = CommunicationUtils.Send(returnWebSocket, newBalance.ToString());
        }
    }
}
