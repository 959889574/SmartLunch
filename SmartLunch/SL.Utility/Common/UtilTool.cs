using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Utility
{
    public class UtilTool
    {
        /// <summary>
        /// DataRow转model
        /// </summary>
        /// <param name="row"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Object ToModel(DataRow row, Object model)
        {
            if (row != null)
            {
                try
                {
                    Type type = model.GetType(); //获取类型
                    foreach (var item in type.GetProperties())
                    {
                        if (row.Table.Columns.Contains(item.Name) && (row[item.Name] != null || row[item.Name] != DBNull.Value))
                        {
                            item.SetValue(model, row[item.Name]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return model;
        }

        /// <summary>
        /// 替换html中的特殊字符
        /// </summary>
        /// <param name="theString">需要进行替换的文本。</param>
        /// <returns>替换完的文本。</returns>
        public static string HtmlEncode(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
            {
                return "";
            }
            theString = theString.Replace(">", "&gt;");
            theString = theString.Replace("<", "&lt;");
            theString = theString.Replace("  ", " &nbsp;");
            theString = theString.Replace("\"", "&quot;");
            theString = theString.Replace("'", "&#39;");
            theString = theString.Replace("\r\n", "<br/>");
            theString = theString.Replace("—", "&mdash;");
            theString = theString.Replace("--", "&#45;&#45;");  //Biny 添加于 2016年9月18日
            //theString = theString.Replace("&", "&amp;"); //hem.li 20161020
            return theString;
        }

        /// <summary>
        /// 恢复html中的特殊字符
        /// </summary>
        /// <param name="theString">需要恢复的文本。</param>
        /// <returns>恢复好的文本。</returns>
        public static string HtmlDecode(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
            {
                return "";
            }
            theString = theString.Replace("&gt;", ">");
            theString = theString.Replace("&lt;", "<");
            theString = theString.Replace("&nbsp;", "  ");
            theString = theString.Replace("&quot;", "\"");
            theString = theString.Replace("&#39;", "'");
            theString = theString.Replace("<br/>", "\r\n");
            theString = theString.Replace("&mdash;", "—");//2012-05-07新加的
            theString = theString.Replace("&#45;&#45;", "--");  //Biny 添加于 2016年9月18日
            theString = theString.Replace("&amp;", "&"); //hem.li 20161020
            return theString;
        }

    }
}
