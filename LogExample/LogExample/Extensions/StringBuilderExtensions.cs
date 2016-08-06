using LogExample.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;

namespace LogExample.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        ///  增加操作到TraceLog里面 主要用于Api时操作方法的记录
        /// </summary>
        /// <param name="sbd"></param>
        /// <param name="ActionContext"></param>
        public static void AddToTraceLog(this StringBuilder sbd, HttpActionContext ActionContext)
        {
            TraceLog TraceLog = ActionContext.Request.Properties["TraceLog"] as TraceLog;
            TraceLog.Operations = sbd;
            ActionContext.Request.Properties["TraceLog"] = TraceLog;
        }
    }
}