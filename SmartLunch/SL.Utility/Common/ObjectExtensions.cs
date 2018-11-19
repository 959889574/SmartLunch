using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SL.Utility
{
    public static class ObjectExtensions
    {
        #region Private Fields
        private static Regex _invalidXmlChars = new Regex(@"[^\u0009\u000a\u000d\u0020-\uD7FF\uE000-\uFFFD]",
                        RegexOptions.Compiled);
        #endregion

        /// <summary>
        /// removes any unusual unicode characters that can't be encoded into XML
        /// </summary>
        public static string RemoveInvalidXmlChars(this string source)
        {
            if (string.IsNullOrEmpty(source)) return "";

            return _invalidXmlChars.Replace(source, "");
        }

       

        public static string SqlInjectReplace(this string sqlPlan)
        {
            if (string.IsNullOrWhiteSpace(sqlPlan))
            {
                return "";
            }

            return sqlPlan.Replace("'", "''");
        }

        public static DateTime ConvertJsTimeStampToDateTime(this long JsTimeStamp)
        {
            // 获取当前时区时间
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return startTime.AddMilliseconds(JsTimeStamp);
        }

        public static bool ConvertBoolean(this object cellValue, bool defaultValue = false)
        {
            if (cellValue == null || cellValue == DBNull.Value)
            {
                return defaultValue;
            }

            if (cellValue is bool)
            {
                return (bool)cellValue;
            }

            if (cellValue is int)
            {
                return (int)cellValue >= 1;
            }

            if (cellValue is string)
            {
                int oInt;
                if (int.TryParse("" + cellValue, out oInt))
                {
                    return oInt >= 1;
                }
            }

            bool oBoolean;
            if (bool.TryParse("" + cellValue, out oBoolean))
            {
                return oBoolean;
            }

            return defaultValue;
        }

        public static Int32 ConvertInt32(this object cellValue, int defaultValue = 0)
        {
            if (cellValue == null || cellValue == DBNull.Value)
            {
                return defaultValue;
            }

            if (cellValue is int)
            {
                return (int)cellValue;
            }

            if (cellValue is byte)
            {
                return (byte)cellValue;
            }
            if (cellValue is Int16)
            {
                return (Int16)cellValue;
            }

            int oInt;
            if (int.TryParse(cellValue.ToString(), out oInt))
            {
                return oInt;
            }

            return defaultValue;
        }

        public static Int64 ConvertInt64(this object cellValue, Int64 defaultValue = 0)
        {
            if (cellValue == null || cellValue == DBNull.Value)
            {
                return defaultValue;
            }
            if (cellValue is Int64)
            {
                return (Int64)cellValue;
            }
            if (cellValue is int)
            {
                return (int)cellValue;
            }
            if (cellValue is Int16)
            {
                return (Int16)cellValue;
            }
            if (cellValue is byte)
            {
                return (byte)cellValue;
            }

            Int64 oInt;
            if (Int64.TryParse(cellValue.ToString(), out oInt))
            {
                return oInt;
            }

            return defaultValue;
        }

        public static decimal ConvertDecimal(this object cellValue, decimal defaultValue = 0)
        {
            if (cellValue == null || cellValue.Equals(DBNull.Value))
            {
                return defaultValue;
            }

            if (cellValue is decimal)
            {
                return (decimal)cellValue;
            }

            decimal result;
            if (decimal.TryParse(cellValue.ToString(), out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static string ConvertString(this object cellValue, string defaultValue = "")
        {
            if (cellValue == null || cellValue.Equals(DBNull.Value))
            {
                return defaultValue;
            }
            return cellValue.ToString().Trim();
        }

        public static string ConvertTimestampInt64String(this object cellValue, string defaultValue = "-1")
        {
            if (cellValue == null || cellValue.Equals(DBNull.Value))
            {
                return defaultValue;
            }

            var stamp = cellValue as byte[];
            if (stamp != null)
            {
                var value = Convert.ToInt64(BitConverter.ToString(stamp, 0).Replace("-", ""), 16);
                return value.ToString();
            }

            return Convert.ToInt64(cellValue).ToString();
        }

        public static Int64 ConvertTimestampInt64(this object cellValue, Int64 defaultValue = -1)
        {
            if (cellValue == null || cellValue.Equals(DBNull.Value))
            {
                return defaultValue;
            }

            var stamp = cellValue as byte[];
            if (stamp != null)
            {
                var value = Convert.ToInt64(BitConverter.ToString(stamp, 0).Replace("-", ""), 16);
                return value;
            }

            return Convert.ToInt64(cellValue);
        }

       

        public static string ReverseString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            StringBuilder sb = new StringBuilder(input.Length);
            for (int i = input.Length - 1; i >= 0; i--)
            {
                sb.Append(input[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T">自动义属性Attribute扩展类</typeparam>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object cellValue) where T : Attribute, new()
        {
            T obj = default(T);
            string value = cellValue.ToString();
            FieldInfo field = cellValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(T), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return new T();
            foreach (var item in objs)
            {
                if (item is T)
                {
                    obj = (T)item;
                    break;
                }
            }
            return obj;
        }
    }
}
