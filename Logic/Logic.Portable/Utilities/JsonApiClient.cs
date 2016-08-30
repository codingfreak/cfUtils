namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    /// <summary>
    /// Handles consumption of the api.
    /// </summary>
    public sealed class JsonApiClient : HttpClient
    {
        #region constructors and destructors

        private JsonApiClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
        }

        #endregion

        #region methods

        /// <summary>
        /// Make a http delete request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> DeleteSimpleAsync(Uri endpoint)
        {
            return await SendAsync(endpoint, HttpMethod.Delete);
        }

        /// <summary>
        /// Make a http delete request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> DeleteSimpleAsync(string relativePath, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, HttpMethod.Delete, queryParams);
        }

        /// <summary>
        /// Retrieves a new instance of an API client.
        /// </summary>
        /// <param name="baseApiEndpoint">An optional URI for the <see cref="BaseApiEndpoint" /> property.</param>
        /// <returns>The usable instance.</returns>
        public static JsonApiClient GetInstance(Uri baseApiEndpoint = null)
        {
            if (baseApiEndpoint != null)
            {
                BaseApiEndpoint = baseApiEndpoint;
            }
            return GetClient();
        }

        /// <summary>
        /// Retrieves a new instance of an API client using specific headers.
        /// </summary>
        /// <param name="headers">The headers to use.</param>
        /// <param name="baseApiEndpoint">An optional URI for the <see cref="BaseApiEndpoint" /> property.</param>
        /// <returns>The usable instance.</returns>
        public static JsonApiClient GetInstance(Dictionary<string, string> headers, Uri baseApiEndpoint = null)
        {
            if (baseApiEndpoint != null)
            {
                BaseApiEndpoint = baseApiEndpoint;
            }
            return GetClient(headers);
        }

        /// <summary>
        /// Converts a <paramref name="model" /> to a <see cref="StringContent" /> which can be passed to a POST request in JSON.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="model" />.</typeparam>
        /// <param name="model">The model to convert to a JSON-body.</param>
        /// <returns>The JSON content for a request.</returns>
        public static StringContent GetJsonRequestContent<T>(T model)
        {
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Make a http get request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> GetSimpleAsync(Uri endpoint, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            return await SendAsync(endpoint, HttpMethod.Get, queryParams);
        }

        /// <summary>
        /// Make a http get request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> GetSimpleAsync(string relativePath, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, HttpMethod.Get, queryParams);
        }

        /// <summary>
        /// Make a http get request on the given endpoint with the given query parameters and return a result model.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns>An instance of <typeparamref name="TResult" />.</returns>
        public async Task<TResult> GetWithResultAsync<TResult>(Uri endpoint, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            return await SendWithResultAsync<TResult>(endpoint, HttpMethod.Get, queryParams);
        }

        /// <summary>
        /// Make a http get request on the given endpoint with the given query parameters and return a result model.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns>An instance of <typeparamref name="TResult" />.</returns>
        public async Task<TResult> GetWithResultAsync<TResult>(string relativePath, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendWithResultAsync<TResult>(uri, HttpMethod.Get, queryParams);
        }

        /// <summary>
        /// Make a http patch request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PatchSimpleAsync(Uri endpoint, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            return await SendAsync(endpoint, new HttpMethod("PATCH"), queryParams);
        }

        /// <summary>
        /// Make a http patch request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PatchSimpleAsync(string relativePath, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, new HttpMethod("PATCH"), queryParams);
        }

        /// <summary>
        /// Make a http patch request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PatchSimpleAsync<TInput>(Uri endpoint, TInput inputData)
        {
            return await SendAsync(endpoint, new HttpMethod("PATCH"), inputData);
        }

        /// <summary>
        /// Make a http patch request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PatchSimpleAsync<TInput>(string relativePath, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, new HttpMethod("PATCH"), inputData);
        }

        /// <summary>
        /// Make a http post request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PostSimpleAsync(Uri endpoint, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            return await SendAsync(endpoint, HttpMethod.Post, queryParams);
        }

        /// <summary>
        /// Make a http post request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PostSimpleAsync(string relativePath, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, HttpMethod.Post, queryParams);
        }

        /// <summary>
        /// Makes a simple http post to a given endpoint with the provided data and returns a success status.
        /// </summary>
        /// <typeparam name="TInput">The type of the input data model.</typeparam>
        /// <param name="endpoint">The api endpoint to call.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <returns><c>True</c> if the operation was a success, <c>False</c> otherwise.</returns>
        public async Task<bool> PostSimpleAsync<TInput>(Uri endpoint, TInput inputData)
        {
            return await SendAsync(endpoint, HttpMethod.Post, inputData);
        }

        /// <summary>
        /// Makes a simple http post to a given endpoint with the provided data and returns a success status.
        /// </summary>
        /// <typeparam name="TInput">The type of the input data model.</typeparam>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the operation was a success, <c>False</c> otherwise.</returns>
        public async Task<bool> PostSimpleAsync<TInput>(string relativePath, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, HttpMethod.Post, inputData);
        }

        /// <summary>
        /// Makes a simple http post request to a given api endpoint with the provided data und returns a result value.
        /// </summary>
        /// <typeparam name="TInput">The type of the input data model.</typeparam>
        /// <typeparam name="TResult">The type of the result data model.</typeparam>
        /// <param name="endpoint">The api endpoint to call.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <returns>An instance of type <typeparamref name="TResult" />.</returns>
        public async Task<TResult> PostWithResultAsync<TInput, TResult>(Uri endpoint, TInput inputData)
        {
            return await SendWithResultAsync<TInput, TResult>(endpoint, HttpMethod.Post, inputData);
        }

        /// <summary>
        /// Makes a simple http post request to a given api endpoint with the provided data und returns a result value.
        /// </summary>
        /// <typeparam name="TInput">The type of the input data model.</typeparam>
        /// <typeparam name="TResult">The type of the result data model.</typeparam>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns>An instance of type <typeparamref name="TResult" />.</returns>
        public async Task<TResult> PostWithResultAsync<TInput, TResult>(string relativePath, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendWithResultAsync<TInput, TResult>(uri, HttpMethod.Post, inputData);
        }

        /// <summary>
        /// Make a http put request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="endpoint">The endpoint to call.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PutSimpleAsync<TInput>(Uri endpoint, TInput inputData)
        {
            return await SendAsync(endpoint, HttpMethod.Put, inputData);
        }

        /// <summary>
        /// Make a http put request on the given endpoint with the given query parameters and return a bool indicating success.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns><c>True</c> if the server said the request was successful, <c>False</c> otherwise.</returns>
        public async Task<bool> PutSimpleAsync<TInput>(string relativePath, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendAsync(uri, HttpMethod.Put, inputData);
        }

        /// <summary>
        /// Makes a simple http put request to a given api endpoint with the provided data und returns a result value.
        /// </summary>
        /// <typeparam name="TInput">The type of the input data model.</typeparam>
        /// <typeparam name="TResult">The type of the result data model.</typeparam>
        /// <param name="endpoint">The api endpoint to call.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <returns>An instance of type <typeparamref name="TResult" />.</returns>
        public async Task<TResult> PutWithResultAsync<TInput, TResult>(Uri endpoint, TInput inputData)
        {
            return await SendWithResultAsync<TInput, TResult>(endpoint, HttpMethod.Put, inputData);
        }

        /// <summary>
        /// Makes a simple http put request to a given api endpoint with the provided data und returns a result value.
        /// </summary>
        /// <typeparam name="TInput">The type of the input data model.</typeparam>
        /// <typeparam name="TResult">The type of the result data model.</typeparam>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="inputData">The data to send to the endpoint.</param>
        /// <param name="queryParams">The parameters to include in the get request.</param>
        /// <returns>An instance of type <typeparamref name="TResult" />.</returns>
        public async Task<TResult> PutWithResultAsync<TInput, TResult>(string relativePath, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            if (uri == null)
            {
                throw new ArgumentException("Relative path is invalid.", nameof(relativePath));
            }
            return await SendWithResultAsync<TInput, TResult>(uri, HttpMethod.Put, inputData);
        }

        /// <summary>
        /// Sends a HTTP request of the given <paramref name="method" /> and checks if the service response with a success code.
        /// </summary>
        /// <param name="endpoint">The endpoint fo the target service.</param>
        /// <param name="method">The http-method to use.</param>
        /// <param name="queryParams">Optional url parameters.</param>
        /// <returns><c>true</c> if the request was successful otherwise <c>false</c>.</returns>
        public async Task<bool> SendAsync(Uri endpoint, HttpMethod method, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var message = new HttpRequestMessage(method, endpoint);
            var responseMessage = await SendAsync(message).ConfigureAwait(false);
            HandleResponse(responseMessage);
            return responseMessage.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a HTTP request of the given <paramref name="method" /> containing <paramref name="inputData" /> and checks if the
        /// service response with a success code.
        /// </summary>
        /// <typeparam name="TInput">The type of the the input data.</typeparam>
        /// <param name="endpoint">The endpoint fo the target service.</param>
        /// <param name="method">The http-method to use.</param>
        /// <param name="inputData">The data to send to the service.</param>
        /// <param name="queryParams">Optional url parameters.</param>
        /// <returns><c>true</c> if the request was successful otherwise <c>false</c>.</returns>
        public async Task<bool> SendAsync<TInput>(Uri endpoint, HttpMethod method, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var requestContent = GetJsonRequestContent(inputData);
            var message = new HttpRequestMessage(method, endpoint)
            {
                Content = requestContent
            };
            var responseMessage = await SendAsync(message).ConfigureAwait(false);
            HandleResponse(responseMessage);
            return responseMessage.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a HTTP request of the given <paramref name="method" /> and retrieves a deserialized result of type
        /// <typeparamref name="TResult" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="endpoint">The endpoint fo the target service.</param>
        /// <param name="method">The http-method to use.</param>
        /// <param name="queryParams">Optional url parameters.</param>
        /// <returns>The deserialzed result.</returns>
        public async Task<TResult> SendWithResultAsync<TResult>(Uri endpoint, HttpMethod method, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var message = new HttpRequestMessage(method, endpoint);
            var responseMessage = await SendAsync(message).ConfigureAwait(false);
            HandleResponse(responseMessage);
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"The server returned status code {responseMessage.StatusCode}");
            }
            try
            {
                var responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseModel = JsonConvert.DeserializeObject<TResult>(responseContent);
                return responseModel;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Unexpected data from server could not be deserialized.", ex);
            }
        }

        /// <summary>
        /// Sends a HTTP request of the given <paramref name="method" /> containing <paramref name="inputData" /> and retrieves a
        /// deserialized result of type <typeparamref name="TResult" />.
        /// </summary>
        /// <typeparam name="TInput">The type of the the input data.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="endpoint">The endpoint fo the target service.</param>
        /// <param name="method">The http-method to use.</param>
        /// <param name="inputData">The data to send to the service.</param>
        /// <param name="queryParams">Optional url parameters.</param>
        /// <returns>The deserialzed result.</returns>
        public async Task<TResult> SendWithResultAsync<TInput, TResult>(Uri endpoint, HttpMethod method, TInput inputData, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var requestContent = GetJsonRequestContent(inputData);
            var message = new HttpRequestMessage(method, endpoint)
            {
                Content = requestContent
            };
            var responseMessage = await SendAsync(message).ConfigureAwait(false);
            HandleResponse(responseMessage);
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"The server returned status code {responseMessage.StatusCode}");
            }
            try
            {
                var responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseModel = JsonConvert.DeserializeObject<TResult>(responseContent);
                return responseModel;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Unexpected data from server could not be deserialized.", ex);
            }
        }

        /// <summary>
        /// Creates a http get query string that can be attached to an api endpoint.
        /// </summary>
        /// <param name="parameters">The parameters to convert to string.</param>
        /// <param name="doUrlEncode">Set to <c>True</c> to url-encode the parameter values (but not the keys).</param>
        /// <returns>A query string in the form "key1=value1&key2=value2"</returns>
        public static string ToQuerystring(IEnumerable<KeyValuePair<string, string>> parameters, bool doUrlEncode = true)
        {
            var paramStrings = parameters.Select(pair => pair.Key + "=" + (doUrlEncode ? Uri.EscapeUriString(pair.Value) : pair.Value));
            return string.Join("&", paramStrings);
        }

        /// <summary>
        /// Uploads a blob using <see cref="MultipartFormDataContent" /> to a REST service using a POST and retrieves the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="endpoint">The endpoint fo the target service.</param>
        /// <param name="originalFileName">The original filename including the ending.</param>
        /// <param name="fileData">The serialized file data.</param>
        /// <param name="additionalData">Optional additional values to pass to the form-data.</param>
        /// <returns>The deserialzed result.</returns>
        public async Task<TResult> UploadBytesAsync<TResult>(Uri endpoint, string originalFileName, byte[] fileData, Dictionary<string, string> additionalData = null)
        {
            using (var content = new MultipartFormDataContent("Upload----" + Guid.NewGuid().ToString("N").Substring(8)))
            {
                content.Headers.ContentEncoding.Clear();
                content.Headers.ContentEncoding.Add("utf8");
                if (additionalData != null)
                {
                    foreach (var k in additionalData.Keys)
                    {
                        content.Add(name: k, content: new StringContent(additionalData[k]));
                    }
                }
                var parts = originalFileName.Split('.');
                content.Add(new StreamContent(new MemoryStream(fileData)), parts[0], originalFileName);
                return await PostWithResultAsync<MultipartFormDataContent, TResult>(endpoint, content);
            }
        }

        /// <summary>
        /// Uploads a blob using <see cref="MultipartFormDataContent" /> to a REST service using a POST and retrieves the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="originalFileName">The original filename including the ending.</param>
        /// <param name="fileData">The serialized file data.</param>
        /// <param name="additionalData">Optional additional values to pass to the form-data.</param>
        /// <param name="queryParams">Optional url parameters.</param>
        /// <returns>The deserialzed result.</returns>
        public async Task<TResult> UploadBytesAsync<TResult>(
            string relativePath,
            string originalFileName,
            byte[] fileData,
            Dictionary<string, string> additionalData = null,
            IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            var uri = GetUri(relativePath, queryParams);
            return await UploadBytesAsync<TResult>(uri, originalFileName, fileData, additionalData);
        }

        /// <summary>
        /// Is called to add the 'debug'-header into the request of the given <paramref name="client" />.
        /// </summary>
        /// <param name="client">The mobile client which should be prepared.</param>
        [Conditional("DEBUG")]
        private static void AddDebugHeader(JsonApiClient client)
        {
            foreach (var k in DebugHeaders.Keys)
            {
                client.DefaultRequestHeaders.Add(k, DefaultHeaders[k]);
            }
        }

        /// <summary>
        /// Generates the authentication header with a given combination of <paramref name="username" /> and
        /// <paramref name="password" />
        /// </summary>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>The HTTP authentication header.</returns>
        private static AuthenticationHeaderValue GetBasicCredentialsHeader(string username, string password)
        {
            var toEncode = username + ":" + password;
            var toBase64 = Encoding.UTF8.GetBytes(toEncode);
            var parameter = Convert.ToBase64String(toBase64);
            return new AuthenticationHeaderValue("Basic", parameter);
        }

        /// <summary>
        /// Retrieves an instance of this client ready to use.
        /// </summary>
        /// <remarks>
        /// This method adds all needed headers automatically.
        /// </remarks>
        /// <param name="headers">User defined headers for this specific instance.</param>
        /// <returns>The client to use for calls against the API.</returns>
        private static JsonApiClient GetClient(Dictionary<string, string> headers = null)
        {
            if (BaseApiEndpoint == null)
            {
                throw new InvalidOperationException("No value for BaseApiEndpoint specified.");
            }
            var handler = new HttpClientHandler();
            var client = new JsonApiClient(handler, true)
            {
                BaseAddress = BaseApiEndpoint
            };
            client.DefaultRequestHeaders.Clear();
            if (Credentials != null)
            {
                client.DefaultRequestHeaders.Authorization = GetBasicCredentialsHeader(Credentials.UserName, Credentials.Password);
            }
            client.DefaultRequestHeaders.Accept.Clear();
            // add media type for accept header
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (headers == null)
            {
                // use default headers when no special headers where given.
                headers = DefaultHeaders;
            }
            foreach (var k in headers.Keys)
            {
                client.DefaultRequestHeaders.Add(k, headers[k]);
            }
            AddDebugHeader(client);
            return client;
        }

        /// <summary>
        /// Retrieves the absolute Uri by adding the <paramref name="relativePath" /> to the <see cref="BaseApiEndpoint" />.
        /// </summary>
        /// <param name="relativePath">The relative URL part based on <see cref="BaseApiEndpoint" />.</param>
        /// <param name="queryParams">Optional url parameters.</param>
        /// <returns>The new absolute URL.</returns>
        private static Uri GetUri(string relativePath, IEnumerable<KeyValuePair<string, string>> queryParams = null)
        {
            try
            {
                var query = string.Empty;
                if ((queryParams != null) && queryParams.Any())
                {
                    query = $"?" + ToQuerystring(queryParams);
                }
                return new Uri($"{BaseApiEndpoint}{relativePath}{query}");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Is called to handle each HTTP response internally.
        /// </summary>
        /// <param name="responseMessage">The HTTP reponse message.</param>
        private void HandleResponse(HttpResponseMessage responseMessage)
        {
            LastResponseHeaders = new Dictionary<string, string>();
            foreach (var header in responseMessage.Headers)
            {
                LastResponseHeaders.Add(header.Key, header.Value.ToString());
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The base URI to use for all requests.
        /// </summary>
        public static Uri BaseApiEndpoint { get; set; }

        /// <summary>
        /// The credentials to pass to the authorization header if any.
        /// </summary>
        public static NetworkCredential Credentials { get; set; }

        /// <summary>
        /// All default headers that should be passed to the services when the call is perfomed from
        /// a DEBUG configruation.
        /// </summary>
        /// <remarks>
        /// Put only those headers here which are not part of the <see cref="DefaultHeaders" />.
        /// </remarks>
        public static Dictionary<string, string> DebugHeaders { get; } = new Dictionary<string, string>();

        /// <summary>
        /// All default headers that should be passed to the services in any case.
        /// </summary>
        /// <remarks>
        /// You can use <see cref="DebugHeaders" /> to add headers in DEBUG configuration.
        /// </remarks>
        public static Dictionary<string, string> DefaultHeaders { get; } = new Dictionary<string, string>();

        /// <summary>
        /// The response headers of the last request.
        /// </summary>
        public Dictionary<string, string> LastResponseHeaders { get; private set; }

        /// <summary>
        /// Creates an instance of the client configured for unit tests.
        /// </summary>
        public static JsonApiClient UnitTestClient => GetInstance();

        #endregion
    }
}