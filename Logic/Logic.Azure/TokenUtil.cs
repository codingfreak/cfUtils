using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Azure
{
    using System.Threading.Tasks;

    using Base.Utilities;

    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    using Portable.Extensions;

    /// <summary>
    /// Encapsulates logic for retrieving access token from Azure AAD.
    /// </summary>
    public static class TokenUtil
    {
        #region methods

        /// <summary>
        /// Retrieves a new auth token from AAD.
        /// </summary>
        /// <remarks>
        /// Uses the config to retrieve the values. The following elements mus exist in config:
        /// <list type="bullet">
        /// <item><term>ida:Authority</term><description>The root of the authority url.</description></item>
        /// <item><term>ida:TenantDomain</term><description>The domain name of the Azure tenant as the second part of the authority url.</description></item>
        /// <item><term>ida:ServiceUrl</term><description>The url of the service that should be accessed.</description></item>
        /// <item><term>ida:ClientId</term><description>The unique client id as it is configured in Azure Portal.</description></item>
        /// <item><term>ida:AppKey</term><description>This value is optional and contains the App-Key-Secret if it is configured in azure portal.</description></item>
        /// <item><term>ida:RedirectUrl</term><description>The redirect url as it is configured in Azure Portal.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>The authentication token.</returns>
        public static async Task<string> RetrieveTokenAsync()
        {
            var url = ConfigurationUtil.Get<string>("ida:Authority");
            var tenantDomain = ConfigurationUtil.Get<string>("ida:TenantDomain");
            var targetServiceUrl = ConfigurationUtil.Get<string>("ida:ServiceUrl");
            var clientId = ConfigurationUtil.Get<string>("ida:ClientId");
            var appKey = ConfigurationUtil.Get<string>("ida:AppKey", null);
            var redirectUrl = ConfigurationUtil.Get<string>("ida:RedirectUrl");
            if (!url.IsNullOrEmpty() && !tenantDomain.IsNullOrEmpty() && !targetServiceUrl.IsNullOrEmpty() && !clientId.IsNullOrEmpty() && !redirectUrl.IsNullOrEmpty())
            {                
                Uri redirectUri = null;
                try
                {                    
                    redirectUri = new Uri(redirectUrl);
                }
                catch (Exception ex)
                {
                    throw new FormatException("Some uri-parameters are not formed correctly.", ex);
                }
                return await RetrieveTokenAsync(url, tenantDomain, targetServiceUrl, clientId, redirectUri, appKey).ConfigureAwait(false);
            }
            throw new ArgumentException("Some parameters are missing in configuration.");
        }

        /// <summary>
        /// Retrieves a new auth token from AAD.
        /// </summary>
        /// <param name="authUrl">The root of the authority url.</param>
        /// <param name="tenantDomain">The domain name of the Azure tenant as the second part of the authority url.</param>
        /// <param name="targetServiceUrl">The url of the service that should be accessed. Be sure to check trailing slashes!</param>
        /// <param name="clientId">The unique client id as it is configured in Azure Portal.</param>
        /// <param name="appKey">This value is optional and contains the App-Key-Secret if it is configured in azure portal.</param>
        /// <param name="redirectUrl">The redirect url as it is configured in Azure Portal.</param>
        /// <returns>The authentication token.</returns>
        public static async Task<string> RetrieveTokenAsync(string authUrl, string tenantDomain, string targetServiceUrl, string clientId, Uri redirectUrl, string appKey = null)
        {
            var authenticationContext = new AuthenticationContext($"{authUrl}/{tenantDomain}");
            try
            {
                AuthenticationResult result = null;                
                if (appKey.IsNullOrEmpty())
                {
                    // use user auth
                    var parameters = new PlatformParameters(PromptBehavior.Auto);
                    result = await authenticationContext.AcquireTokenAsync(targetServiceUrl, clientId, redirectUrl, parameters).ConfigureAwait(false);
                }
                else
                {
                    // use key auth
                    var clientCredential = new ClientCredential(clientId, appKey);
                    result = await authenticationContext.AcquireTokenAsync(targetServiceUrl, clientCredential).ConfigureAwait(false);
                }
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                }
                // store token for reuse
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not retrieve token.", ex);
            }
        }

        #endregion
    }
}