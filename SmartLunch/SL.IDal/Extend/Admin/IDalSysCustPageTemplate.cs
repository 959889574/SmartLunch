using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalSysCustPageTemplate
    {
        /// <summary>
        /// 获取专题页系统模板
        /// </summary>
        /// <param name="Code">模板编号</param>
        /// <returns></returns>
        EntSysCustPageTemplate GetSysCustPageTemplate(string Code);

    }
}
