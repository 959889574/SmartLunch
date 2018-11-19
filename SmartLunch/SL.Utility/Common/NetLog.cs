using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Utility
{
    public static class NetLog
    {
        /// <summary>  
        /// 写入日志到txt文件文件中  
        /// </summary>  
        /// <param name="action">动作</param>  
        /// <param name="strMessage">日志内容</param>  
        /// <param name="time">时间</param>  
        public static void WriteTextLog(object strMessage)
        {
            DateTime time = DateTime.Now;
            string fileFullPath = GetPath();
            StringBuilder str = new StringBuilder();
            str.Append(time.ToString() + ":" + JsonConvert.SerializeObject(strMessage) + "\r\n");

            StreamWriter sw;
            if (!File.Exists(fileFullPath))
            {
                sw = File.CreateText(fileFullPath);
            }
            else
            {
                sw = File.AppendText(fileFullPath);
            }
            sw.WriteLine(str.ToString());
            sw.Close();
        }

        public static string GetPath(bool isNeedCreatePath = true)
        {
            DateTime time = DateTime.Now;

            string path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug", "") + @"Log\";//去掉bin\Debug

            if (isNeedCreatePath && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileFullPath = path + time.ToString("yyyyMMdd") + ".txt";

            return fileFullPath;
        }
    }
}
