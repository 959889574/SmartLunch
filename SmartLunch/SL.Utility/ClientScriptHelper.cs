using System;
using System.Text;
using System.Web.UI;

namespace SL.Utility
{
    /// <summary>
    /// 客户端脚本
    /// </summary>
    public class ClientScriptHelper
    {
        Page _page = null;
        public ClientScriptHelper(Page page)
        {
            this._page = page;
        }

        #region [ RegisterClientScriptBlock ]

        /// <summary>
        /// 使用键和脚本文本向 Page 对象注册客户端脚本
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="script">脚本</param>
        public void RegisterClientScriptBlock(string key, string script)
        {
            this.RegisterClientScriptBlock(typeof(ClientScriptHelper), key, script, false);
        }

        /// <summary>
        /// 使用键和脚本文本向 Page 对象注册客户端脚本
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="script">脚本</param>
        /// <param name="addScriptTags">指示是否添加脚本标记的布尔值</param>
        public void RegisterClientScriptBlock(string key, string script, bool addScriptTags)
        {
            this.RegisterClientScriptBlock(typeof(ClientScriptHelper), key, script, addScriptTags);
        }

        /// <summary>
        /// 使用类型、键和脚本文本向 Page 对象注册客户端脚本
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="key">键</param>
        /// <param name="script">脚本</param>
        public void RegisterClientScriptBlock(Type type, string key, string script)
        {
            this.RegisterClientScriptBlock(type, key, script, false);
        }

        /// <summary>
        /// 使用类型、键和脚本文本向 Page 对象注册客户端脚本
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="key">键</param>
        /// <param name="script">脚本</param>
        /// <param name="addScriptTags">指示是否添加脚本标记的布尔值</param>
        public void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTags)
        {
            if (_page.ClientScript.IsClientScriptBlockRegistered(type, key) == false)
            {
                _page.ClientScript.RegisterClientScriptBlock(type, key, script, addScriptTags);
            }
        }

        #endregion

        #region [ AlertMessage ]

        /// <summary>
        ///在客户端显示一条信息
        /// </summary>
        /// <param name="message">显示的信息</param>
        public void AlertMessage(string message)
        {
            this.RegisterClientScriptBlock("AlertMessage", "<script language='javascript'>alert(\"" + message.Replace("'", "′") + "\");</script>");
        }

