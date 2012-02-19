using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NetFrameworkExtensions.Web.Mvc
{
    /// <summary>
    /// Result allowing us to return a custom HttpStatus code as well as a normal view
    /// </summary>
    /// <remarks>
    /// E.g. friendly page not found UI with a 404 status
    /// </remarks>
    public class HttpStatusCodeWithBodyResult : ViewResult
    {
        private int _statusCode;
        private string _description;

        /// <summary>
        /// Result with not view
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="description"></param>
        public HttpStatusCodeWithBodyResult(int statusCode, string description = null) : this(null, statusCode, description) { }

        /// <summary>
        /// Result with a view
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="statusCode"></param>
        /// <param name="description"></param>
        public HttpStatusCodeWithBodyResult(string viewName, int statusCode, string description = null)
        {
            _statusCode = statusCode;
            _description = description;
            ViewName = viewName;
        }

        /// <summary>
        /// Execute by setting Http Status and then calling base ExecuteResult on the ViewResult
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            var httpContext = context.HttpContext;
            var response = httpContext.Response;

            response.StatusCode = _statusCode;
            response.StatusDescription = _description;

            base.ExecuteResult(context);
        }
    }
}
