using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public sealed class GameData
    {
        private static GameData instance = null;
        private static readonly object padlock = new object();
        private static Dictionary<string, int> _connectedClients = new Dictionary<string, int>();

        GameData()
        {
            _connectedClients.Clear();
        }

        public static GameData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameData();
                    }
                    return instance;
                }
            }
        }

        public static int loginUser(string UDID)
        {
            if(string.IsNullOrEmpty(UDID))
            {
                throw new ArgumentException("Argument is invalid");
            }

            if(_connectedClients.TryGetValue(UDID, out int playerID))
            {
                return playerID;
            }

            playerID = Guid.NewGuid().GetHashCode();
            _connectedClients.Add(UDID, playerID);

            return playerID;
        }
    }
}
