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
            this.deviceId = deviceId;
        }

        override public void ProcessAndRespond(WebSocket returnWebSocket)
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

        public override void InitializeParams(dynamic message)
        {
            deviceId = message.deviceId;
        }
    }
}
