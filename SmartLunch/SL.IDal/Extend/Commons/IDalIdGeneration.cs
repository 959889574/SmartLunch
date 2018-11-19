using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalIdGeneration
    {

        /// <summary>
        /// 生成id集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="len">长度</param>
        /// <param name="cId">顾客ID</param>
        /// <param name="uId">创建人</param>
        /// <param name="sum">数量</param>
        /// <returns></returns>
         List<string> getIdGeneration(EnIdGeneration type, int len, string uId, string cId, int sum);


        /// <summary>
        /// 生成id
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="len">长度</param>
        /// <param name="cId">顾客ID</param>
        /// <param name="uId">创建人</param>
        /// <returns></returns>
         string getIdGeneration(EnIdGeneration type, int len, string uId, string cId);
    }
}
