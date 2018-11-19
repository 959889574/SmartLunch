using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalTAppAuthorize
    {

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<EntTAppAuthorize> GetList(TAppAuthorizeSearchModel search);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        List<EntTAppAuthorize> GetList(TAppAuthorizeSearchModel search, ref int dataCount);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Tuple<int, string> GetInsert(EntTAppAuthorize model);


        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="type">1 修改 2 删除</param>
        /// <param name="model"></param>
        /// <returns></returns>
        int GetUpdateOrDelete(int type, EntTAppAuthorize model);


        /// <summary>
        /// 校验手机验证码
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="AuthorizationCode"></param>
        /// <param name="AppCode"></param>
        /// <param name="PicVerifyCode"></param>
        /// <returns></returns>
        Tuple<int, EntTAppAuthorize> CheckValidateCode(string Phone, string AuthorizationCode, string AppCode, string PicVerifyCode);

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="AuthorizationCode"></param>
        /// <param name="AppCode"></param>
        /// <returns></returns>
        Tuple<bool, string> GetValidateCode(string Phone, string AuthorizationCode, string AppCode);
    }
}
