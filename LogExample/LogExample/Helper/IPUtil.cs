using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

namespace LogExample.Helper
{
    public static class IPUtil
    {
        private static readonly string HttpContext = "MS_HttpContext";
        private static readonly string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        /// <summary>
        /// IP正则表达式
        /// </summary>
        private static Regex IPRegex = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");

        /// <summary>
        /// 获取客户端请求IP地址
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>request</returns>
        public static string GetIpAddr(this HttpRequestBase request)
        {
            //HTTP_X_FORWARDED_FOR
            string ipAddress = request.ServerVariables["x-forwarded-for"];
            if (!IsEffectiveIP(ipAddress))
            {
                ipAddress = request.ServerVariables["Proxy-Client-IP"];
            }
            if (!IsEffectiveIP(ipAddress))
            {
                ipAddress = request.ServerVariables["WL-Proxy-Client-IP"];
            }
            if (!IsEffectiveIP(ipAddress))
            {
                ipAddress = request.ServerVariables["Remote_Addr"];
                if (ipAddress.Equals("127.0.0.1") || ipAddress.Equals("::1"))
                {
                    // 根据网卡取本机配置的IP
                    IPAddress[] AddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                    foreach (IPAddress _IPAddress in AddressList)
                    {
                        if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                        {
                            ipAddress = _IPAddress.ToString();
                            break;
                        }
                    }
                }
            }
            // 对于通过多个代理的情况，第一个IP为客户端真实IP,多个IP按照','分割
            if (ipAddress != null && ipAddress.Length > 15)
            {
                if (ipAddress.IndexOf(",") > 0)
                {
                    ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(","));
                }
            }
            return ipAddress;
        }

        /// <summary>
        /// WEB API中获取请求IP地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIpAddr(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    if (ctx.Request.UserHostAddress.Equals("127.0.0.1") || ctx.Request.UserHostAddress.Equals("::1"))
                    {
                        // 根据网卡取本机配置的IP
                        IPAddress[] AddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                        foreach (IPAddress _IPAddress in AddressList)
                        {
                            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                            {
                                return _IPAddress.ToString();
                            }
                        }
                    }
                    else
                    {
                        return ctx.Request.UserHostAddress;
                    }
                }
                if (request.Properties.ContainsKey(RemoteEndpointMessage))
                {
                    dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                    if (remoteEndpoint != null)
                    {
                        return remoteEndpoint.Address;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 是否有效IP地址
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>bool</returns>
        public static bool IsEffectiveIP(string ipAddress)
        {
            return !(string.IsNullOrEmpty(ipAddress) || "unknown".Equals(ipAddress, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// 校验用户是否能通过IP限制
        /// </summary>
        /// <param name="ip">用户当前登录IP</param>
        /// <param name="list">用户限制IP集合</param>
        /// <returns>是否能通过</returns>
        public static bool CheckIP(string ip, List<RegexIP> list)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException();
            }
            bool result = false;
            //只要有一个校验通过即可
            foreach (var item in list)
            {
                switch (item.CheckType)
                {
                    case IPCheckType.Accurate:
                        result = Regex.IsMatch(ip, item.Ip);
                        if (result)
                        {
                            return result;
                        }
                        break;
                    case IPCheckType.Range:
                        result = RangeCheck(item.Ip, item.EndIP, ip);
                        if (result)
                        {
                            return result;
                        }
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 校验ip地址是否在区间之内
        /// </summary>
        /// <param name="startIP">起始IP</param>
        /// <param name="endIP">截止IP</param>
        /// <param name="ip">待校验IP</param>
        /// <returns></returns>
        public static bool RangeCheck(string startIP, string endIP, string ip)
        {
            //先替换*
            startIP = startIP.Replace("*", "0");
            endIP = endIP.Replace("*", "255");

            //直接将IP地址转换成Long 比较数值大小即可
            long start = IP2Long(startIP);
            long end = IP2Long(endIP);
            long ipAddress = IP2Long(ip);
            return (ipAddress >= start && ipAddress <= end);
        }

        /// <summary>
        /// 将IP地址转换成数值
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <returns>数值</returns>
        private static long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }

        /// <summary>
        /// 校验IP地址是否合法
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>bool</returns>
        public static bool VaildIP(string ip)
        {
            return IPRegex.IsMatch(ip);
        }

        /// <summary>
        /// 校验起始IP是否小于等于截止IP
        /// </summary>
        /// <param name="startIP">起始IP</param>
        /// <param name="endIP">截止IP</param>
        /// <returns>bool</returns>
        public static bool IpRangeVaild(string startIP, string endIP)
        {
            //直接将IP地址转换成Long 比较数值大小即可
            long start = IP2Long(startIP);
            long end = IP2Long(endIP);
            return !(start > end);
        }

        /// <summary>
        /// 获取服务器本机内网IP地址
        /// </summary>
        /// <returns>本机内网IP地址</returns>
        public static string GetHostIP()
        {
            string hostname = System.Net.Dns.GetHostName();//得到本机名   
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            foreach (IPAddress _IPAddress in localhost.AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    return _IPAddress.ToString();
                }
            }
            return "192.168.0.1";
        }
    }

    /// <summary>
    /// 规则IP校验
    /// </summary>
    public class RegexIP
    {
        /// <summary>
        /// 起始IP
        /// </summary>
        public string Ip { get; set; }

        ///<summary>
        ///范围匹配时采用结束范围 
        ///</summary>
        public string EndIP { get; set; }

        ///<summary>
        ///类别 0：精确匹配或者模糊匹配  1：范围匹配
        ///</summary>
        public IPCheckType CheckType { get; set; }
    }

    /// <summary>
    /// IP类别
    /// </summary>
    public enum IPCheckType
    {
        /// <summary>
        /// 精确匹配或者模糊匹配
        /// </summary>
        Accurate = 0,
        /// <summary>
        /// 范围匹配
        /// </summary>
        Range = 1
    }
}