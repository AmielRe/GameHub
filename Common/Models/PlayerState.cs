using Common.Enums;
using System.Net.WebSockets;

namespace Common.Models
{
    public class PlayerState
    {
        public int PlayerId { get; set; }
        public string DeviceId { get; set; }
        public WebSocket WebSocket { get; set; }
        public Dictionary<ResourceType, int> Resources { get; } = new Dictionary<ResourceType, int>();

        public PlayerState(int playerId, string deviceId, WebSocket webSocket)
        {
            PlayerId = playerId;
            DeviceId = deviceId;
            WebSocket = webSocket;
        }

        public int UpdateResource(ResourceType type, int value)
        {
            if (Resources.ContainsKey(type))
            {
                Resources[type] += value;
            }
            else
            {
                Resources[type] = value;
            }

            return Resources[type];
        }
    }
}
