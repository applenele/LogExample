using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace LogExample.Models.DataModels
{
    public class TraceLog
    {
        public TraceLog()
        {
            this.Watch = new Stopwatch();
            this.Watch.Start();
            this.Operations = new StringBuilder();
        }

        /// <summary>
        /// 监控类型
        /// </summary>
        public enum MonitorType
        {
            /// <summary>
            /// Action
            /// </summary>
            Action = 1,

            /// <summary>
            /// 视图
            /// </summary>
            View = 2
        }


        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public Stopwatch Watch { get; set; }

        public DateTime ExecuteStartTime { get; set; }

        public DateTime ExecuteEndTime { get; set; }

        public string Cookie { set; get; }

        public string Header { set; get; }

        public string Response { set; get; }

        public StringBuilder Operations { set; get; }

        public string Ip { set; get; }

        /// <summary>
        /// Form 表单数据
        /// </summary>
        public NameValueCollection FormCollections { get; set; }

        /// <summary>
        /// URL 参数
        /// </summary>
        public NameValueCollection QueryCollections { get; set; }

        /// <summary>
        /// 文本流
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// 获取监控指标日志
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public string GetLogInfo(MonitorType mtype = MonitorType.Action)
        {
            this.Watch.Stop();
            string actionView = "Action执行时间监控：";
            string action = "Action";
            if (mtype == MonitorType.View)
            {
                actionView = "View视图生成时间监控：";
                action = "View";
            }
            string msgContent = string.Format(
                @"{0}ControllerName：{1}Controller{2}Name:{3}开始时间：{4}结束时间：{5}总时间：{6}秒,Cookie:{7},Header:{8},Ip:{9},操作详情:{10}",
                actionView,
                this.ControllerName,
                action,
                this.ActionName,
                this.ExecuteStartTime,
                this.ExecuteEndTime,
                this.Watch.ElapsedMilliseconds, this.Cookie, Header,Ip, Operations.ToString());

            if (!string.IsNullOrEmpty(this.Raw))
            {
                msgContent += @"Raw：" + this.Raw;
            }
            else if (this.FormCollections != null)
            {
                msgContent += @"Form：" + this.GetCollections(this.FormCollections);
            }
            else if (this.QueryCollections != null)
            {
                msgContent += @"Query：" + this.GetCollections(this.QueryCollections);
            }
            return msgContent;
        }


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