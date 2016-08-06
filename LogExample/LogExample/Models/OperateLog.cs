using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogExample.Models
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class OperateLog
    {
        [Key]
        public int Id { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// ip
        /// </summary>
        public string Ip { set; get; }

        /// <summary>
        /// 访问的地址
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { set; get; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OperateType OperateType { set; get; }

        /// <summary>
        /// 用户的Id
        /// </summary>
        public int UserId { set; get; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; }
    }

    public enum OperateType
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
}