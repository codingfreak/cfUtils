using System;
using System.Linq;

namespace s2.s2Utils.Logic.WebUtils.Filters
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using Base.Utilities;

    /// <summary>
    /// Abstract base class for a filter that checks mandatory headers.
    /// </summary>    
    public abstract class BaseHeaderCheckFilterAttribute : ActionFilterAttribute
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseHeaderCheckFilterAttribute()
        {
        }

        /// <summary>
        /// Constructor allowing to pass in mandatory headers to ignore even if they are defined.
        /// </summary>
        /// <param name="headersToIgnore"></param>
        public BaseHeaderCheckFilterAttribute(string[] headersToIgnore)
        {
            HeadersToIgnore = headersToIgnore;
        }

        #endregion

        #region methods

        /// <summary>
        /// Is called when the action filter is on his turn to perform the logic.
        /// </summary>
        /// <param name="actionContext">The context of the action.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            AddHeadersToIgnore(actionContext);
            var result = CheckHeaders(headers);            
            if (!result.Item1)
            {
                AddDebugResponsePhrase(actionContext, result.Item2);
#if !DEBUG
                AddReleaseResponsePhrase(actionContext);
#endif
            }
            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Is called by <see cref="OnActionExecuting"/> to give possibility to act on check before it happens.
        /// </summary>
        /// <param name="actionContext">The current action context.</param>
        protected virtual void AddHeadersToIgnore(HttpActionContext actionContext)
        {            
        }

        /// <summary>
        /// Checks the given <paramref name="headers" />.
        /// </summary>
        /// <param name="headers">The request headers to check.</param>
        /// <returns>
        /// A tuple containing a boolean in Item1 showing if the check resulted ok and an optional message for the client in Item2.
        /// </returns>
        protected abstract Tuple<bool, string> CheckHeaders(HttpRequestHeaders headers);

        /// <summary>
        /// Is called to add the given <paramref name="reasonPhrase" /> inside a new <see cref="HttpResponseMessage" /> into the
        /// <paramref name="actionContext" />.
        /// </summary>
        /// <param name="actionContext">The HTTP context where to add the response to.</param>
        /// <param name="reasonPhrase">A text which will returned to the HTTP client.</param>
        [Conditional("DEBUG")]
        private static void AddDebugResponsePhrase(HttpActionContext actionContext, string reasonPhrase)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = reasonPhrase
            };
        }

        /// <summary>
        /// Is called to add a static reason phrase inside a new <see cref="HttpResponseMessage" /> into the <paramref name="actionContext" />.
        /// </summary>
        /// <param name="actionContext">The HTTP context where to add the response to.</param>        
        private static void AddReleaseResponsePhrase(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "Headers invalid."
            };
        }

        #endregion

        #region properties

        /// <summary>
        /// Provides a list of header tags which are ignored in the check for mandantory headers.
        /// </summary>
        public string[] HeadersToIgnore { get; set; }

        #endregion
    }
}