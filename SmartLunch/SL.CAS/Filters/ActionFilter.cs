using System.Web;
using System.Web.Mvc;
using SL.Utility;
using SL.Utility.UIConfig;

namespace SL.CAS.Filters
{
    /// <summary>
    /// Action过滤器
    /// </summary>
    public class ActionFilter:ActionFilterAttribute
    {
        /// <summary>
        /// 进入Action之前执行
        /// </summary>
        /// <param name="filterContext">应用程序上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            object session = HttpContext.Current.Session[SysConfiguar.SessionKey];
            if (session == null)
            {
                filterContext.Result = new RedirectResult("/hsmanager/Account/Login");
            }
        }
    }
}