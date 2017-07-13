using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Base.Enumerations;
    using System.Net.Sockets;

    /// <summary>
    /// Contains helper-methods for working against the network on the TCP-level.
    /// </summary>
    public static class NetworkUtil
    {
        #region methods

        /// <summary>
        /// Tries to open a network connection to a specific port retrieving the result.
        /// </summary>
        /// <remarks>
        /// Stores the result of the last operation in <see cref="LastCheckResult" /> too.
        /// </remarks>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeout">The timeout in seconds to wait for a reply. Defaults to 2 because 1 second is mostly too short for .NET.</param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns>The result of the operation.</returns>
        public static PortStateEnum GetPortState(string host, int port, int timeout = 2, bool useUdp = false)
        {
            var outerResult = PortStateEnum.Unkown;
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            var token = tokenSource.Token;
            try
            {
                // use a task to enable outer cancellation regardless of the asyncResult-timeout which isn't working very well
                outerResult = Task.Run(
                    () =>
                    {
                        var result = PortStateEnum.Unkown;
                        if (!useUdp)
                        {
                            // Use TCP
                            var client = new TcpClient();
                            try
                            {
                                var asyncResult = client.BeginConnect(host, port, null, null);
                                var waitHandle = asyncResult.AsyncWaitHandle;
                                try
                                {
                                    if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout), false))
                                    {
                                        // The result was positiv
                                        if (!asyncResult.IsCompleted)
                                        {
                                            result = PortStateEnum.TimedOut;
                                        }
                                        else
                                        {
                                            result = client.Connected ? PortStateEnum.Open : PortStateEnum.Closed;
                                        }
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
                            catch (SocketException sockEx)
                            {
                                // see https://msdn.microsoft.com/en-us/library/ms740668.aspx for a list of all states
                                switch (sockEx.NativeErrorCode)
                                {
                                    case 10060:
                                        result = PortStateEnum.TimedOut;
                                        break;
                                    case 10061:
                                        result = PortStateEnum.Refused;
                                        break;
                                }
                            }
                            catch
                            {
                                // empty catch
                            }
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
                                var asyncResult = client.BeginReceive(
                                    r =>
                                    {
                                    },
                                    null);
                                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout), false);
                                if (!asyncResult.IsCompleted)
                                {
                                    return PortStateEnum.TimedOut;
                                }
                                result = PortStateEnum.Open;
                            }
                            catch (SocketException sockEx)
                            {
                                // see https://msdn.microsoft.com/en-us/library/ms740668.aspx for a list of all states
                                switch (sockEx.NativeErrorCode)
                                {
                                    case 10060:
                                        result = PortStateEnum.TimedOut;
                                        break;
                                    case 10061:
                                        result = PortStateEnum.Refused;
                                        break;
                                }
                            }
                            catch
                            {
                                // empty catch
                            }
                            finally
                            {
                                // wait handle didn't came back in time
                                client.Close();
                            }
                        }                        
                        return result;
                    },
                    token).ContinueWith(
                    t =>
                    {
                        if (t.IsCanceled || token.IsCancellationRequested)
                        {
                            return PortStateEnum.TimedOut;
                        }
                        return t.Result;
                    },
                    token).Result;
            }
            catch (AggregateException aex)
            {
                var flatten = aex.Flatten();
                if (flatten.InnerException is TaskCanceledException)
                {
                    outerResult = PortStateEnum.TimedOut;
                }
            }
            catch
            {    
                // empty catch
            }
            LastCheckResult = outerResult;
            return outerResult;
        }

        /// <summary>
        /// Checks, if a certain <paramref name="port" /> is opened on the local machine.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Original version found at
        /// <see>http://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available/></see>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="port" /> parameter contains a value less or equal
        /// than 0.
        /// </exception>
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
        /// Checks, if a certain <paramref name="port" /> is opened on a given <paramref name="host" />.
        /// </summary>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeout">The timeout in seconds to wait for a reply.</param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns><c>True</c> if the port is opened, otherwise <c>false.</c></returns>
        public static bool IsPortOpened(string host, int port, int timeout = 1, bool useUdp = false)
        {
            return GetPortState(host, port, timeout, useUdp) == PortStateEnum.Open;
        }

        /// <summary>
        /// Checks, if a certain TCP <paramref name="port" /> is opened on a given <paramref name="host" />.
        /// </summary>
        /// <remarks>
        /// Due to the lack of cancellation support in the <see cref="TcpClient"/> and <see cref="UdpClient"/>
        /// you should use the synchronously method insteadif timeout is valueable for you.
        /// </remarks>
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
                        await client.ConnectAsync(host, port).ConfigureAwait(false);
                        result = client.Connected;
                    }
                    catch
                    {
                        // empty catch
                    }
                }
            }
            else
            {
                var client = new UdpClient();
                try
                {
                    client.Connect(host, port);
                    await client.ReceiveAsync();
                    result = true;
                }
                catch
                {
                    // empty catch
                }
                finally
                {
                    // wait handle didn't came back in time                    
                    client.Close();
                }
            }
            return result;
        }

        #endregion

        #region properties

        /// <summary>
        /// The result of the last internal port check.
        /// </summary>
        public static PortStateEnum LastCheckResult { get; private set; } = PortStateEnum.Unkown;

        #endregion
    }
}