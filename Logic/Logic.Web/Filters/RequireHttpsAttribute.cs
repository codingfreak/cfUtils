namespace codingfreaks.cfUtils.Logic.WebUtils.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using Base.Utilities;

    /// <summary>
    /// Is used to enforce HTTPS communication on a <see cref="ApiController"/> or one of its methods.
    /// </summary>
    public class RequireHttpsApiAttribute : AuthorizationFilterAttribute
    {
        #region methods

        /// <summary>
        /// Calls when a process requests authorization.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using <see cref="T:System.Web.Http.Filters.AuthorizationFilterAttribute"/>.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            if (request.RequestUri.Scheme == Uri.UriSchemeHttps)
            {
                return;
            }
            HttpResponseMessage response;
            var uri = new UriBuilder(request.RequestUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = ConfigurationUtil.Get("System.Web.SslPort", 443)
            };
            var body = string.Format("<p>The resource can be found at <a href=\"{0}\">{0}</a>.</p>", uri.Uri.AbsoluteUri);
            if (request.Method.Equals(HttpMethod.Get) || request.Method.Equals(HttpMethod.Head))
            {
                response = request.CreateResponse(HttpStatusCode.Found);
                response.Headers.Location = uri.Uri;
                if (request.Method.Equals(HttpMethod.Get))
                {
                    response.Content = new StringContent(body, Encoding.UTF8, "text/html");
                }
            }
            else
            {
                response = request.CreateResponse(HttpStatusCode.NotFound);
                response.Content = new StringContent(body, Encoding.UTF8, "text/html");
            }
            actionContext.Response = response;
        }

        #endregion
    }
}