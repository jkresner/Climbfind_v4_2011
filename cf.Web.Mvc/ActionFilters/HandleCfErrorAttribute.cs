using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Instrumentation;
using System.Web.Mvc;
using System.Web.Routing;
using cf.Identity;

namespace cf.Web.Mvc.ActionFilters
{
    public class HandleCfErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            CfTrace.Error(context.Exception);

            if (context.Exception is AccessViolationException)
            {
                context.Result = new ViewResult() { ViewName = "Unauthorized" };
                (context.Result as ViewResult).ViewBag.Msg = context.Exception.Message;
                context.ExceptionHandled = true;
            }
            else
            {
                var ex = getBaseException(context.Exception);

                var errorDisplayText = ex.Message;
                if (CfIdentity.IsAuthenticated && CfPrincipal.IsGod()) { errorDisplayText = ex.ToString(); }

                context.Result = new ViewResult() { ViewName = "Error" };
                (context.Result as ViewResult).ViewBag.Msg = errorDisplayText;
                context.ExceptionHandled = true;
            }


            base.OnException(context);
        }


        private static Exception getBaseException(Exception appException)
        {
            Exception baseException = appException;

            //-- Navigate down to find the first Exception
            while (baseException.InnerException != null)
            {
                baseException = baseException.InnerException;
            }

            return (baseException);
        } 
    }
}