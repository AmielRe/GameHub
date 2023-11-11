using Common.Attributes;
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
            int playerID = GameData.LoginUser(deviceId, returnWebSocket);

            _ = CommunicationUtils.Send(returnWebSocket, playerID.ToString());
        }

        public override void InitializeParams(dynamic message)
        {
            deviceId = message.deviceId;
        }
    }
}
