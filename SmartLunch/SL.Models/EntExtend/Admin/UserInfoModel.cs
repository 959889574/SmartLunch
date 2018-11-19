using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Models
{
    public class UserInfoModel
    {
        public UserInfoModel() { }
        public UserInfoModel(string UserName, string pwd)
        {
            this.UserName = UserName;
            this.UserPassword = pwd;
            this.LoginDate = DateTime.Now;
        }
        private string _UserName = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private string _UserPaw = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string UserPassword
        {
            get { return _UserPaw; }
            set { _UserPaw = value; }
        }

        private DateTime _LoginDate = DateTime.Now;
        /// <summary>
        /// 登入时间
        /// </summary>
        public DateTime LoginDate
        {
            get { return _LoginDate; }
            set { _LoginDate = value; }
        }


    }
}