        public void AlertMessage()
        {
            AlertMessage("保存成功！");
        }
        /// <summary>
        /// 在客户端显示一条提示信息
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string message)
        {
            _page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('" + message + "');</script>");
        }
        /// <summary>
        /// 在客户端显示一条信息，并提示错误发生的域 
        /// </summary>
        /// <param name="field">错误发生的域</param>
        /// <param name="message">信息</param>
        //public void AlertMessage(string field, string message)
        //{
        //    this.RegisterClientScriptBlock("AlertMessage_" + field, "<script language='javascript'>alert(\"" + message.Replace("'", "′") + "\");displayErrField('" + field + "');</script>");
        //}

        #endregion

        /// <summary>
        /// 再客户端显示一条信息，然后转到一个页面
        /// </summary>
        /// <param name="message">显示的信息</param>
        /// <param name="url">转到页面路径</param>
        public void AlertRedirect(string message, string url)
        {
            this.RegisterClientScriptBlock("AlertRedirect", "<script language='javascript'>alert(\"" + message.Replace("'", "′") + "\");window.navigate('" + url + "');</script>");
        }

        /// <summary>
        /// 转到指定的页面
        /// </summary>
        /// <param name="url">转到页面路径</param>
        public void Redirect(string url)
        {
            this.RegisterClientScriptBlock("Redirect", "<script language='javascript'>window.navigate('" + url + "');</script>");
        }

        /// <summary>
        /// 再客户端显示一条信息，然后关闭页面
        /// </summary>
        /// <param name="message"></param>
        public void AlertClose(string message)
        {
            this.RegisterClientScriptBlock("AlertClose", "<script language='javascript'>alert(\"" + message.Replace("'", "′") + "\");window.close();</script>");
        }

        /// <summary>
        /// 在客户端显示一条信息，然后返回上一个页面
        /// </summary>
        /// <param name="message"></param>
        public void AlertReflashFrame(string message, string frame)
        {
            AlertReflashFrame(message, frame, false);
        }

        /// <summary>
        /// 在客户端显示一条信息，然后返回上一个页面
        /// </summary>
        /// <param name="message"></param>
        public void AlertReflashFrame(string message, string frame, bool isOpener)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language='javascript'>");
            sb.Append("alert('" + message.Replace("'", "′") + "');");
            if (isOpener)
            {
                sb.Append("window.opener.parent.frames['" + frame + "'].location.reload();");
                sb.Append("if(typeof(window.opener) == 'undefined') { window.location.href=window.location.href;} else { window.opener.location.reload();}");
                sb.Append("window.close();");
            }
            else
            {
                sb.Append("window.parent.frames['" + frame + "'].location.reload();");
            }
            sb.Append("</script>");

            this.RegisterClientScriptBlock("AlertReflashFrame_" + frame, sb.ToString());
        }

        /// <summary>
        ///  在客户端显示一条信息，然后把页面刷新一遍
        /// </summary>
        /// <param name="message"></param>
        public void AlertReLoad(string message)
        {
            this.RegisterClientScriptBlock("AlertReLoad", "<script language='javascript'>alert('"
                + message.Replace("'", "′") + "');window.location.href=window.location.href;</script>");
        }

        /// <summary>
        /// 刷新打开这个页面的父页面
        /// </summary>
        /// <param name="message"></param>
        public void AlertReflashOpener(string message)
        {
            this.RegisterClientScriptBlock("AlertReflashOpener", "<script language='javascript'>alert('"
                + message.Replace("'", "′") + "');if(typeof(window.opener) == 'undefined') { window.location.href=window.location.href;} else { window.opener.location.reload();window.close();}</script>");
        }

        public void AlertBack(string message)
        {
            this.RegisterClientScriptBlock("AlertBack", "<script language='javascript'>alert('" + message.Replace("'", "′") + "');history.back();</script>");
        }

        public void ReflashFrame(string strFrames)
        {
            this.RegisterClientScriptBlock("ReflashFrame", "<script language='javascript'>" + strFrames + ".location.reload();</script>");
        }

        public void AlertConfirm(string message)
        {
            this.RegisterClientScriptBlock("AlertConfirm", "<script language='javascript'> if(confirm('" + message + "')) return true; else return false;</script>");
        }

        /// <summary>
        /// 使页面上所有元素处于禁用状态
        /// </summary>
        public void DisableAllElement()
        {
            this.RegisterClientScriptBlock("DisableAllElement", "<script language='javascript'>DisableAllElement();</script>");
        }

        public void DisableAllInputElement()
        {
            this.RegisterClientScriptBlock("DisableAllInputElement", "<script language='javascript'>DisableAllInputElement();</script>");
        }

        public void HiddenClientElement(string name)
        {
            this.RegisterClientScriptBlock("HiddenClientElement_" + name, "<script language='javascript'>HiddenByName('" + name + "');</script>");
        }

        public void DisableClientElement(string name)
        {
            this.RegisterClientScriptBlock("DisableClientElement_" + name, "<script language='javascript'>Form1." + name + ".disabled=true ;</script>");
        }

        //public void ShowDetailPanel(string partpath )
        //{
        //    string path = ConfigurationManager.AppSettings["basePath"];
        //    string fullPath = string.Format("{0}/{1}", path,partpath);
        //    string javascript = "window.open('" + fullPath + "','', 'captionbar=no,toolbar=no,scrollbars=no,width=920,height=800', '')";
        //    _page.ClientScript.RegisterClientScriptBlock(_page.GetType(),_page.GetType().FullName,javascript,true);       

        //}

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            this.RegisterClientScriptBlock("CloseWindow", "<script language='javascript'>window.close();</script>");
        }

        public string PageTitle
        {
            set
            {
                //string strSystemName = System.Configuration.ConfigurationManager.AppSettings["SystemName"];
                //this.RegisterClientScriptBlock("pageTitle", "<script language='javascript'>if(window.parent!=null)window.parent.document.title=\"" + strSystemName + "--当前用户-" + value + "\";</script>");
            }
        }
    }
}
