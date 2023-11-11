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
            this.friendPlayerId = friendPlayerId;
            this.resourceType = resourceType;
            this.resourceValue = resourceValue;
        }

        public SendGiftMessage() { }

        public override void InitializeParams(dynamic message)
        {
            friendPlayerId = message.friendPlayerId;
            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            PlayerState? sendingUser = GameData.GetUserByWebSocket(returnWebSocket) ?? throw new Exception("Player was not found!");
            
            sendingUser.UpdateResource(resourceType, resourceValue * (-1));

            PlayerState? recevingUser = GameData.GetUserByPlayerId(friendPlayerId) ?? throw new Exception($"Player {friendPlayerId} was not found!");

            recevingUser.UpdateResource(resourceType, resourceValue);

            if(recevingUser.WebSocket != null)
            {
                _ = CommunicationUtils.Send(recevingUser.WebSocket, $"Hey! player {sendingUser.PlayerId} just sent you a gift!");
            }

            _ = CommunicationUtils.Send(returnWebSocket, $"Gift was sent successfully to player {friendPlayerId}");
        }
    }
}
