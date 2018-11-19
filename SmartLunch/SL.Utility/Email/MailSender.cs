using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SL.Utility
{
    /// <summary>
    ///  发送邮件 by mbh
    /// </summary>
    public class MailSender
    {
        //private static SmtpConfig smtpConfig;

        public static void Send(string server, string sender, string recipient, string subject, string body, bool isBodyHtml, Encoding encoding, bool isAuthentication, params string[] files)
        {
            SmtpClient smtpClient = new SmtpClient(server);
            MailMessage message = new MailMessage(sender, recipient);
            message.IsBodyHtml = isBodyHtml;

            message.SubjectEncoding = encoding;
            message.BodyEncoding = encoding;

            message.Subject = subject;
            message.Body = body;

            message.Attachments.Clear();
            if (files != null && files.Length != 0)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    Attachment attach = new Attachment(files[i]);
                    message.Attachments.Add(attach);
                }
            }

            if (isAuthentication == true)
            {
                var configSetting =  SmtpConfig.Create().SmtpSetting;

                smtpClient.Credentials = new NetworkCredential(configSetting.User, configSetting.Password);
            }
            smtpClient.Send(message);


        }
        /// <summary>
        /// 发送html邮件，并且把附加嵌入html中 by mbh
        /// </summary>
        /// <param name="server"></param>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="encoding"></param>
        /// <param name="isAuthentication"></param>
        /// <param name="fileInline"></param>
        /// <param name="files"></param>
        public static void Send(string server, string sender, string recipient, string subject, string body, bool isBodyHtml, Encoding encoding, bool isAuthentication, bool fileInline, params string[] files)
        {
            SmtpClient smtpClient = new SmtpClient(server);
            MailMessage message = new MailMessage(sender, recipient);
            message.IsBodyHtml = isBodyHtml;

            message.SubjectEncoding = encoding;
            message.BodyEncoding = encoding;

            message.Subject = subject;

            message.Attachments.Clear();
            if (files != null && files.Length != 0)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    Attachment attach = new Attachment(files[i]);
                    if (fileInline)
                    {
                        //attach.ContentId = attach.ContentId;
                        attach.ContentDisposition.Inline = true;
                        attach.NameEncoding = Encoding.UTF8;
                    }
                    message.Attachments.Add(attach);

                    body = body.Replace("BGImgInline" + i, attach.ContentId);
                }
            }
            message.Body = body;

            if (isAuthentication == true)
            {
                var configSetting =  SmtpConfig.Create().SmtpSetting;

                smtpClient.Credentials = new NetworkCredential(configSetting.User, configSetting.Password);
            }
            smtpClient.Send(message);


        }

        /// <summary>
        /// 发送html邮件，并且把附件嵌入html中 by mbh
        /// </summary>
        /// <param name="recipient">接受邮件地址</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="fileInline">是否内联</param>
        /// <param name="files">文件数组</param>
        public static void SendHtmlMail(string recipient, string subject, string body, bool fileInline, string[] files)
        {
            var configSetting =  SmtpConfig.Create().SmtpSetting;
            Send(configSetting.Server, configSetting.Sender, recipient, subject, body, true, Encoding.Default, true, fileInline, files);
        }

        public static void Send(string recipient, string subject, string body)
        {
            var configSetting =  SmtpConfig.Create().SmtpSetting;
            Send(configSetting.Server, configSetting.Sender, recipient, subject, body, true, Encoding.Default, true, null);
        }

        public static void Send(string Recipient, string Sender, string Subject, string Body)
        {
            var configSetting =  SmtpConfig.Create().SmtpSetting;
            Send(configSetting.Server, Sender, Recipient, Subject, Body, true, Encoding.UTF8, true, null);
        }

        //static readonly string smtpServer = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
        //static readonly string userName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
        //static readonly string pwd = System.Configuration.ConfigurationManager.AppSettings["Pwd"];
        //static readonly int smtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
        //static readonly string authorName = System.Configuration.ConfigurationManager.AppSettings["AuthorName"];
        //static readonly string to = System.Configuration.ConfigurationManager.AppSettings["To"];


        public void Send(string subject, string body)
        {

            //List<string> toList = StringPlus.GetSubStringList(StringPlus.ToDBC(to), ',');
            //OpenSmtp.Mail.Smtp smtp = new OpenSmtp.Mail.Smtp(smtpServer, userName, pwd, smtpPort);
            //foreach (string s in toList)
            //{
            //    OpenSmtp.Mail.MailMessage msg = new OpenSmtp.Mail.MailMessage();
            //    msg.From = new OpenSmtp.Mail.EmailAddress(userName, authorName);

            //    msg.AddRecipient(s, OpenSmtp.Mail.AddressType.To);

            //    //设置邮件正文,并指定格式为 html 格式
            //    msg.HtmlBody = body;
            //    //设置邮件标题
            //    msg.Subject = subject;
            //    //指定邮件正文的编码
            //    msg.Charset = "gb2312";
            //    //发送邮件
            //    smtp.SendMail(msg);
            //}
        }
    }

    public class SmtpSetting
    {
        private string _server;

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        private bool _authentication;

        public bool Authentication
        {
            get { return _authentication; }
            set { _authentication = value; }
        }
        private string _user;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        private string _sender;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }

    public class SmtpConfig
    {
        private static SmtpConfig _smtpConfig;
        public SmtpSetting SmtpSetting
        {
            get
            {
                SmtpSetting smtpSetting = new SmtpSetting();
                smtpSetting.Server = ConfigurationManager.AppSettings["Server"] + "";
                smtpSetting.Authentication = Convert.ToBoolean(ConfigurationManager.AppSettings["Authentication"] + "");
                smtpSetting.User = ConfigurationManager.AppSettings["User"] + "";
                smtpSetting.Password = ConfigurationManager.AppSettings["Password"] + "";
                smtpSetting.Sender = ConfigurationManager.AppSettings["Sender"] + "";

                return smtpSetting;
            }
        }
        private SmtpConfig()
        {

        }
        public static SmtpConfig Create()
        {
            if (_smtpConfig == null)
            {
                _smtpConfig = new SmtpConfig();
            }
            return _smtpConfig;
        }
    }
}
