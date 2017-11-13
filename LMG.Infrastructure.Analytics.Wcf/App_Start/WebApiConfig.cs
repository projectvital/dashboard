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
