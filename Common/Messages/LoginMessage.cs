using Common.Attributes;
using Common.Models;
using Common.Utils;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("Login")]
    public class LoginMessage : Message
    {
        public string deviceId;

        public LoginMessage() { }

        public LoginMessage(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("Device ID cannot be null or empty", nameof(deviceId));
            }

            this.deviceId = deviceId;
        }

        override public void ProcessAndRespond(WebSocket returnWebSocket)
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
                throw new Exception($"Error processing login message: {ex.Message}");
            }
        }

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
