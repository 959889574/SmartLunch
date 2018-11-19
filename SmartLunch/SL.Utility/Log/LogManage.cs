using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Utility
{
    public class LogManage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常</param>
        public static void ErrorLog(string msg, Exception ex)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                if (ex != null)
                {
                    msg += ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                }
                logger.Error(msg);
            }
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常</param>
        /// <param name="param">参数</param>
        public static void ErrorLog(string msg, Exception ex, params ParameterInfo[] param)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                if (ex != null)
                {
                    msg += ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                }
                foreach (var item in param)
                {
                    msg += item.Key + ":" + JsonConvert.SerializeObject(item.Value) + Environment.NewLine;
                }
                logger.Error(msg);
            }
        }

        public static void InfoLog(string msg, params ParameterInfo[] param)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                if (param.Length > 0)
                {
                    msg += Environment.NewLine;
                    foreach (var item in param)
                    {
                        msg += item.Key + ":" + JsonConvert.SerializeObject(item.Value) + Environment.NewLine;
                    }
                }
                logger.Info(msg);
            }
        }

    }

    public class ParameterInfo
    {
        public ParameterInfo() { }
        /// <summary>
        /// 参数键值对
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        public ParameterInfo(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
        /// <summary>
        /// 参数名
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
    }
}
