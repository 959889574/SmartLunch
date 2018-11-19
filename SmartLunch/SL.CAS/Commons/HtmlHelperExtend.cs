using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SL.CAS
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// 给CSS文件或JS文件指定版本号
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">CSS或JS路径</param>
        /// <returns></returns>
        public static string GetCssJsUrl(this HtmlHelper helper, string url)
        {
            string version = ConfigurationManager.AppSettings["JsVersion"];
            version = version == null ? "1.0" : version;
            var rootPath = DocumentPath.TrimEnd('/');
            url = rootPath + url;
            return url +="?v=" + version;
        }

        /// <summary>
        /// 获取站点的根目录
        /// </summary>
        public static string DocumentPath
        {
            get
            {
                string strPath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
                return strPath;
            }
        }
    }
}