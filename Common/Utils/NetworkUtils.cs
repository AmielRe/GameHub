using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class NetworkUtils
    {
        public static string GetIPv4Address()
        {
            try
            { 
                string ipv4Address = null;
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                        (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        foreach (UnicastIPAddressInformation ipInfo in ipProperties.UnicastAddresses)
                        {
                            if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipv4Address = ipInfo.Address.ToString();
                                break;
                            }
                        }

                        if (ipv4Address != null)
                        {
                            break;
                        }
                    }
                }

                if (ipv4Address == null)
                {
                    // Handle the case where no IPv4 address is found
                    throw new Exception("No valid IPv4 address found.");
                }

                return ipv4Address;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving IPv4 address: {ex.Message}");
            }
        }

    }
}
