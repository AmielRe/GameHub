using Common.Enums;
using Common.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Numerics;

namespace Common
{
    public sealed class GameData
    {
        private static GameData? instance;
        private static readonly object padlock = new();
        private static readonly ConcurrentDictionary<int, PlayerState> _players = new();

        GameData()
        {
            _players.Clear();
        }

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

        public static int LoginUser(string deviceId, WebSocket playerWebSocket)
        {
            if(string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("Argument is invalid");
            }

            // Check if the player is already connected
            var existingPlayer = _players.Values.FirstOrDefault(player => player.DeviceId == deviceId);
            if (existingPlayer != null)
            {
                // Respond accordingly if the player is already connected
                return existingPlayer.PlayerId;
            }

            // Create a new player state
            int playerID = Guid.NewGuid().GetHashCode();
            var playerState = new PlayerState(playerID, deviceId, playerWebSocket);

            _players.TryAdd(playerID, playerState);

            return playerID;
        }

        public static void LogoutUser(WebSocket playerWebSocket)
        {
            var playerToLogout = _players.Values.FirstOrDefault(player => player.WebSocket == playerWebSocket);

            if(playerToLogout != null)
            {
                _players.TryRemove(playerToLogout.PlayerId, out playerToLogout);
            }
        }

        public static int? UpdateResource(WebSocket playerWebSocket, ResourceType resourceType, int resourceValue)
        {
            var playerToUpdate = _players.Values.FirstOrDefault(player => player.WebSocket == playerWebSocket);

            int? newBalance = playerToUpdate?.UpdateResource(resourceType, resourceValue);

            return newBalance;
        }
    }
}
