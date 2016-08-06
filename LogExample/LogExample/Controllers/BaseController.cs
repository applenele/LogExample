using LogExample.Extensions;
using LogExample.Helper;
using LogExample.Models;
using LogExample.Models.DataModels;
using LogExample.Schemas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LogExample.Controllers
{
    public class BaseController : Controller
    {

        public TraceLog TraceLog { set; get; }

        public User CurrentUser { set; get; }

        protected override void Initialize(RequestContext requestContext)
        {
            TraceLog = new TraceLog();
            base.Initialize(requestContext);
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CurrentUser = filterContext.HttpContext.Session.GetCurrentUserInfo();
            if (CurrentUser == null)
            {
                var attr = filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>();
                bool isAnonymous = attr.Any(a => a is AllowAnonymousAttribute);
                if (!isAnonymous)
                    filterContext.Result = RedirectToRoute("Default", new { controller = "Account", action = "Login" });
            }

            TraceLog.ExecuteStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            TraceLog.ControllerName = filterContext.RouteData.Values["controller"] as string;
            TraceLog.ActionName = filterContext.RouteData.Values["action"] as string;
            TraceLog.Cookie = filterContext.HttpContext.Request.Cookies.ToDictString();
            TraceLog.Header = filterContext.HttpContext.Request.Headers.ToString();
            TraceLog.Ip = filterContext.HttpContext.Request.GetIpAddr();

            base.OnActionExecuting(filterContext);
        }



        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //TraceLog monLog = filterContext.Controller.ViewData["TraceLog"] as TraceLog;
            TraceLog.ExecuteEndTime = DateTime.Now;
            TraceLog.FormCollections = filterContext.HttpContext.Request.Form;//form表单提交的数据
            TraceLog.QueryCollections = filterContext.HttpContext.Request.QueryString;//Url 参数
            TraceLog.Response = filterContext.HttpContext.Response.ToString();

            Task.Factory.StartNew(() =>
            {
                LoggerHelper.Monitor(TraceLog.GetLogInfo());
            });


            ///保存操作日志到数据库
            var attrs = filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<RequireLogAttribute>();
            var _attr = attrs.Where(a => a is RequireLogAttribute).FirstOrDefault();
            if (_attr != null)
            {
                using (DB db = new DB())
                {
                    var log = new OperateLog()
                    {
                        CreatedDate = DateTime.Now,
                        Description = TraceLog.Operations.ToString(),
                        Ip = TraceLog.Ip,
                        OperateType = _attr.OperateType,
                        Url = filterContext.HttpContext.Request.Url.AbsoluteUri,
                        UserId = CurrentUser == null ? 0 : CurrentUser.Id,
                        UserName = CurrentUser == null ? "" : CurrentUser.UserName
                    };

                    db.OperateLogs.Add(log);
                    db.SaveChanges();
                }
            }
            base.OnActionExecuted(filterContext);
        }



    }
}