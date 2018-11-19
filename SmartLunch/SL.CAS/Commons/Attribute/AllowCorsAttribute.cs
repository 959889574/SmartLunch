using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SL.CAS
{
    /// <summary>
    /// 跨域访问
    /// </summary>
    public class AllowCorsAttribute : ActionFilterAttribute
    {
        private string[] _domains;
        public AllowCorsAttribute()
        {
            //如果[AllowCors]特性标签没有带域名，那么就从配置文件中读取允许访问的域名，然后将它赋给_domains
            _domains = string.IsNullOrEmpty(ConfigurationManager.AppSettings["AllowCors"]) ? new string[] { } : ConfigurationManager.AppSettings["AllowCors"].Split('|');
        }
        /// <summary>
        /// 优先读取参数允许域名
        /// </summary>
        /// <param name="domain"></param>
        public AllowCorsAttribute(params string[] domain)
        {
            _domains = domain;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlReferrer = filterContext.HttpContext.Request.UrlReferrer;
            if (urlReferrer != null)
            {
                var absolutePath = urlReferrer.OriginalString;
                var absolutePathFormat = absolutePath.Substring(0, absolutePath.Length - 1);
                //不填为所有网站都允许跨域
                if (_domains.Length <= 0)
                {
                    filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                }
                else if (_domains.Contains(absolutePathFormat))
                {
                    filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                }
            }
            else
            {
                //如果urlReferrer为空，我理解为自己本地访问(亲自测试，本地访问urlReferrer为null)
                filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            }
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "GET, HEAD, OPTIONS, POST, PUT");
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
            base.OnActionExecuting(filterContext);
        }
    }
}