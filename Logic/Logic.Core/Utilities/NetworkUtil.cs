namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using Enumerations;

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
        /// <param name="timeoutSeconds">
        /// The timeoutSeconds in seconds to wait for a reply. Defaults to 2 because 1 second is
        /// mostly too short for .NET.
        /// </param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns>The result of the operation.</returns>
        public static PortState GetPortState(string host, int port, int timeoutSeconds = 2, bool useUdp = false)
        {
            var outerResult = PortState.Unknown;
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var token = tokenSource.Token;
            try
            {
                // use a task to enable outer cancellation regardless of the asyncResult-timeoutSeconds which isn't working very well
                outerResult = Task.Run(
                    () =>
                    {
                        var result = PortState.Unknown;
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
                                    if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutSeconds), false))
                                    {
                                        // The result was positive
                                        if (!asyncResult.IsCompleted)
                                        {
                                            result = PortState.TimedOut;
                                        }
                                        else
                                        {
                                            result = client.Connected ? PortState.Open : PortState.Closed;
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
                                TraceUtil.WriteTraceError(sockEx.Message);
                                // see https://msdn.microsoft.com/en-us/library/ms740668.aspx for a list of all states
                                switch (sockEx.NativeErrorCode)
                                {
                                    case 10060:
                                        result = PortState.TimedOut;
                                        break;
                                    case 10061:
                                        result = PortState.Refused;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                TraceUtil.WriteTraceError(ex.Message);
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
                                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutSeconds), false);
                                if (!asyncResult.IsCompleted)
                                {
                                    return PortState.TimedOut;
                                }
                                result = PortState.Open;
                            }
                            catch (SocketException sockEx)
                            {
                                TraceUtil.WriteTraceError(sockEx.Message);
                                // see https://msdn.microsoft.com/en-us/library/ms740668.aspx for a list of all states
                                switch (sockEx.NativeErrorCode)
                                {
                                    case 10060:
                                        result = PortState.TimedOut;
                                        break;
                                    case 10061:
                                        result = PortState.Refused;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                TraceUtil.WriteTraceError(ex.Message);
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
                            return PortState.TimedOut;
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
                    outerResult = PortState.TimedOut;
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
        /// Tries to open a network connection to a specific port retrieving the result.
        /// </summary>
        /// <remarks>
        /// Stores the result of the last operation in <see cref="LastCheckResult" /> too.
        /// </remarks>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeoutSeconds">
        /// The timeoutSeconds in seconds to wait for a reply. Defaults to 2 because 1 second is
        /// mostly too short for .NET.
        /// </param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns>The result of the operation.</returns>
        public static async Task<PortState> GetPortStateAsync(string host, int port, int timeoutSeconds = 2, bool useUdp = false)
        {
            var result = useUdp ? await GetUdpPortStateAsync(host, port, timeoutSeconds) : await GetTcpPortStateAsync(host, port, timeoutSeconds);
            LastCheckResult = result;
            return result;
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
            CheckUtil.ThrowIfZeroOrNegative(() => port);
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            return tcpConnInfoArray.Any(ep => ep.Port == port);
        }

        /// <summary>
        /// Checks, if a certain <paramref name="port" /> is opened on a given <paramref name="host" />.
        /// </summary>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeout">The timeoutSeconds in seconds to wait for a reply.</param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns><c>True</c> if the port is opened, otherwise <c>false.</c></returns>
        public static bool IsPortOpened(string host, int port, int timeout = 1, bool useUdp = false)
        {
            return GetPortState(host, port, timeout, useUdp) == PortState.Open;
        }

        /// <summary>
        /// Checks, if a certain TCP <paramref name="port" /> is opened on a given <paramref name="host" />.
        /// </summary>
        /// <remarks>
        /// Due to the lack of cancellation support in the <see cref="TcpClient" /> and <see cref="UdpClient" />
        /// you should use the synchronously method insteadif timeoutSeconds is valueable for you.
        /// </remarks>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeout">The timeoutSeconds in seconds to wait for a reply.</param>
        /// <param name="useUdp"><c>true</c> if a UDP port should be checked.</param>
        /// <returns><c>true</c> if the port is opened, otherwise <c>false.</c></returns>
        public static async Task<bool> IsPortOpenedAsync(string host, int port, int timeout, bool useUdp = false)
        {
            var result = false;
            if (!useUdp)
            {
                // use TCP          
                using (var client = new TcpClient())
                {
                    try
                    {
                        client.ReceiveTimeout = timeout * 1000;
                        client.SendTimeout = timeout * 1000;
                        await client.ConnectAsync(host, port).ConfigureAwait(false);
                        result = client.Connected;
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.WriteTraceError(ex.Message);
                    }
                }
            }
            else
            {
                // use UDP
                var client = new UdpClient();
                try
                {
                    client.Connect(host, port);
                    await client.ReceiveAsync().ConfigureAwait(false);
                    result = true;
                }
                catch (Exception ex)
                {
                    TraceUtil.WriteTraceError(ex.Message);
                }
                finally
                {
                    // wait handle didn't came back in time                    
                    client.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Tries to open a network connection via TCP to a specific port retrieving the result.
        /// </summary>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeoutSeconds">
        /// The timeoutSeconds in seconds to wait for a reply. Defaults to 2 because 1 second is
        /// mostly too short for .NET.
        /// </param>
        /// <returns>The result of the operation.</returns>
        private static async Task<PortState> GetTcpPortStateAsync(string host, int port, int timeoutSeconds = 2)
        {
            var cancellationCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                using (var cts = new CancellationTokenSource(timeoutSeconds))
                {
                    using (var client = new TcpClient())
                    {
                        var task = client.ConnectAsync(host, port);
                        using (cts.Token.Register(() => cancellationCompletionSource.TrySetResult(true)))
                        {
                            if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
                            {
                                throw new OperationCanceledException(cts.Token);
                            }
                            // throw exception inside 'task' (if any)
                            if (task.Exception?.InnerException != null)
                            {
                                throw task.Exception.InnerException;
                            }
                        }
                        return client.Connected ? PortState.Open : PortState.Closed;
                    }
                }
            }
            catch (OperationCanceledException operationCanceledEx)
            {
                // connection timeout
                return PortState.Closed;
            }
            catch (SocketException socketEx)
            {
                Console.WriteLine($"Socket-Fehler: {socketEx}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex}");
            }
            return PortState.Unknown;
        }

        /// <summary>
        /// Tries to open a network connection via UDP to a specific port retrieving the result.
        /// </summary>
        /// <param name="host">IP-Address or host name to check for the port.</param>
        /// <param name="port">The port-number to check.</param>
        /// <param name="timeoutSeconds">
        /// The timeoutSeconds in seconds to wait for a reply. Defaults to 2 because 1 second is
        /// mostly too short for .NET.
        /// </param>
        /// <returns>The result of the operation.</returns>
        private static async Task<PortState> GetUdpPortStateAsync(string host, int port, int timeoutSeconds = 2)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            return await Task.Run(
                () =>
                {
                    using (var client = new UdpClient())
                    {
                        try
                        {
                            client.Connect(host, port);
                            var asyncResult = client.BeginReceive(
                                r =>
                                {
                                },
                                null);
                            asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutSeconds), false);
                            if (!asyncResult.IsCompleted)
                            {
                                return PortState.TimedOut;
                            }
                            return PortState.Open;
                        }
                        catch (SocketException sockEx)
                        {
                            TraceUtil.WriteTraceError(sockEx.Message);
                            // see https://msdn.microsoft.com/en-us/library/ms740668.aspx for a list of all states
                            switch (sockEx.NativeErrorCode)
                            {
                                case 10060:
                                    return PortState.TimedOut;
                                case 10061:
                                    return PortState.Refused;
                            }
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.WriteTraceError(ex.Message);
                        }
                    }
                    return PortState.Unknown;
                },
                tokenSource.Token);
        }

        #endregion

        #region properties

        /// <summary>
        /// The result of the last internal port check.
        /// </summary>
        public static PortState LastCheckResult { get; private set; } = PortState.Unknown;

        #endregion
    }
}