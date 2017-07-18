namespace codingfreaks.cfUtils.Logic.WebMvcUtils.Models
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Filters;

    using Newtonsoft.Json;

    /// <summary>
    /// Is used by the <see cref="JsonHandlerAttribute" /> to implement JSON.NET replacement
    /// of the default serialization.
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        #region methods

        /// <inheritdoc />
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("GET ist not allowed.");
            }
            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data == null)
            {
                return;
            }
            var result = JsonConvert.SerializeObject(Data);
            response.Write(result);
        }

        #endregion
    }
}