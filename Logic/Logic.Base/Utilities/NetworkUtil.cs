namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using s2.s2Utils.Logic.Portable.Utilities;

    /// <summary>
    /// Contains helper-methods for working against the network on the TCP-level.
    /// </summary>
    public static class NetworkUtil
    {
        #region methods

        /// <summary>
        /// Checks, if a certain <paramref name="port"/> is opened on the local machine.
        /// </summary>
        /// <remarks>
        /// <para>Original version found at <see>http://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available/></see>.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">Is thrown if the <paramref name="port"/> parameter contains a value less or equal than 0.</exception>
        /// <param name="port">The number of the port to check.</param>
        /// <returns><c>True</c> if the port is opened, otherwise <c>false.</c></returns>
        public static bool IsPortOpened(int port)
        {
            CheckUtil.ThrowIfZeroOrNegativ(() => port);
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            return tcpConnInfoArray.Any(ep => ep.Port == port);
        }

        /// <summary>
        /// Checks, if a certain <paramref name="port"/> is opened on a given <paramref name="host"/>.
        /// </summary>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeout">The timeout in seconds to wait for a reply.</param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns><c>True</c> if the port is opened, otherwise <c>false.</c></returns>
        public static bool IsPortOpened(string host, int port, int timeout = 1, bool useUdp = false)
        {
            var result = false;
            if (!useUdp)
            {
                // Use TCP
                var client = new TcpClient();
                try
                {
                    client.ReceiveTimeout = timeout * 1000;
                    client.SendTimeout = timeout * 1000;
                    var asyncResult = client.BeginConnect(host, port, null, null);
                    var waitHandle = asyncResult.AsyncWaitHandle;
                    try
                    {
                        if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout), false))
                        {
                            // The result was positiv
                            result = client.Connected;
                        }
                        // ensure the ending-call
                        client.EndConnect(asyncResult);
                    }
                    finally
                    {
                        // Ensure to close the wait handle.
                        waitHandle.Close();
                    }
                }
                catch { }
                finally
                {
                    // wait handle didn't came back in time
                    client.Close();
                }
            }
            else
            {
                // Use UDP
                var client = new UdpClient();
                try
                {                    
                    client.Connect(host, port);
                    result = true;                    
                }
                catch { }
                finally
                {
                    // wait handle didn't came back in time
                    client.Close();
                }
            }            
            return result;
        }

        /// <summary>
        /// Checks, if a certain TCP <paramref name="port"/> is opened on a given <paramref name="host"/>.
        /// </summary>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeout">The timeout in seconds to wait for a reply.</param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns><c>true</c> if the port is opened, otherwise <c>false.</c></returns>
        public static async Task<bool> IsPortOpenedAsync(string host, int port, int timeout, bool useUdp = false)
        {
            var result = false;
            if (!useUdp)
            {
                // Use TCP          
                using (var client = new TcpClient())
                {
                    try
                    {
                        client.ReceiveTimeout = timeout * 1000;
                        client.SendTimeout = timeout * 1000;
                        await client.ConnectAsync(host, port);
                        result = client.Connected;
                    }
                    catch
                    {                        
                    }                    
                }
            }
            else
            {
                var client = new UdpClient();
                try
                {                    
                    client.Connect(host, port);
                    result = true;
                }
                catch { }
                finally
                {
                    // wait handle didn't came back in time                    
                    client.Close();
                }
            }
            return result;
        }

        #endregion
    }
}