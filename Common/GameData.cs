using Common.Enums;
using Common.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Common
{
    /// <summary>
    /// Represents the central data storage and management for the game server.
    /// </summary>
    public sealed class GameData
    {
        private static int idCounter = 0;
        private static GameData? instance;
        private static readonly object padlock = new();
        private static readonly ConcurrentDictionary<int, PlayerState> _players = new();

        /// <summary>
        /// Private constructor to ensure singleton pattern.
        /// </summary>
        GameData()
        {
            _players.Clear();
        }

        /// <summary>
        /// Gets the singleton instance of the GameData class.
        /// </summary>
        public static GameData Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new GameData();
                    return instance;
                }
            }
        }

        /// <summary>
        /// Retrieves a player by their device ID.
        /// </summary>
        /// <param name="deviceId">The device ID of the player.</param>
        /// <returns>The player state associated with the device ID, or null if not found.</returns>
        public static PlayerState? GetUserByDeviceId(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("Argument is invalid");
            }

            // Check if the player is already connected
            var existingPlayer = _players.Values.FirstOrDefault(player => player.DeviceId == deviceId);

            return existingPlayer;
        }

        /// <summary>
        /// Retrieves a player by their WebSocket.
        /// </summary>
        /// <param name="webSocket">The WebSocket associated with the player.</param>
        /// <returns>The player state associated with the WebSocket, or null if not found.</returns>
        public static PlayerState? GetUserByWebSocket(WebSocket webSocket)
        {
            return _players.Values.FirstOrDefault(player => player.WebSocket == webSocket);
        }

        /// <summary>
        /// Retrieves a player by their player ID.
        /// </summary>
        /// <param name="playerId">The player ID.</param>
        /// <returns>The player state associated with the player ID, or null if not found.</returns>
        public static PlayerState? GetUserByPlayerId(int playerId)
        {
            return _players.Values.FirstOrDefault(player => player.PlayerId == playerId);
        }

        /// <summary>
        /// Adds a new user to the game.
        /// </summary>
        /// <param name="deviceId">The device ID of the new user.</param>
        /// <param name="playerWebSocket">The WebSocket associated with the new user.</param>
        /// <returns>The player ID assigned to the new user.</returns>
        public static int AddUser(string deviceId, WebSocket playerWebSocket)
        {
            if (playerWebSocket == null)
            {
                throw new ArgumentNullException(nameof(playerWebSocket), "WebSocket cannot be null");
            }

            // Check if the WebSocket is already associated with another user
            if (_players.Values.Any(player => player.WebSocket == playerWebSocket))
            {
                throw new InvalidOperationException("WebSocket is already associated with another user");
            }

            // Create a new player state
            int playerID = idCounter++;
            var playerState = new PlayerState(playerID, deviceId, playerWebSocket);

            _players.TryAdd(playerID, playerState);

            return playerID;
        }

        /// <summary>
        /// Logs out a user by closing their WebSocket connection.
        /// </summary>
        /// <param name="playerWebSocket">The WebSocket associated with the user to log out.</param>
        public static void LogoutUser(WebSocket playerWebSocket)
        {
            var playerToLogout = GetUserByWebSocket(playerWebSocket);

            if (playerToLogout != null)
            {
                _players.TryRemove(playerToLogout.PlayerId, out playerToLogout);
            }
        }

        /// <summary>
        /// Updates a user's resource and returns the new resource balance.
        /// </summary>
        /// <param name="playerWebSocket">The WebSocket associated with the user to update.</param>
        /// <param name="resourceType">The type of resource to update.</param>
        /// <param name="resourceValue">The value to update the resource by.</param>
        /// <returns>The new resource balance.</returns>
        public static int? UpdateResource(WebSocket playerWebSocket, ResourceType resourceType, int resourceValue)
        {
            var playerToUpdate = GetUserByWebSocket(playerWebSocket);

            if (playerToUpdate != null)
            {
                int? newBalance = playerToUpdate.UpdateResource(resourceType, resourceValue);
                return newBalance;
            }
            else
            {
                // Handle the case where the player to update is not found
                throw new InvalidOperationException("User not found for WebSocket during resource update");
            }
        }

        /// <summary>
        /// Logs out all users by closing their WebSocket connections.
        /// </summary>
        public static void LogoutAllUsers()
        {
            foreach (var item in _players.Values)
            {
                item.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            _players.Clear();
        }
    }
}
