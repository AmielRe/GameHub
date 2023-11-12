using Common.Attributes;
using Common.Enums;
using Common.Models;
using Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net.WebSockets;

namespace Common.Messages
{
    /// <summary>
    /// Represents a message for sending a gift between players.
    /// </summary>
    [Message("SendGift")]
    public class SendGiftMessage : Message
    {
        /// <summary>
        /// Gets or sets the player ID of the friend who will receive the gift.
        /// </summary>
        public int friendPlayerId;

        /// <summary>
        /// Gets or sets the type of resource to be sent as a gift.
        /// </summary>
        public ResourceType resourceType;

        /// <summary>
        /// Gets or sets the value of the resource to be sent as a gift.
        /// </summary>
        public int resourceValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGiftMessage"/> class with the specified parameters.
        /// </summary>
        /// <param name="friendPlayerId">The player ID of the friend who will receive the gift.</param>
        /// <param name="resourceType">The type of resource to be sent as a gift.</param>
        /// <param name="resourceValue">The value of the resource to be sent as a gift.</param>
        /// <exception cref="ArgumentException">Thrown if any of the parameters are invalid (negative friend player ID, invalid resource type, or negative resource value).</exception>
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

        /// <summary>
        /// Default constructor for <see cref="SendGiftMessage"/>.
        /// </summary>
        public SendGiftMessage() { }

        /// <inheritdoc/>
        public override void InitializeParams(dynamic message)
        {
            if (Convert.ToInt32(message?.friendPlayerId) < 0 || !Enum.IsDefined(typeof(ResourceType), (int?)message?.resourceType) || Convert.ToInt32(message?.resourceValue) < 0)
            {
                throw new ArgumentException("Invalid parameters for initializing SendGiftMessage", nameof(message));
            }

            friendPlayerId = message.friendPlayerId;
            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        /// <inheritdoc/>
        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            try
            {
                // Get sending user and check he has enough balance for the gift
                PlayerState? sendingUser = GameData.GetUserByWebSocket(returnWebSocket) ?? throw new Exception("Sending user was not found!");
                if (!sendingUser.Resources.TryGetValue(resourceType, out int currentBalance) || (currentBalance - resourceValue < 0))
                {
                    throw new InvalidOperationException("Invalid balance for required operation!");
                }

                // Get receiving user
                PlayerState? receivingUser = GameData.GetUserByPlayerId(friendPlayerId) ?? throw new Exception($"Receiving user {friendPlayerId} was not found!");
                
                // Check the sending and receiving players are different users
                if(sendingUser.PlayerId == receivingUser.PlayerId)
                {
                    throw new InvalidOperationException("Invalid action! player cannot send a gift to himself");
                }

                // Transfer resource
                sendingUser.UpdateResource(resourceType, resourceValue * (-1));
                receivingUser.UpdateResource(resourceType, resourceValue);

                // Send event to the client for a new gift (if player connected)
                if (receivingUser.WebSocket != null)
                {
                    _ = CommunicationUtils.Send(receivingUser.WebSocket, $"Hey! Player {sendingUser.PlayerId} just sent you {resourceValue} {resourceType} as a gift!");
                }

                _ = CommunicationUtils.Send(returnWebSocket, $"Gift was sent successfully to player {friendPlayerId}");
            }
            catch (Exception ex)
            {
                // Handle exceptions during message processing
                throw new Exception($"Error processing send gift message: {ex.Message}");
            }
        }
    }
}
