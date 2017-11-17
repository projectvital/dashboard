/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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