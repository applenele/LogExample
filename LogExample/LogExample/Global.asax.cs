using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace LogExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            log4net.Config.XmlConfigurator.Configure();

           
        }

        /// <summary>
        /// 在webapi中启用 session
        /// </summary>
        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            base.Init();
        }
    }
}
