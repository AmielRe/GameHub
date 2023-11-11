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
            if (!Enum.IsDefined(typeof(ResourceType), resourceType))
            {
                throw new ArgumentException("Invalid resource type", nameof(resourceType));
            }

            this.resourceType = resourceType;
            this.resourceValue = resourceValue;
        }

        public override void InitializeParams(dynamic message)
        {
            if (!Enum.IsDefined(typeof(ResourceType), message?.resourceType))
            {
                throw new ArgumentException("Invalid parameters for initializing UpdateResourcesMessage", nameof(message));
            }

            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            try
            {
                PlayerState? playerState = GameData.GetUserByWebSocket(returnWebSocket) ?? throw new Exception("Player was not found!");
                if (!playerState.Resources.TryGetValue(resourceType, out int currentBalance) || (currentBalance + resourceValue >= 0))
                {
                    throw new InvalidOperationException("Invalid balance for required operation!");
                }

                int newBalance = playerState.UpdateResource(resourceType, resourceValue);

                _ = CommunicationUtils.Send(returnWebSocket, newBalance.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing update resources message: {ex.Message}");
            }
        }
    }
}
