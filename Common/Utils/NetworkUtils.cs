using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Common.Utils
{
    /// <summary>
    /// Utility class for network-related operations.
    /// </summary>
    public static class NetworkUtils
    {
        /// <summary>
        /// Retrieves the IPv4 address of the machine from the available network interfaces.
        /// </summary>
        /// <returns>The IPv4 address of the machine.</returns>
        /// <exception cref="Exception">Thrown when no valid IPv4 address is found or an error occurs during retrieval.</exception>
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
                // Handle exceptions that might occur during IPv4 address retrieval
                throw new Exception($"Error retrieving IPv4 address: {ex.Message}");
            }
        }
    }
}
