using LogExample.Helper;
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



            //加载log4net 配置
            LoggerHelper.SetConfig(new System.IO.FileInfo(Server.MapPath("~/log4net.config")));

            //数据参数去掉空格
            ModelBinders.Binders.Add(typeof(string), new StringTrimModelBinder());
        }

        /// <summary>
        /// 在webapi中启用 session
        /// </summary>
        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            base.Init();
        }

        /// <summary>
        ///  数据参数字符串去掉空格
        /// </summary>
        public class StringTrimModelBinder : DefaultModelBinder
        {
            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var value = base.BindModel(controllerContext, bindingContext);
                if (value is string) return (value as string).Trim();
                return value;
            }
        }
    }
}
