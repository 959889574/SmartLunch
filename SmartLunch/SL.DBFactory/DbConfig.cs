using System.Configuration;

namespace DBAccessFactory
{
    /// <summary>
    /// 默认数据库设置
    /// Author:YGZ
    /// 日期：2017年1月22日
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 默认数据库链接
        /// </summary>
        public string DefaultConnString { get { return ConfigurationManager.ConnectionStrings["DefaultConnString"].ConnectionString; } }

        /// <summary>
        /// 默认数据库类型
        /// </summary>
        public string DbType { get { return ConfigurationManager.AppSettings["DefaultConnString"]; } }

    }
}