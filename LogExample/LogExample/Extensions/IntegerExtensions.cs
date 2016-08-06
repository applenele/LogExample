using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogExample.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// int 转enum
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int input)
        {
            return (T)Enum.ToObject(typeof(T), input);
        }

    }
}