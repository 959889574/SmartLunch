using System.Web.Mvc;
using NLog;

namespace SL.CAS.Filters
{
    /// <summary>
    /// 异常处理类
    /// </summary>
    public class ExceptionsFilter : HandleErrorAttribute
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 重写异常处理方法
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            _log.Error(context.Exception);
            base.OnException(context);
        }
    }
}