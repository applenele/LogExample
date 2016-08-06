using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace LogExample.Helper
{
    public class CollectionHelper
    {

        /// <summary>
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