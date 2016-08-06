using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LogExample.Extensions
{
    public static class CookieExtensions
    {
        /// <summary>
        ///  讲cookie转成string 形式
        /// </summary>
        /// <param name="Cookie"></param>
        /// <returns></returns>
        public static string ToDictString(this HttpCookieCollection Cookie)
        {
            StringBuilder sbd = new StringBuilder();
            string[] keys = Cookie.AllKeys;
            for (int i=0;i<keys.Length;i++)
            {
                sbd.AppendFormat("{0}:{1}{2}", keys[i], Cookie[keys[i]],(i==keys.Length-1)?"":",");
            }
            return sbd.ToString();
        }
    }
}