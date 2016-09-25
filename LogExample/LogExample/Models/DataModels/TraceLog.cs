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
            this.ActionWatch = new Stopwatch();
            this.ActionWatch.Start();



            this.Operations = new StringBuilder();
        }

        public string Url { get; set; }

        public Stopwatch ActionWatch { get; set; }

        public Stopwatch VIewWatch { get; set; }

        public DateTime ExecuteStartTime { get; set; }

        public DateTime ExecuteEndTime { get; set; }

        public string Cookie { set; get; }

        public string Header { set; get; }


        public StringBuilder Operations { set; get; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string RequestMethod { set; get; }

        /// <summary>
        /// IP
        /// </summary>
        public string Ip { set; get; }

        /// <summary>
        /// 输入的参数
        /// </summary>
        public string Params { set; get; }


        /// <summary>
        /// 获取监控指标日志
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public string GetLogInfo(TraceType mtype = TraceType.MVC)
        {
            string msgContent = null;
            if (mtype == TraceType.MVC)
            {
                string title = "MVC时间监控：";
                msgContent = string.Format(
              @"{0},Url:{1},请求方式:{2},开始时间:{3},结束时间:{4},action总时间:{5}毫秒,view总时间:{6},Cookie:{7},Header:{8},Ip:{9}",
              title,
              Url,
              RequestMethod,
              ExecuteStartTime,
              ExecuteEndTime,
              ActionWatch.ElapsedMilliseconds,
              VIewWatch.ElapsedMilliseconds,
              this.Cookie, Header, Ip);
                if (!string.IsNullOrEmpty(this.Operations.ToString()))
                {
                    msgContent += @",操作详情：" + this.Operations.ToString();
                }
                if (!string.IsNullOrEmpty(Params))
                {
                    msgContent += @",输入参数：" + this.Params;
                }

            }
            if (mtype == TraceType.Api)
            {
                var title = "Api时间监控：";
                msgContent = string.Format(
                 @"{0},Url:{1},请求方式:{2},开始时间:{3},结束时间:{4},action时间:{5}毫秒,Cookie:{6},Header:{7},Ip:{8}",
                    title,
                    Url,
                    RequestMethod,
                    ExecuteStartTime,
                    ExecuteEndTime,
                    ActionWatch.ElapsedMilliseconds,
                    this.Cookie, Header, Ip);
                if (!string.IsNullOrEmpty(this.Operations.ToString()))
                {
                    msgContent += @",操作详情：" + this.Operations.ToString();
                }
                if (!string.IsNullOrEmpty(Params))
                {
                    msgContent += @",输入参数：" + this.Params;
                }
            }

            return msgContent;
        }



    }

}