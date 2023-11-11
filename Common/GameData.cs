﻿using Common.Enums;
using Common.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Common
{
    public sealed class GameData
    {
        private static int idCounter = 0;
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

        public static PlayerState? GetUserByDeviceId(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("Argument is invalid");
            }

            // Check if the player is already connected
            var existingPlayer = _players.Values.FirstOrDefault(player => player.DeviceId == deviceId);

            return existingPlayer;
        }

        public static PlayerState? GetUserByWebSocket(WebSocket webSocket)
        {
            return _players.Values.FirstOrDefault(player => player.WebSocket == webSocket);
        }

        public static PlayerState? GetUserByPlayerId(int playerId)
        {
            return _players.Values.FirstOrDefault(player => player.PlayerId == playerId);
        }

        public static int AddUser(string deviceId, WebSocket playerWebSocket)
        {
            // Create a new player state
            int playerID = idCounter++;
            var playerState = new PlayerState(playerID, deviceId, playerWebSocket);

            _players.TryAdd(playerID, playerState);

            return playerID;
        }

        public static void LogoutUser(WebSocket playerWebSocket)
        {
            var playerToLogout = GetUserByWebSocket(playerWebSocket);

            if (playerToLogout != null)
            {
                _players.TryRemove(playerToLogout.PlayerId, out playerToLogout);
            }
        }

        public static int? UpdateResource(WebSocket playerWebSocket, ResourceType resourceType, int resourceValue)
        {
            var playerToUpdate = GetUserByWebSocket(playerWebSocket);

            int? newBalance = playerToUpdate?.UpdateResource(resourceType, resourceValue);

            return newBalance;
        }
    }
}
