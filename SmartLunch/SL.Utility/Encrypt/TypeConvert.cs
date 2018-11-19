using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SL.Utility
{

    /// <summary>
    /// 数据类型转换/截取/过滤
    /// </summary>
    public static class TypeConvert
    {
        public static string ToJustJsonString(this object Object)
        {
            if (Object == null)
                return "";

            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string jsonString = JsonConvert.SerializeObject(Object, Formatting.Indented, timeFormat);
            return jsonString;
        }

        public static string ToUpperFirst(this string s)
        {
            return Regex.Replace(s, @"\b[a-zA-Z]\w+", delegate(Match match)
            {
                if (s.IndexOf('_') >= 0)
                {
                    string v = match.ToString();
                    var arr = v.Split('_');
                    StringBuilder sb = new StringBuilder();
                    s = arr.Aggregate(sb, (m, n) => m.Append(char.ToUpper(Convert.ToChar(arr[0])) + n.Substring(1).ToLower())).ToString();
                }
                else
                {
                    s = char.ToUpper(s[0]) + s.Substring(1).ToLower();
                }
                return s;
            });
        }

        /// <summary>
        /// 把数据库在列转化成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DBColToString(this object obj)
        {
            return null == obj || obj is DBNull ? "" : obj.ToString();
        }

        public static bool DBColToBool(this object obj)
        {
            return null != obj && !(obj is DBNull) && obj.ToString() != "0" && obj.ToString() != "false" && obj.ToString() != "N";
        }

        public static string Bool2YN(this bool obj)
        {
            return obj ? "Y" : "N";
        }

        public static string ToStringOfValue(this Enum value)
        {
            return Convert.ToInt32(value).ToString();
        }

        /// <summary>
        /// 数据转化成decimal,转换错误返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal DBColToDecimal(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return 0;
            }
            else
            {
                decimal temp = 0;
                decimal.TryParse(obj.ToString(), out temp);
                return temp;
            }
        }

        public static decimal DBColToDecimalHasPercent(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return 0;
            }
            else
            {
                string val = obj.ToString().Replace("%", string.Empty);
                decimal temp = 0;
                decimal.TryParse(val, out temp);
                return temp/100;
            }
        }

        public static decimal DBColToDecimal(this object obj, int digital)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return 0;
            }
            else
            {
                decimal temp = 0;
                decimal.TryParse(obj.ToString(), out temp);
                return Math.Round(temp, digital);
            }
        }

        public static string DBColToDecimalString3(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return "0.000";
            }
            else
            {
                try
                {
                    return Math.Round(Convert.ToDecimal(obj), 3).ToString();
                }
                catch
                {
                    return "0.000";
                }
            }
        }

        /// <summary>
        /// 保留3位小数后去掉末尾的零，可控制为0返回0或空字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="showZero">true时如果值为0返回0，false时值为0则返回空</param>
        /// <returns></returns>
        public static string DBColToDecimalString(this object obj, bool showZero)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return showZero ? "0" : "";
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(obj).ToString("#0.###");
                }
                catch
                {
                    return showZero ? "0" : "";
                }
            }
        }

        public static string DecimalToString(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return "";
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(obj).ToString("#0.#########");
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 数据转化成int32,转换错误返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int DBColToInt(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return 0;
            }
            else
            {
                int temp = 0;
                int.TryParse(obj.ToString(), out temp);
                return temp;
            }
        }

        public static Int64 DBColToInt64(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return 0;
            }
            else
            {
                Int64 temp = 0;
                Int64.TryParse(obj.ToString(), out temp);
                return temp;
            }
        }

        public static DateTime? DBColToDateTime(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return null;
            }
            else
            {
                DateTime? aa = null;
                DateTime bb;
                if (DateTime.TryParse(obj.ToString(), out bb))
                {
                    aa = bb;
                }
                return aa;
            }
        }

        /// <summary>
        /// Strings to date time.
        /// <para>无值或转换错误时,返回1900-01-01值</para>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>DateTime.</returns>
        ///  Created By Ny6000
        ///  Created Date:2016/6/12 15:28:09
        public static DateTime StringToDateTime(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return DateTime.Parse("1900-01-01");
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(obj);
                }
                catch
                {
                    return DateTime.Parse("1900-01-01");
                }
            }
        }

        public static DateTime DBColToDateTime(this object obj, DateTime argDefaultValue)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return argDefaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(obj);
                }
                catch
                {
                    return argDefaultValue;
                }
            }
        }

        public static string DBColToDateTimeString(this object obj, string formatString)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return "";
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        return Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        return Convert.ToDateTime(obj).ToString(formatString);
                    }
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string DBColToDateTimeString(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0)
            {
                return "";
            }
            else
            {
                try
                {
                    string str = Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
                    if (str.Equals("1900-01-01") || str.Equals("0001-01-01"))
                    {
                        return "";
                    }

                    return str;
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string DBNumToDateString(this object obj)
        {
            if (null == obj || obj is DBNull || obj.ToString().Length == 0 || obj.ToString().Equals("0"))
            {
                return "";
            }
            else
            {
                string str = obj.ToString();
                if (str.Length.Equals(8))
                {
                    return str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6);
                }
                else
                {
                    return str;
                }
            }
        }

        /// <summary>
        /// 计算字符串的长度（按字节计算,不计算前后空格） by mbh
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetBytesLength(this string value)
        {
            int length = 0;
            if (!string.IsNullOrEmpty(value) && value.Trim().Length > 0)
            {
                value = value.Trim();
                Regex r = new Regex(@"[\u4E00-\u9FFF]");

                char[] stringChar = value.ToCharArray();
                foreach (char chr in stringChar)
                {
                    Console.Write(chr.ToString());
                    if (r.IsMatch(chr.ToString()))
                    {
                        length += 2;
                    }
                    else
                    {
                        length += 1;
                    }
                }
            }

            return length;
        }

        /// <summary>
        /// 判断是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsDecimal(this string str)
        {
            decimal temp = 0;
            return decimal.TryParse(str, out temp);
        }

        public static object ToDBNull(this string obj)
        {
            if (string.IsNullOrEmpty(obj.Trim()))
            {
                return DBNull.Value;
            }
            else
            {
                return obj;
            }
        }

        public static object ToDBNull(this DateTime? obj)
        {
            if (null == obj)
            {
                return DBNull.Value;
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Gets the distinct table.
        /// <para>根据数据列名,获得列记录不重复的数据集合</para>
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="Field">The field.</param>
        /// <returns>DataTable.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/1/23 11:13:48
        public static DataTable GetDistinctTable(this DataTable dt, string Field)
        {
            DataTable newDt = dt.Copy();
            ArrayList indexList = new ArrayList();
            for (int i = 0; i < newDt.Rows.Count - 1; i++)
            {
                if (!indexList.Contains(i))
                {
                    for (int j = i + 1; j < newDt.Rows.Count; j++)
                    {
                        if (newDt.Rows[i][Field].ToString() == newDt.Rows[j][Field].ToString())
                        {
                            indexList.Add(j);
                        }
                    }
                }
            }

            indexList.Sort();

            for (int i = indexList.Count - 1; i >= 0; i--)
            {
                int index = Convert.ToInt32(indexList[i]);
                newDt.Rows.RemoveAt(index);
            }

            return newDt;
        }

        /// <summary>
        /// 金额转为大写金额
        /// </summary>
        /// <param name="LowerMoney"></param>
        /// <returns></returns>
        public static string MoneyToChinese(string LowerMoney)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;

            // 保留两位小数 123.489→123.49　　123.4→123.4
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "元";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零元", "亿元");
            strUpper = strUpper.Replace("亿零万零元", "亿元");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零元", "万元");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零元", "元");
            strUpper = strUpper.Replace("零零", "零");

            // 对壹圆以下的金额的处理
            if (strUpper.Substring(0, 1) == "元")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零元整";
            }
            functionReturnValue = strUpper;

            if (IsNegative)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }

        /// <summary>
        /// Gets the short value.
        /// <para>字符串截取</para>
        /// </summary>
        /// <param name="argInputText">The argument input text.</param>
        /// <param name="argStartIndex">Start index of the argument.</param>
        /// <param name="argShowLength">Length of the argument show.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/1/23 11:13:17
        public static string GetSubString(string argInputText, int argStartIndex, int argShowLength)
        {
            if (string.IsNullOrEmpty(argInputText))
            {
                return "";
            }

            if (argStartIndex.Equals(0) && argInputText.Length < argShowLength)
            {
                return argInputText;
            }

            if (!argStartIndex.Equals(0))
            {
                if (argInputText.Length >= argStartIndex && argInputText.Length <= argStartIndex + argShowLength)
                {
                    return argInputText.Substring(argStartIndex);
                }
                else if (argInputText.Length < argStartIndex)
                {
                    return "";
                }
            }

            return argInputText.Substring(argStartIndex, argShowLength);
        }


        /// <summary>
        /// Gets the SQL para.
        /// <para>特殊字符替换，防止ｓｑｌ注入方式之一</para>
        /// </summary>
        /// <param name="argInputText">The argument input text.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2015/3/20 16:02:50
        public static string GetSqlParaValue(string argInputText)
        {
            if (string.IsNullOrEmpty(argInputText))
            {
                return argInputText;
            }

            string tmp = "";
            tmp = argInputText;
            tmp = tmp.Replace("'", "''");
            tmp = Regex.Replace(tmp, " or ", "_or_", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, " drop ", "_drop_", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, " delete ", "_delete_", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, " insert ", "_insert_", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, " and ", "_and_", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, "exec ", "exec_", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, "sp_", "sp__", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, "asp__net", "asp_net", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, "xp_", "xp__", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, @"chr\(", "chr_(", RegexOptions.IgnoreCase);
            tmp = Regex.Replace(tmp, @"char\(", "char_(", RegexOptions.IgnoreCase);
            return (tmp);
        }

    }

}