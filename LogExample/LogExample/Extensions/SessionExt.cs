using LogExample.Helper;
using LogExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace LogExample.Extensions
{
    public static class SessionExt
    {
        public readonly static string SESSION_KEY = "UserInfo";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="UserGUID"></param>
        public static void PutUserIDInSession(this HttpSessionState Session, string UserID)
        {
            if (Session == null || string.IsNullOrEmpty(UserID))
            {
                throw new ArgumentNullException();
            }
            Session[SESSION_KEY] = UserID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="UserID"></param>
        public static void PutUserIDInSession(this HttpSessionStateBase Session, string UserID)
        {
            if (Session == null || string.IsNullOrEmpty(UserID))
            {
                throw new ArgumentNullException();
            }
            Session[SESSION_KEY] = UserID;
        }


        /// <summary>
        /// 从Session中获取用户GUID
        /// </summary>
        /// <param name="Session">Session</param>
        /// <returns>用户GUID</returns>
        public static string GetCurrentUserID(this HttpSessionState Session)
        {
            return Object2String(Session[SESSION_KEY]);
        }

        /// <summary>
        /// 从Session中获取用户GUID
        /// </summary>
        /// <param name="Session">Session</param>
        /// <returns>用户GUID</returns>
        public static string GetCurrentUserID(this HttpSessionStateBase Session)
        {
            return Object2String(Session[SESSION_KEY]);
        }

        /// <summary>
        /// 从缓存里面获取当前用户信息
        /// </summary>
        /// <param name="UserGUID">用户GUID</param>
        /// <returns>用户信息</returns>
        public static User GetCurrentUserInfo(this HttpSessionStateBase Session)
        {
            string UserGUID = Session.GetCurrentUserID();
            if (string.IsNullOrEmpty(UserGUID))
            {
                return null;
            }
            return GetCurrentUserInfo(UserGUID);
        }

        /// <summary>
        /// 从缓存里面获取当前用户信息
        /// </summary>
        /// <param name="UserGUID">用户GUID</param>
        /// <returns>用户信息</returns>
        public static User GetCurrentUserInfo(this HttpSessionState Session)
        {
            string UserID = Object2String(Session[SESSION_KEY]);
            if (string.IsNullOrEmpty(UserID))
            {
                return null;
            }
            return GetCurrentUserInfo(UserID);
        }

        /// <summary>
        /// 从缓存里面获取当前用户信息
        /// </summary>
        /// <param name="UserGUID">用户GUID</param>
        /// <returns>用户信息</returns>
        public static User GetCurrentUserInfo(string UserID)
        {
            User user = new User();
            if (string.IsNullOrEmpty(UserID))
            {
                throw new ArgumentNullException();
            }
            int id = Convert.ToInt32(EncryptHelper.DESDecode(UserID));
            using (DB db = new DB())
            {
                user = db.Users.FirstOrDefault(x => x.Id == id);
            }
            return user;
        }

        /// <summary>
        /// 将信息保存到Session
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Key">key</param>
        /// <param name="code">信息</param>
        public static void SaveInfo(this HttpSessionState Session, string Key, string code)
        {
            Session[Key] = code;
        }


        #region "私有方法"
        /// <summary>
        /// object转string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Object2String(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
            {
                return null;
            }
            return obj.ToString();
        }
        #endregion

    }
}