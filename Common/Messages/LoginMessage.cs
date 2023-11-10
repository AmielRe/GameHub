using Common.Attributes;
using Common.Utils;
using System.Net.WebSockets;

namespace Common.Messages
{
    [Message("Login")]
    public class LoginMessage : Message
    {
        public string UDID;

        public LoginMessage() { }

        public LoginMessage(string UDID)
        {
            this.UDID = UDID;
        }

        override public void Handle(WebSocket returnWebSocket)
        {
            int playerID = GameData.loginUser(this.UDID);

            _ = CommunicationUtils.Send(returnWebSocket, playerID.ToString());
        }

        public override void InitializeParams(dynamic message)
        {
            this.UDID = message.UDID;
        }
    }
}
