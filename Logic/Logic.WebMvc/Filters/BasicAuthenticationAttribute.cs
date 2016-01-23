namespace codingfreaks.cfUtils.Logic.WebMvcUtils.Filters
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web.Mvc;

    using Portable.Extensions;

    /// <summary>
    /// Can be used to force the client to enter a fixed <see cref="Username"/> and <see cref="Password"/> as
    /// an equivalent to htaccess/htpasswd.
    /// </summary>
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        #region constructors and destructors

        /// <summary>
        /// Default contructor.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">he password for the <see cref="Username"/>.</param>
        public BasicAuthenticationAttribute(string username, string password)
        {
            Username = username;
            Password = password;
        }

        #endregion

        #region properties

        /// <summary>
        /// Defines the name for the area to protect.
        /// </summary>
        public string BasicRealm { get; set; }

        /// <summary>
        /// The password for the <see cref="Username"/>.
        /// </summary>
        protected string Password { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        protected string Username { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];
            if (!auth.IsNullOrEmpty())
            {
                var parts = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new
                {
                    Name = parts[0],
                    Pass = parts[1]
                };
                if (user.Name == Username && user.Pass == Password)
                {
                    return;
                }
            }
            var res = filterContext.HttpContext.Response;
            res.StatusCode = 401;
            res.AddHeader("WWW-Authenticate", string.Format(CultureInfo.InvariantCulture, "Basic realm=\"{0}\"", BasicRealm ?? Guid.NewGuid().ToString("N")));
            res.End();
        }

        #endregion
    }
}