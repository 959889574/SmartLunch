using SL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SL.Utility;

namespace SL.CAS
{
    public class SmartEyeLoginSession
    {
        private const string SessionName = "SmartEyeLoginUserSessionName";
        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        public static LoginInfoModel GetSession()
        {
            LoginInfoModel userInfo = null;
            string user = string.Empty;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.Cookies[SessionName] != null)
                {
                    user = HttpContext.Current.Request.Cookies[SessionName].Value.DBColToString();
                }
                else
                {
                    user = HttpContext.Current.Session[SessionName].DBColToString();
                }
                if (!string.IsNullOrEmpty(user))
                {
                    user = user.Contains("[Encrypt]") ? DesCodeHelper.DecryptDes(user.Substring(9, user.Length - 9)) : user;
                    userInfo = JsonConvert.DeserializeObject<LoginInfoModel>(user);
                }
            }
            return userInfo;
        }
     
        /// <summary>
        /// 保存智慧识货登录信息
        /// </summary>
        /// <param name="userInfo">登录信息</param>
        /// <param name="type">0 cookie 1 Session 不填或其他值都默认cookie保存</param>
        /// <param name="isEncrypt">是否加密</param>
        public static void SetSmartSession(LoginInfoModel userInfo, int type = 0, bool isEncrypt = true)
        {
            if (userInfo != null)
            {
                string user = isEncrypt ? "[Encrypt]" + DesCodeHelper.EncryptDes(JsonConvert.SerializeObject(userInfo)) : JsonConvert.SerializeObject(userInfo);
                switch (type)
                {
                    case 1:
                        if (HttpContext.Current != null)
                            HttpContext.Current.Session.Add(SessionName, user);
                        break;
                    case 0:
                    default:
                        HttpCookie cookie = new HttpCookie(SessionName)
                        {
                            Expires = DateTime.Now.AddHours(2),
                            Value = user
                        };
                        HttpContext.Current.Response.Cookies.Add(cookie);
                        break;
                }
            }
        }

        /// <summary>
        /// 清除登录信息
        /// </summary>
        /// <param name="userInfo"></param>
        public static void DelSession()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session.Remove(SessionName);
                HttpContext.Current.Request.Cookies.Remove(SessionName);
            }
        }

        /// <summary>
        /// 是否登录状态
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            bool flag = false;
            var userInfo = GetSession();
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.UserName))
                flag = true;
            return flag;
        }
    }

}