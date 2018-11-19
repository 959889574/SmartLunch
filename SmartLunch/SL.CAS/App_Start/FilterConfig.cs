using System.Web.Mvc;
using SL.CAS.Filters;

namespace SL.CAS
{
    /// <summary>
    /// filters
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 全局filter配置
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionsFilter());
        }
    }
}
