using LogExample.Models.Enum;
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

        public string Url { get; set; }

        public Stopwatch Watch { get; set; }

        public DateTime ExecuteStartTime { get; set; }

        public DateTime ExecuteEndTime { get; set; }

        public string Cookie { set; get; }

        public string Header { set; get; }

        public string Response { set; get; }

        public StringBuilder Operations { set; get; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string RequestMethod { set; get; }

        public string Ip { set; get; }

        /// <summary>
        /// 输入的参数
        /// </summary>
        public string Input { set; get; }

        /// <summary>
        /// 返回
        /// </summary>
        public string Output { set; get; }


        /// <summary>
        /// 获取监控指标日志
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public string GetLogInfo(TraceType mtype = TraceType.Action)
        {
            this.Watch.Stop();
            string actionView = "Action执行时间监控：";
            string action = "Action";
            if (mtype == TraceType.View)
            {
                actionView = "View视图生成时间监控：";
                action = "View";
            }
            string msgContent = string.Format(
                @"{0},Url:{1},请求方式:{2},开始时间:{3}结束时间:{4}总时间:{5}秒,Cookie:{6},Header:{7},Ip:{8},操作详情:{9},响应:{10}",
                actionView,
                Url,
                RequestMethod,
                ExecuteStartTime,
                ExecuteEndTime,
                Watch.ElapsedMilliseconds, this.Cookie, Header, Ip, Operations.ToString(), Response);
            if (!string.IsNullOrEmpty(Input))
            {
                msgContent += @",输入参数：" + this.Input;
            }
            else if (!string.IsNullOrEmpty(Output))
            {
                msgContent += @",输出参数：" + this.Output;
            }

            return msgContent;
        }



    }

}