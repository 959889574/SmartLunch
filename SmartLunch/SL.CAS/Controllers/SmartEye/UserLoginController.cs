using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using SL.DBHelper;
using System.Data;


namespace SL.CAS
{
    public class UserLoginController : Controller
    {
        // GET: /Account/
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoLogin(UserModel model)
        {
            try
            {
                string msg = string.Empty;
                bool falg = false;

                string sql = string.Format(" SELECT UserName FROM TAdminUser WHERE UserName=@UserName AND PASSWORD=@PASSWORD ");

                List<SqlParameter> para = new List<SqlParameter>();
                para.Add(new SqlParameter("@UserName", model.username));
                para.Add(new SqlParameter("@PASSWORD", model.password));

                DataSet ds = SQLHelper.Instance.ExecuteDataSet(sql, para.ToArray());

                if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    SmartEyeLoginSession.SetSmartSession(new LoginInfoModel(ds.Tables[0].Rows[0]["UserName"].ToString()), 1);
                    falg = true;
                    msg = "登录成功";
                }
                else
                {
                    msg = "登录失败,请输入正确的账号信息";
                }

                return Json(new
                {
                    status = falg ? "success" : "",
                    message = msg
                });
            }
            catch (Exception ex)
            {
                return Json(new
            {
                status = "false",
                message = ex.Message
            });
            }
        }
          

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [SmartEyeLoginFilter]
        public ActionResult outLogin()
        {
            LoginSession.DelSession();
            return RedirectToAction("Login", "UserLogin");
        }
    }

    public class UserModel
    {
        /// <summary>
        /// 客户名
        /// </summary>
        public string customerId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 是否记住
        /// </summary>
        public bool remember { get; set; }
    }
}