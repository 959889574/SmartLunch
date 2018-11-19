
using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalTSysAppVersionRecord
    {


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
         int GetInsert(EntTSysAppVersionRecord model);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
         int GetUpdate(EntTSysAppVersionRecord model);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="FlowNo"></param>
        /// <param name="Release"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
         int GetUpdate(string FlowNo, int Release, string UserName);
        /// <summary>
        /// 获取单个实体类
        /// </summary>
        /// <param name="FlowNo"></param>
        /// <returns></returns>
         int GetDelete(string FlowNo);

        /// <summary>
        /// 查询单个实体对象
        /// </summary>
        /// <param name="FlowNo"></param>
        /// <returns></returns>
         EntTSysAppVersionRecord GetSelectData(string FlowNo);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
         List<EntTSysAppVersionRecord> GetList(TSysAppVersionRecordSearchModel search, ref int dataCount);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
         List<EntTSysAppVersionRecord> GetList(TSysAppVersionRecordSearchModel search);
        /// <summary>
        /// 获取单个model
        /// </summary>
        /// <param name="AppVersion"></param>
        /// <param name="AppCode"></param>
        /// <param name="ClientType"></param>
        /// <returns></returns>
         EntTSysAppVersionRecord GetSelectModel(string AppVersion, string AppCode, string ClientType);
    }
}
