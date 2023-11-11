using Common.Attributes;
using Common.Enums;
using Common.Models;
using Common.Utils;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("SendGift")]
    public class SendGiftMessage : Message
    {
        public int friendPlayerId;
        public ResourceType resourceType;
        public int resourceValue;

        public SendGiftMessage(int friendPlayerId, ResourceType resourceType, int resourceValue)
        {
            if (friendPlayerId < 0)
            {
                throw new ArgumentException("Friend player ID cannot be negative", nameof(friendPlayerId));
            }

            if (!Enum.IsDefined(typeof(ResourceType), resourceType))
            {
                throw new ArgumentException("Invalid resource type", nameof(resourceType));
            }

            if (resourceValue < 0)
            {
                throw new ArgumentException("Resource value cannot be negative", nameof(resourceValue));
            }

            this.friendPlayerId = friendPlayerId;
            this.resourceType = resourceType;
            this.resourceValue = resourceValue;
        }

        public SendGiftMessage() { }

        public override void InitializeParams(dynamic message)
        {
            if (Convert.ToInt32(message?.friendPlayerId) < 0 || !Enum.IsDefined(typeof(ResourceType), message?.resourceType) || Convert.ToInt32(message?.resourceValue) < 0)
            {
                throw new ArgumentException("Invalid parameters for initializing SendGiftMessage", nameof(message));
            }

            friendPlayerId = message.friendPlayerId;
            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            try
            {
                PlayerState? sendingUser = GameData.GetUserByWebSocket(returnWebSocket) ?? throw new Exception("Sending user was not found!");
                if(!sendingUser.Resources.TryGetValue(resourceType, out int currentBalance) || (currentBalance - resourceValue >= 0))
                {
                    throw new InvalidOperationException("Invalid balance for required operation!");
                }
                sendingUser.UpdateResource(resourceType, resourceValue * (-1));

                PlayerState? receivingUser = GameData.GetUserByPlayerId(friendPlayerId) ?? throw new Exception($"Receiving user {friendPlayerId} was not found!");
                receivingUser.UpdateResource(resourceType, resourceValue);

                if (receivingUser.WebSocket != null)
                {
                    _ = CommunicationUtils.Send(receivingUser.WebSocket, $"Hey! Player {sendingUser.PlayerId} just sent you a gift!");
                }

                _ = CommunicationUtils.Send(returnWebSocket, $"Gift was sent successfully to player {friendPlayerId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing send gift message: {ex.Message}");
            }
        }
    }
}
