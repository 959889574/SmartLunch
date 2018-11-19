using System.Reflection;
using DBAccessFactory;

namespace DBFactory
{
    public class DBAccessFactory
    {
        /// <summary>
        /// 创建数据访问层IDAL的实现类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateDALObject<T>()
        {
            string str = new DbConfig().DefaultConnString.ToUpper();
            if (str == "ORACLE")
            {
                return (T)Assembly.Load("DAccessLayer.OracleDal").CreateInstance("DAccessLayer.OracleDal.Dal" + typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 5));
            }
            else
            {
                return (T)Assembly.Load("DAccessLayer.SQLServerDal").CreateInstance("DAccessLayer.SQLServerDal.Dal" + typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 5));
            }
        }
    }
}
