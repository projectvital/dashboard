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
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LMG.Infrastructure.Analytics.Wcf
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ChartDataApi",
                routeTemplate: "v1/ChartData/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DataApi",
                routeTemplate: "v1/Data/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
             );

            config.Routes.MapHttpRoute(
                name: "GenericApi",
                routeTemplate: "v1/{controller}/{option}/{id}",
                defaults: new { id = RouteParameter.Optional, option = RouteParameter.Optional }
             );

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings
                .Add(new RequestHeaderMapping("Accept",
                              "text/html",
                              StringComparison.InvariantCultureIgnoreCase,
                              true,
                              "text/plain"/*"text/csv"/*"application/json"*/));


            var cors = new EnableCorsAttribute("http://dashboards.uva.project-vital.eu,http://dashboards.uclan.project-vital.eu,http://dashboards.uhasselt.project-vital.eu,http://staging.project-vital.eu,http://localhost:49589", "*", "*");
            config.EnableCors(cors);
            config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));

            
        }
    }
}
