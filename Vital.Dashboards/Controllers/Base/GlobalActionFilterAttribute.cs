using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vital.Dashboards.Controllers.Base
{
    public class GlobalActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.ApiUrl = ApiUrl;
        }

        public static string ApiUrl
        {
            get
            {
#if (DEBUG)
                return "http://localhost:55648";
#else
                return (System.Web.Configuration.WebConfigurationManager.AppSettings["ApiUrl"]);
#endif
            }
        }
        public static string ApiUrlServerSide
        {
            get
            {
                if (ApiUrl == "http://staging.api.project-vital.eu")
                    return "http://different.url.if.needed";
                return ApiUrl;
            }
        }
     
    }
}