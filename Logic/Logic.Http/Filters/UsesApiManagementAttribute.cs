namespace s2.s2Utils.Logic.WebUtils.Filters
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// If used to ensure that requests will come from Azure API Management and are not stated directly.
    /// </summary>
    /// <remarks>
    /// Be aware that this implementation is not hardened against attacks!
    /// </remarks>
    public class UsesApiManagementAttribute : ActionFilterAttribute
    {
        #region constructors and destructors

        public UsesApiManagementAttribute(string optionalApiKey)
        {
            ApiDirectKey = optionalApiKey;
        }

        #endregion

        #region methods

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var agent = string.Empty;            
            var apiManagementOk = CheckForApiMangement(actionContext);
            var keyOk = CheckForApiKey(actionContext);
            if (!apiManagementOk && !keyOk)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Requests are only allowed using the API management URLs.");
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.UseProxy, message);                
            }
            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Checks if the correct API key was passed as a parameter.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if the parameter was passed otherwise <c>false</c>.</returns>
        private bool CheckForApiKey(HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(ApiDirectKey))
            {
                return false;
            }
            var queryParsed = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
            if (!queryParsed.AllKeys.Contains("api_key"))
            {
                return false;
            }
            return queryParsed["api_key"].Equals(ApiDirectKey, StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks if the correct API key was passed as a parameter.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if the parameter was passed otherwise <c>false</c>.</returns>
        private bool CheckForApiMangement(HttpActionContext actionContext)
        {
            return actionContext.Request.Headers.Contains("Ocp-Apim-Subscription-Key");
        }

        #endregion

        #region properties

        /// <summary>
        /// A key that can be used when the request should be allowed without API management.
        /// </summary>
        public string ApiDirectKey { get; }

        #endregion
    }
}