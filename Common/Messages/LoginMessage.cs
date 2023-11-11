using Common.Attributes;
using Common.Models;
using Common.Utils;
using System;
using System.Net.WebSockets;

namespace Common.Messages
{
    /// <summary>
    /// Represents a message for user login.
    /// </summary>
    [Message("Login")]
    public class LoginMessage : Message
    {
        /// <summary>
        /// Gets or sets the device ID associated with the login.
        /// </summary>
        public string deviceId;

        /// <summary>
        /// Default constructor for <see cref="LoginMessage"/>.
        /// </summary>
        public LoginMessage() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginMessage"/> class with the specified device ID.
        /// </summary>
        /// <param name="deviceId">The device ID associated with the login.</param>
        /// <exception cref="ArgumentException">Thrown if the device ID is null or empty.</exception>
        public LoginMessage(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("Device ID cannot be null or empty", nameof(deviceId));
            }

            this.deviceId = deviceId;
        }

        /// <inheritdoc/>
        public override void ProcessAndRespond(WebSocket returnWebSocket)
        {
            try
            {
                PlayerState? playerState = GameData.GetUserByDeviceId(deviceId);
                int playerId;

                if (playerState != null)
                {
                    playerId = playerState.PlayerId;
                }
                else
                {
                    playerId = GameData.AddUser(deviceId, returnWebSocket);
                }

                _ = CommunicationUtils.Send(returnWebSocket, playerId.ToString());
            }
            catch (Exception ex)
            {
                // Handle exceptions during message processing
                throw new Exception($"Error processing login message: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public override void InitializeParams(dynamic message)
        {
            if (string.IsNullOrEmpty(Convert.ToString(message?.deviceId)))
            {
                throw new ArgumentException("Device ID is missing or invalid", nameof(message));
            }

            deviceId = message.deviceId;
        }
    }
}
