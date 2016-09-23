using LogExample.Extensions;
using LogExample.Helper;
using LogExample.Models;
using LogExample.Models.DataModels;
using LogExample.Schemas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using Newtonsoft.Json.Linq;

namespace LogExample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { action = new StartWithConstraint() }
            );

            config.Filters.Add(new ApiPermissionFilter());  //注册全局API Action过滤器
            config.Filters.Add(new ApiHandleErrorAttribute());  //注册全局异常过滤器
            config.Filters.Add(new ModelStateFilterAttribute()); // model 实体验证

            //添加Trim去除空格 只会对json数据有用
            config.Formatters.JsonFormatter.SerializerSettings.Converters
             .Add(new TrimmingConverter());
        }
    }



    public class ApiPermissionFilter : ActionFilterAttribute
    {

        public User CurrentUser { set; get; }
        /// <summary>
        /// 2016-01-06 对API接口用户认证添加新的方式 目前支持两种
        /// 1：session认证
        /// 2：令牌认证方式 其中令牌中需要包含用户GUID信息 并且是进行加密有时间限制
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                // 匿名访问验证
                var anonymousAction = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>();
                if (!anonymousAction.Any())
                {
                    //得到用户登录的信息
                    //两种方式1.从Session获取
                    CurrentUser = HttpContext.Current.Session.GetCurrentUserInfo();
                    if (CurrentUser == null)
                    {
                        //从请求头获取Authorization
                        var Authorization = actionContext.Request.Headers.Authorization;
                        if (Authorization != null && Authorization.Scheme == "Basic")
                        {
                            UserToken token = null;
                            if (TokenHelper.VerifyToken(Authorization.Parameter, out token))
                            {
                                CurrentUser = SessionExt.GetCurrentUserInfo(token.UserID);
                            }
                        }
                    }
                    //判断用户是否为空
                    if (CurrentUser == null)
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("logout"));
                    }
                }

                var TraceLog = new TraceLog();
                ///开始记录追踪日志
                TraceLog.ExecuteStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
                TraceLog.Url = actionContext.Request.RequestUri.AbsoluteUri;
                TraceLog.Cookie = HttpContext.Current.Request.Cookies.ToDictString();
                TraceLog.Header = actionContext.Request.Headers.ToString();
                TraceLog.Ip = actionContext.Request.GetIpAddr();
                TraceLog.RequestMethod = actionContext.Request.Method.Method;
                TraceLog.Input = CollectionHelper.GetCollections(HttpContext.Current.Request.Form) + CollectionHelper.GetCollections(HttpContext.Current.Request.Form); //获取参数

                actionContext.Request.Properties["TraceLog"] = TraceLog;

                base.OnActionExecuting(actionContext);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// action 执行之后
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var TraceLog = actionExecutedContext.Request.Properties["TraceLog"] as TraceLog;
            if (TraceLog != null)
            {
                TraceLog.ExecuteEndTime = DateTime.Now;
                //TraceLog.Response = HttpContext.Current.Response.ToString();

                Task.Factory.StartNew(() =>
                {
                    LoggerHelper.Monitor(TraceLog.GetLogInfo());
                });
            }


            ///保存操作日志到数据库  在这里也可以可以获取ActionName 和ControllerName  
           // var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            var attrs = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<RequireLogAttribute>();
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
                        Url = HttpContext.Current.Request.Url.AbsoluteUri,
                        UserId = CurrentUser == null ? 0 : CurrentUser.Id,
                        UserName = CurrentUser == null ? "" : CurrentUser.UserName
                    };
                    db.OperateLogs.Add(log);
                    db.SaveChanges();
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }

    /// <summary>
    /// 如果请求url如： api/area/controller/x  x有可能是actioin或id
    /// 在url中的x位置出现的是以 get put delete post开头的字符串，则当作action,否则就当作id
    /// 如果action为空，则把请求方法赋给action
    /// </summary>
    public class StartWithConstraint : IHttpRouteConstraint
    {
        public string[] StartWithArray { get; set; }
        private string _id = "id";

        public StartWithConstraint(string[] startwithArray = null)
        {
            if (startwithArray == null)
                startwithArray = new string[] { "GET", "PUT", "DELETE", "POST", "EDIT", "UPDATE", "AUDIT", "DOWNLOAD" };

            this.StartWithArray = startwithArray;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (values == null) // shouldn't ever hit this.                   
                return true;

            if (!values.ContainsKey(parameterName) || !values.ContainsKey(_id)) // make sure the parameter is there.
                return true;

            var action = values[parameterName].ToString().ToLower();
            if (string.IsNullOrEmpty(action)) // if the param key is empty in this case "action" add the method so it doesn't hit other methods like "GetStatus"
            {
                values[parameterName] = request.Method.ToString();
            }
            else if (string.IsNullOrEmpty(values[_id].ToString()))
            {
                var isidstr = true;
                StartWithArray.ToList().ForEach(x =>
                {
                    if (action.StartsWith(x.ToLower()))
                        isidstr = false;
                });

                if (isidstr)
                {
                    values[_id] = values[parameterName];
                    values[parameterName] = request.Method.ToString();
                }
            }
            return true;
        }
    }


    /// <summary>
    /// Triming 去掉空格 
    /// </summary>
    public class TrimmingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                if (reader.Value != null)
                    return (reader.Value as string).Trim();

            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var text = (string)value;
            if (text == null)
                writer.WriteNull();
            else
                writer.WriteValue(text.Trim());
        }
    }

     
}
