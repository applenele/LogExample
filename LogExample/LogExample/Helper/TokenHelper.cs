using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogExample.Helper
{
    /// <summary>
    /// 用户认证令牌
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 令牌生成时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="UserID">当前用户</param>
        public UserToken(string UserID)
        {
            this.UserID = UserID;
            Time = DateTime.Now;
        }
    }

    /// <summary>
    /// 用户令牌帮助类
    /// </summary>
    public class TokenHelper
    {
        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="UserID">当前用户</param>
        /// <returns>用户令牌</returns>
        public static string CreateToken(string UserID)
        {
            string token = JsonConvert.SerializeObject(new UserToken(UserID));
            return EncryptHelper.DESEncode(token);
        }

        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="UserToken">用户认证令牌实体</param>
        /// <returns>用户令牌</returns>
        public static string CreateToken(UserToken UserToken)
        {
            string token = JsonConvert.SerializeObject(UserToken);
            return EncryptHelper.DESEncode(token);
        }

        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="token">用户认证令牌</param>
        /// <returns>用户令牌</returns>
        public static UserToken RestoreToken(string token)
        {
            return JsonConvert.DeserializeObject<UserToken>(EncryptHelper.DESDecode(token));
        }

        /// <summary>
        /// 验证token有效性
        /// </summary>
        /// <param name="token">用户认证令牌</param>
        /// <returns></returns>
        public static bool VerifyToken(string token, out UserToken userToken)
        {
            UserToken UserToken = RestoreToken(token);
            userToken = UserToken;
            if (UserToken == null || string.IsNullOrEmpty(UserToken.UserID))
            {
                return false;
            }
            //验证令牌是否过期,默认有效期30分钟
            if ((DateTime.Now - UserToken.Time).Minutes > 30)
            {
                return false;
            }
            return true;
        }
    }
}