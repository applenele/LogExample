using LogExample.Extensions;
using LogExample.Helper;
using LogExample.Models;
using LogExample.Models.DataModels;
using LogExample.Schemas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        /// <summary>
        /// Action 之前执行
        /// </summary>
        /// <param name="filterContext"></param>
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

            ///开始记录追踪日志
            TraceLog.ExecuteStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            TraceLog.Url = filterContext.HttpContext.Request.Url.AbsoluteUri;
            TraceLog.Cookie = filterContext.HttpContext.Request.Cookies.ToDictString();
            TraceLog.Header = filterContext.HttpContext.Request.Headers.ToString();
            TraceLog.Ip = filterContext.HttpContext.Request.GetIpAddr();
            TraceLog.RequestMethod = filterContext.HttpContext.Request.HttpMethod;
            TraceLog.Input = GetCollections(filterContext.HttpContext.Request.Form) + GetCollections(filterContext.HttpContext.Request.QueryString); //获取参数
            base.OnActionExecuting(filterContext);
        }


        /// <summary>
        /// Action 执行完成之后
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //TraceLog monLog = filterContext.Controller.ViewData["TraceLog"] as TraceLog;
            TraceLog.ExecuteEndTime = DateTime.Now;
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



        #region View 视图生成时间监控 可以记录生成的日志追踪 
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        #endregion


        // <summary>
        /// 获取Post 或Get 参数
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>
        public string GetCollections(NameValueCollection collections)
        {
            string parameters = string.Empty;
            if (collections == null || collections.Count == 0)
            {
                return parameters;
            }
            parameters = collections.Keys.Cast<string>()
                .Aggregate(parameters, (current, key) => current + string.Format("{0}={1}&", key, collections[key]));
            if (!string.IsNullOrWhiteSpace(parameters) && parameters.EndsWith("&"))
            {
                parameters = parameters.Substring(0, parameters.Length - 1);
            }
            return parameters;
        }
    }
}