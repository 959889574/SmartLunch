using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace SL.Utility
{
    /// <summary>
    /// 手机短消息发送
    /// </summary>
    public class SMSSend
    {
        public string accountID = "";
        public string passord = "";
        public string sign = "";
        //public string subcode = "";
        public string serverUrl = "";

        public string logUrl="";
        public SMSSend()
        {
            accountID = ConfigurationManager.AppSettings["SmsAccountID"] + "";
            passord = ConfigurationManager.AppSettings["SmsPassword"] + "";
            sign = ConfigurationManager.AppSettings["SmsSign"] + "";
            // subcode = ConfigurationManager.AppSettings["subcode"] + "";
            serverUrl = ConfigurationManager.AppSettings["SmsHostAddress"] + "";
        }



        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        //MD5加密程序（32位小写）
        private static string md5(string str)
        {
            byte[] result = Encoding.Default.GetBytes(str);
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            String md = BitConverter.ToString(output).Replace("-", "");
            return md.ToLower();
        }


        /// <summary>
        /// http 发送信息
        /// </summary>
        /// <param name="phones">The phones.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool SendSmsHttp(string phones, string message)
        {
            if (message.Length >= 350)
            {
                return false;
            }
            else if (message.Length < 1)
            {
                return false;
            }

            try
            {
                var path = logUrl.Substring(0, logUrl.LastIndexOf("\\"));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (StreamWriter sw = new StreamWriter(logUrl, true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("---phone----------------------------------------");
                    sw.WriteLine(phones + "--" + DateTime.Now.ToString());
                    sw.WriteLine("----message---------------------------------------");
                    sw.WriteLine(message);
                    sw.WriteLine("--------------------------------------------------");
                }
            }
            catch
            {
            }

            //string url = serverUrl;
            string pResult= Post(serverUrl, GetSendString(phones, message), logUrl);

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(pResult);
                XmlNode xmlNode = xmlDoc.SelectSingleNode("response/result");
                if (xmlNode != null && xmlNode.InnerText.Trim().Equals("0"))
                {
                    return true;
                }
            }
            catch (Exception eee)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(logUrl, true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("---eee.Message----------------------------------------");
                        sw.WriteLine(eee.Message);
                        sw.WriteLine("----eee.Source---------------------------------------");
                        sw.WriteLine(eee.Source);
                        sw.WriteLine("---eee.StackTrace----------------------------------------");
                        sw.WriteLine(eee.StackTrace);
                        sw.WriteLine("---eee.InnerException---------------------------------------");
                        sw.WriteLine(eee.InnerException);
                        sw.WriteLine("-------------------------------------------");
                    }
                }
                catch
                {
                }

            }

            return false;
        }

        /// <summary>
        /// HTTP 获取上行
        /// </summary>
        /// <returns></returns>
        //public string GetDeliverByHttp() {
        //    String Account = userName;
        //    String Password = md5(passord);
        //    String url = serverUrl + "/http/sms/Deliver";
        //    //String data = "Account=" + Account + "&Password=" + Password;
        //    String message = GetBalanceString();
        //   return Post(url, message);
        //}


        /// <summary>
        /// 查询余额 by HTTP.
        /// </summary>
        /// <returns></returns>
        //public string GetBalanceByHttp() {
        //    String Account = userName;
        //    String Password = md5(passord);
        //    String url = serverUrl + "/http/sms/Balance";
        //    //String data = "Account=" + Account + "&Password=" + Password;
        //    String message = GetBalanceString();
        //    return Post(url, message);
        //}


        /// <summary>
        /// 获取状态报告
        /// </summary>
        /// <returns></returns>
        //public string GetReportByHttp(string phone, string smsid)
        //{
        //    String Account = userName;
        //    String Password = md5(passord);
        //    String url = serverUrl + "/http/sms/Report";
        //    //String data = "Account=" + Account + "&Password=" + Password;
        //    String message = GetReport(phone, smsid);
        //    return Post(url, message);
        //}

        //向服务器发送POST数据
        private static string Post(string URL, string message, string logUrl)
        {
            WebClient webClient = new WebClient();
            NameValueCollection postValues = new NameValueCollection();
            postValues.Add("message", message);

            try
            {
                //向服务器发送POST数据
                byte[] responseArray = webClient.UploadValues(URL, postValues);
                return Encoding.UTF8.GetString(responseArray);
            }
            catch (Exception eee)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(logUrl, true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("---eee.Message----------------------------------------");
                        sw.WriteLine(eee.Message);
                        sw.WriteLine("----eee.Source---------------------------------------");
                        sw.WriteLine(eee.Source);
                        sw.WriteLine("---eee.StackTrace----------------------------------------");
                        sw.WriteLine(eee.StackTrace);
                        sw.WriteLine("---eee.InnerException---------------------------------------");
                        sw.WriteLine(eee.InnerException);
                        sw.WriteLine("-------------------------------------------");
                    }
                }
                catch
                {
                }
            }

            return string.Empty;
        }


        /// <summary>
        ///获取发送短信 字符串
        /// </summary>
        /// <param name="phones">电话号码列表</param>
        /// <param name="message">内容</param>
        /// <returns></returns>
        private string GetSendString(string phones, string message)
        {
            StringBuilder sMessage = new StringBuilder();
            sMessage.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sMessage.Append("<message>");
            //用户名
            sMessage.Append("<account>");
            sMessage.Append(accountID);
            sMessage.Append("</account>");
            //密码
            sMessage.Append("<password>");
            sMessage.Append(md5(passord));
            sMessage.Append("</password>");
            //添加收短信号码
            sMessage.Append("<msgid>");
            sMessage.Append("");
            sMessage.Append("</msgid>");

            //添加收短信号码
            sMessage.Append("<phones>");
            sMessage.Append(phones);
            sMessage.Append("</phones>");
            //短信内容
            sMessage.Append("<content>");
            sMessage.Append(message);
            sMessage.Append("</content>");
            //短信签名，服务器端告知 不可修改
            sMessage.Append("<sign>");
            sMessage.Append(sign);
            sMessage.Append("</sign>");
            //扩展子号码 可为空
            sMessage.Append("<subcode>");
            sMessage.Append("");
            sMessage.Append("</subcode>");
            //发送时间可为空
            sMessage.Append("<sendtime>");
            sMessage.Append("");
            sMessage.Append("</sendtime>");
            sMessage.Append("</message>");
            return sMessage.ToString();
        }

        /// <summary>
        ///获取查询余额字符串
        /// </summary>
        /// <returns></returns>
        //private string GetBalanceString() {
        //    StringBuilder sMessage = new StringBuilder();
        //    sMessage.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        //    sMessage.Append("<message>");
        //    //用户名
        //    sMessage.Append("<account>");
        //    sMessage.Append(userName);
        //    sMessage.Append("</account>");
        //    //密码
        //    sMessage.Append("<password>");
        //    sMessage.Append(md5(passord));
        //    sMessage.Append("</password>");
        //    sMessage.Append("</message>");
        //    return sMessage.ToString();
        //}


        /// <summary>
        /// 获取短信状态报告 字符串
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <returns></returns>
        //private string GetReport(string phone,string smsid) {
        //    StringBuilder sMessage = new StringBuilder();
        //    sMessage.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        //    sMessage.Append("<message>");
        //    //用户名
        //    sMessage.Append("<account>");
        //    sMessage.Append(userName);
        //    sMessage.Append("</account>");
        //    //密码
        //    sMessage.Append("<password>");
        //    sMessage.Append(md5(passord));
        //    sMessage.Append("</password>");
        //    //添加收短信号码
        //    sMessage.Append("<msgid>");
        //    sMessage.Append(smsid);
        //    sMessage.Append("</msgid>");

        //    //添加收短信号码
        //    sMessage.Append("<phones>");
        //    sMessage.Append(phone);
        //    sMessage.Append("</phones>");
        //    sMessage.Append("</message>");
        //    return sMessage.ToString();
        //}

    }
}
