using Common.Enums;
using System.Net.WebSockets;

namespace Common.Models
{
    /// <summary>
    /// Represents the state of a player in the game.
    /// </summary>
    public class PlayerState
    {
        /// <summary>
        /// Gets or sets the unique identifier for the player.
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the device identifier associated with the player.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the WebSocket connection associated with the player.
        /// </summary>
        public WebSocket WebSocket { get; set; }

        /// <summary>
        /// Gets a dictionary containing the resources and their respective values associated with the player.
        /// </summary>
        public Dictionary<ResourceType, int> Resources { get; } = new Dictionary<ResourceType, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerState"/> class with the specified player details.
        /// </summary>
        /// <param name="playerId">The unique identifier for the player.</param>
        /// <param name="deviceId">The device identifier associated with the player.</param>
        /// <param name="webSocket">The WebSocket connection associated with the player.</param>
        public PlayerState(int playerId, string deviceId, WebSocket webSocket)
        {
            PlayerId = playerId;
            DeviceId = deviceId;
            WebSocket = webSocket;

            // Initialize Resources with enum values and 0 as the initial value
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                Resources[resourceType] = 0;
            }
        }

        /// <summary>
        /// Updates the specified resource type with the given value and returns the new balance.
        /// </summary>
        /// <param name="type">The type of the resource to update.</param>
        /// <param name="value">The value to update the resource by.</param>
        /// <returns>The new balance of the updated resource.</returns>
        public int UpdateResource(ResourceType type, int value)
        {
            Resources[type] += value;

            return Resources[type];
        }
    }
}
