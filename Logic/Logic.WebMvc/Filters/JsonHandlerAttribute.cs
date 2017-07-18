namespace codingfreaks.cfUtils.Logic.WebMvcUtils.Filters
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Models;

    /// <summary>
    /// Can be applied globally or at any method to replace the default serialisation
    /// of MVC with JSON.NET.
    /// </summary>
    public class JsonHandlerAttribute : ActionFilterAttribute
    {
        #region methods

        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var jsonResult = filterContext.Result as JsonResult;
            if (jsonResult != null)
            {
                // it is a JsonResult
                filterContext.Result = new JsonNetResult
                {
                    ContentEncoding = jsonResult.ContentEncoding,
                    ContentType = jsonResult.ContentType,
                    Data = jsonResult.Data,
                    JsonRequestBehavior = jsonResult.JsonRequestBehavior
                };
            }
            base.OnActionExecuted(filterContext);
        }

        #endregion
    }
}