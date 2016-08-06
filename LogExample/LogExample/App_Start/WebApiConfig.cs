using LogExample.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LogExample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new ApiHandleErrorAttribute());  //注册全局异常过滤器
        }
    }
}
