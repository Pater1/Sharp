using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sharp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "API_PullChunk",
                url: "API/PullChunk/{partyKey}/{samplesRequested}/{channelCount}",
                defaults: new { controller = "API", action = "PullChunk" }
            );
            routes.MapRoute(
                name: "API_PushChunk",
                url: "API/PushChunk/{partyKey}/{pushedChunk}",
                defaults: new { controller = "API", action = "PushChunk" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
