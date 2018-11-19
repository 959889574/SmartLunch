using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalTSysApp
    {
        /// <summary>
        /// 获取app列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        List<EntTSysApp> GetList(TSysAppSearchModel search, ref int dataCount);
        /// <summary>
        /// 获取app列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<EntTSysApp> GetList(TSysAppSearchModel search);

        /// <summary>
        /// 新增app信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int GetInsert(EntTSysApp model);
        /// <summary>
        /// 修改app信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int GetUpdate(EntTSysApp model);
        /// <summary>
        /// 修改app状态
        /// </summary>
        /// <param name="type">1 停用启用 2删除</param>
        /// <param name="AppCode"></param>
        /// <param name="Status">0 false 1 true</param>
        /// <param name="ModifyEmp"></param>
        /// <returns></returns>
        int UpdateStatus(int type, string AppCode, int Status, string ModifyEmp);
    }
}
