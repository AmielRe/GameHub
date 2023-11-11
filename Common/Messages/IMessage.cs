using System.Net.WebSockets;

namespace Common.Messages
{
    public interface IMessage
    {
        void Handle(dynamic message, WebSocket returnWebSocket);

        void InitializeParams(dynamic message);

        public abstract void ProcessAndRespond(WebSocket returnWebSocket);
    }
}
