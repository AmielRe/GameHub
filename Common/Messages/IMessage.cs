using System.Net.WebSockets;

namespace Common.Messages
{
    public interface IMessage
    {
        void Handle(WebSocket returnWebSocket);

        void InitializeParams(dynamic message);
    }
}
