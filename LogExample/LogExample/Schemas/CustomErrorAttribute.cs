﻿using LogExample.Helper;
using LogExample.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace LogExample.Schemas
{
    /// <summary>
    /// 全局页面控制器异常记录
    /// </summary>
    public class CustomErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            ErrorMessage msg = new ErrorMessage(filterContext.Exception, "页面");
            msg.ShowException = MvcException.IsExceptionEnabled();
            msg.ActionArguments = MvcException.GetCollections(filterContext.HttpContext.Request.Form) + MvcException.GetCollections(filterContext.HttpContext.Request.QueryString);

            //错误记录
            LoggerHelper.Error(JsonConvert.SerializeObject(msg, Formatting.Indented), null);

            //设置为true阻止golbal里面的错误执行
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult() { ViewName = "/Views/Error/ISE.cshtml", ViewData = new ViewDataDictionary<ErrorMessage>(msg) };

        }
    }

    /// <summary>
    /// 全局API异常记录
    /// </summary>
    public class ApiHandleErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext filterContext)
        {
            base.OnException(filterContext);

            //异常信息
            ErrorMessage msg = new ErrorMessage(filterContext.Exception, "接口");
            //接口调用参数
            msg.ActionArguments = JsonConvert.SerializeObject(filterContext.ActionContext.ActionArguments, Formatting.Indented);
            msg.ShowException = MvcException.IsExceptionEnabled();

            //错误记录
            string exMsg = JsonConvert.SerializeObject(msg, Formatting.Indented);
            LoggerHelper.Error(exMsg, null);

            filterContext.Response = new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(exMsg) };
        }
    }

    /// <summary>
    /// 异常信息显示
    /// </summary>
    public class MvcException
    {
        /// <summary>
        /// 是否已经获取的允许显示异常
        /// </summary>
        private static bool HasGetExceptionEnabled = false;

        private static bool isExceptionEnabled;

        /// <summary>
        /// 是否显示异常信息
        /// </summary>
        /// <returns>是否显示异常信息</returns>
        public static bool IsExceptionEnabled()
        {
            if (!HasGetExceptionEnabled)
            {
                isExceptionEnabled = GetExceptionEnabled();
                HasGetExceptionEnabled = true;
            }
            return isExceptionEnabled;
        }

        /// <summary>
        /// 根据Web.config AppSettings节点下的ExceptionEnabled值来决定是否显示异常信息
        /// </summary>
        /// <returns></returns>
        private static bool GetExceptionEnabled()
        {
            bool result;
            if (!Boolean.TryParse(ConfigurationManager.AppSettings["ExceptionEnabled"], out result))
            {
                return false;
            }
            return result;
        }


        // <summary>
        /// 获取Post 或Get 参数
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>
        public static string GetCollections(NameValueCollection collections)
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