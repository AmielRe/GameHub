using Common.Attributes;
using Common.Enums;
using Common.Models;
using Common.Utils;
using System;
using System.Net.WebSockets;

namespace Common.Messages
{
    /// <summary>
    /// Represents a message for updating resources of a player.
    /// </summary>
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : Message
    {
        /// <summary>
        /// Gets or sets the type of resource to be updated.
        /// </summary>
        public ResourceType resourceType;

        /// <summary>
        /// Gets or sets the value to update the resource.
        /// </summary>
        public int resourceValue;

        /// <summary>
        /// Default constructor for <see cref="UpdateResourcesMessage"/>.
        /// </summary>
        public UpdateResourcesMessage() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateResourcesMessage"/> class with the specified parameters.
        /// </summary>
        /// <param name="resourceType">The type of resource to be updated.</param>
        /// <param name="resourceValue">The value to update the resource.</param>
        /// <exception cref="ArgumentException">Thrown if the resource type is invalid.</exception>
        public UpdateResourcesMessage(ResourceType resourceType, int resourceValue)
        {
            if (!Enum.IsDefined(typeof(ResourceType), resourceType))
            {
                throw new ArgumentException("Invalid resource type", nameof(resourceType));
            }

            this.resourceType = resourceType;
            this.resourceValue = resourceValue;
        }

        /// <inheritdoc/>
        public override void InitializeParams(dynamic message)
        {
            if (!Enum.IsDefined(typeof(ResourceType), message?.resourceType))
            {
                throw new ArgumentException("Invalid parameters for initializing UpdateResourcesMessage", nameof(message));
            }

            resourceType = message.resourceType;
            resourceValue = message.resourceValue;
        }

        /// <inheritdoc/>
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
                // Handle exceptions during message processing
                throw new Exception($"Error processing update resources message: {ex.Message}");
            }
        }
    }
}
