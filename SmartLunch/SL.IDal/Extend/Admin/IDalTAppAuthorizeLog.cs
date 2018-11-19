using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalTAppAuthorizeLog
    {
        List<EntTAppAuthorizeLog> GetList(TAppAuthorizeLogSearchModel search);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        List<EntTAppAuthorizeLog> GetList(TAppAuthorizeLogSearchModel search, ref int dataCount);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int GetInsert(EntTAppAuthorizeLog model);
    }
}
