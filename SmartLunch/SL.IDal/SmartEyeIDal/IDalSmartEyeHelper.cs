using SL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.IDal
{
    public interface IDalSmartEyeHelper
    {
       
        // 获取接口返回值
        SmartEyeModel<LoginInfoModel> GetLoginInfo(string url, string data);
        //保存商品类目
        int SaveType(string url); 
        //保存商品属性
        int SaveCategory(string url, string CustomerId, string OwnerId);
        /// 获取商品1688浏览列表
        List<GoodsShow1688Model> GetList(Goods1688SearchModel search, ref int dataCount);
        /// 获取商品1688浏览列表
        List<GoodsShow1688Model> GetList(Goods1688SearchModel search);
        /// 获取商品浏览列表
        List<GoodsShowModel> GetGoodsList(GoodsSearchModel search, ref int dataCount);
        /// 获取商品浏览列表
        List<GoodsShowModel> GetGoodsList(GoodsSearchModel search);
        //获取3000+属性存入数据库
        int SaveAttribute(string url, string CustomerId);
        //初始化类目
        List<InitCategoryModel> GetInitCategoryList();
        //初始1688账号
        List<Init1688Model> GetInit1688List(string CustomerId);
        //初始化统计
        List<InitCountModel> GetNumData(string CustomerId);
        int SelSysType(string CustomerId);
        //作废
        int UpdateStatus(string RecogniseID, string CustomerId);
       
    }
}
