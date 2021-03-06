﻿using System;
using System.Data;
using System.IO.Compression;
using System.Text;
using System.Web;

namespace SL.Utility
{
    //class ReportData
    public class XMLReportHelper
    {
        public XMLReportHelper()
        {
        }

        //根据DataSet, 产生提供给报表需要的XML数据，参数ToCompress指定是否压缩数据
        public static void GenDetailData(System.Web.UI.Page DataPage, DataSet myds, bool ToCompress)
        {
            string XMLText = myds.GetXml();
            ResponseXml(DataPage, ref XMLText, ToCompress);
        }

        //根据DataTable, 产生提供给报表需要的XML数据，参数ToCompress指定是否压缩数据
        public static void GenDetailData(System.Web.UI.Page DataPage, DataTable mydt, bool ToCompress)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(mydt);
            GenDetailData(DataPage, ds, ToCompress);
        }

        //根据DataSet, 产生提供给报表需要的XML数据，并同时将ParamterPart中的报表参数数据一起打包，参数ToCompress指定是否压缩数据
        public static void GenEntireData(System.Web.UI.Page DataPage, DataSet myds, ref string ParameterPart, bool ToCompress)
        {
            string RecordsetPart = myds.GetXml();
            string XMLText = "<report>\r\n" + RecordsetPart + ParameterPart + "</report>";
            ResponseXml(DataPage, ref XMLText, ToCompress);
        }

        //根据IDataReader, 产生提供给报表需要的XML数据，其中的空值字段也会产生XML节点，参数ToCompress指定是否压缩数据
        public static void GenNodeXmlDataFromReader(System.Web.UI.Page DataPage, IDataReader dr, bool ToCompress)
        {
            string XMLText = "<xml>\n";
            while (dr.Read())
            {
                XMLText += "<row>";
                for (int i = 0; i < dr.FieldCount; ++i)
                {
                    string FldName = dr.GetName(i);
                    if (FldName == "")
                        FldName = "Fld" + i;
                    XMLText += String.Format("<{0}>{1}</{0}>", FldName, HttpUtility.HtmlEncode(dr.GetValue(i).ToString()));
                }
                XMLText += "</row>\n";
            }
            XMLText += "</xml>\n";

            ResponseXml(DataPage, ref XMLText, ToCompress);
        }

        //根据 IDataReader 产生提供给报表需要的XML参数数据包
        public static void GenParameterData(System.Web.UI.Page DataPage, IDataReader drParamer)
        {
            string XMLText = GenParameterText(drParamer);
            XMLText = "<report>" + XMLText + "</report>";
            ResponseXml(DataPage, ref XMLText, false);
        }

        //将 DataReader 中的数据打包为报表需要的参数数据包形式
        public static string GenParameterText(IDataReader drParamer)
        {
            string ParameterPart = "\r\n<_grparam>\r\n";
            if (drParamer.Read())
            {
                for (int i = 0; i < drParamer.FieldCount; ++i)
                {
                    if (drParamer.IsDBNull(i))
                        continue;

                    if (drParamer.GetFieldType(i).IsArray)
                    {
                        long DataSize = drParamer.GetBytes(i, 0, null, 0, int.MaxValue);
                        byte[] buffer = new byte[DataSize];
                        drParamer.GetBytes(i, 0, buffer, 0, (int)DataSize);
                        ParameterPart += String.Format("<{0}>{1}</{0}>\r\n", drParamer.GetName(i), Convert.ToBase64String(buffer));
                    }
                    else
                    {
                        ParameterPart += String.Format("<{0}>{1}</{0}>\r\n", drParamer.GetName(i), HttpUtility.HtmlEncode(drParamer.GetValue(i).ToString()));
                    }
                }
            }
            ParameterPart += "</_grparam>\r\n";
            return ParameterPart;
        }

        //将报表XML数据文本输出到HTTP请求
        public static void ResponseXml(System.Web.UI.Page DataPage, ref string XMLText, bool ToCompress)
        {
            //报表XML数据的前后不能附加任何其它数据，否则XML数据将不能成功解析，所以调用ClearContent方法清理网页中前面多余的数据
            DataPage.Response.ClearContent();

            if (ToCompress)
            {
                //将string数据转换为byte[]，以便进行压缩
                //System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
                System.Text.UTF8Encoding converter = new System.Text.UTF8Encoding();
                byte[] XmlBytes = converter.GetBytes(XMLText);

                //在 HTTP 头信息中写入报表数据压缩信息
                DataPage.Response.AppendHeader("gr_zip_type", "deflate");                  //指定压缩方法
                DataPage.Response.AppendHeader("gr_zip_size", XmlBytes.Length.ToString()); //指定数据的原始长度
                DataPage.Response.AppendHeader("gr_zip_encode", converter.HeaderName);     //指定数据的编码方式 utf-8 utf-16 ...

                // 把压缩后的xml数据发送给客户端
                DeflateStream compressedzipStream = new DeflateStream(DataPage.Response.OutputStream, CompressionMode.Compress, true);
                compressedzipStream.Write(XmlBytes, 0, XmlBytes.Length);
                compressedzipStream.Close();
            }
            else
            {
                // 把xml对象发送给客户端
                //DataPage.Response.ContentType = "text/xml";
                DataPage.Response.Write(XMLText);
            }

            //报表XML数据的前后不能附加任何其它数据，否则XML数据将不能成功解析，所以调用End方法放弃网页中后面不必要的数据
            DataPage.Response.End();
        }

        ////分批读取报表数据实，从IDataReader中产生一个批次的报表XML数据
        //public static int BatchGenXmlDataFromDataReader(System.Web.UI.Page DataPage, IDataReader dr, int WantRows, bool ToCompress)
        //{
        //    string XMLText = "<xml>\n";
        //    int ReadedRows = 0;
        //    while (dr.Read() && (ReadedRows < WantRows))
        //    {
        //        XMLText += "<row>";
        //        for (int i = 0; i < dr.FieldCount; ++i)
        //        {
        //            string FldName = dr.GetName(i);
        //            if (FldName == "")
        //                FldName = "Fld" + i;

        //            if (dr.GetFieldType(i).IsArray)
        //            {
        //                long DataSize = dr.GetBytes(i, 0, null, 0, int.MaxValue);
        //                byte[] buffer = new byte[DataSize];
        //                dr.GetBytes(i, 0, buffer, 0, (int)DataSize);
        //                XMLText += String.Format("<{0}>{1}</{0}>\r\n", FldName, Convert.ToBase64String(buffer));
        //            }
        //            else
        //            {
        //                XMLText += String.Format("<{0}>{1}</{0}>\r\n", FldName, HttpUtility.HtmlEncode(dr.GetValue(i).ToString()));
        //            }
        //        }
        //        XMLText += "</row>\n";
        //        ++ReadedRows;
        //    }
        //    XMLText += "</xml>\n";

        //    ResponseXml(DataPage, ref XMLText, ToCompress);

        //    return ReadedRows;
        //}

        ////分批读取报表数据实，从DataTable中产生一个批次的报表XML数据，如果是第一批，在头部信息中指定记录数据个数
        //public static int BatchGenXmlDataFromDataTable(System.Web.UI.Page DataPage, DataTable dt, int StartNo, int WantRows, bool ToCompress)
        //{
        //    //如果是第一次取数，在Http头中指定记录数，以便客户端在开始时就产生准确的分页信息
        //    if (StartNo == 0)
        //        DataPage.Response.AppendHeader("gr_batch_total", dt.Rows.Count.ToString());

        //    string XMLText = "<xml>\n";
        //    int ReadedRows = 0;
        //    while ((StartNo < dt.Rows.Count) && (ReadedRows < WantRows))
        //    {
        //        DataRow Row = dt.Rows[StartNo];
        //        XMLText += "<row>";
        //        for (int i = 0; i < dt.Columns.Count; ++i)
        //        {
        //            string FldName = dt.Columns[i].ColumnName;
        //            if (FldName == "")
        //                FldName = "Fld" + i;

        //            if (Row[i].GetType().IsArray)
        //            {
        //                XMLText += String.Format("<{0}>{1}</{0}>\r\n", FldName, Convert.ToBase64String((byte[])Row[i]));
        //            }
        //            else
        //            {
        //                XMLText += String.Format("<{0}>{1}</{0}>\r\n", FldName, HttpUtility.HtmlEncode(Row[i].ToString()));
        //            }
        //        }
        //        XMLText += "</row>\n";
        //        ++ReadedRows;
        //        ++StartNo;
        //    }
        //    XMLText += "</xml>\n";

        //    ResponseXml(DataPage, ref XMLText, ToCompress);

        //    return ReadedRows;
        //}
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //class JSONReportData 产生报表需要的 JSON 格式数据
    public class JSONReportData
    {
        public JSONReportData()
        {
        }

        //根据DataSet, 产生提供给报表需要的XML数据，参数ToCompress指定是否压缩数据
        public static void GenDetailData(System.Web.UI.Page DataPage, DataSet myds, bool ToCompress)
        {
            GenDetailData(DataPage, myds.Tables[0], ToCompress);
        }

        //根据DataTable, 产生提供给报表需要的XML数据，参数ToCompress指定是否压缩数据
        public static void GenDetailData(System.Web.UI.Page DataPage, DataTable dt, bool ToCompress)
        {
            string Out = GenDetailText(dt);
            XMLReportHelper.ResponseXml(DataPage, ref Out, ToCompress);
        }

        //根据DataSet, 产生提供给报表需要的XML数据，并同时将ParamterPart中的报表参数数据一起打包，参数ToCompress指定是否压缩数据
        public static void GenEntireData(System.Web.UI.Page DataPage, DataSet myds, ref string ParameterPart, bool ToCompress)
        {
            string XMLText = GenDetailText(myds.Tables[0]);

            StringBuilder sb = new StringBuilder(XMLText, 0, XMLText.Length - 1, XMLText.Length + ParameterPart.Length + 2); //去掉最后一个“}”
            sb.Append(',');
            sb.Append(ParameterPart);
            sb.Append('}');
            string Out = sb.ToString();
            XMLReportHelper.ResponseXml(DataPage, ref Out, ToCompress);
        }

        //根据 IDataReader 产生提供给报表需要的XML参数数据包
        public static void GenParameterData(System.Web.UI.Page DataPage, IDataReader drParamer)
        {
            string sbJSONText = GenParameterText(drParamer);
            sbJSONText = "{" + sbJSONText + "}";
            XMLReportHelper.ResponseXml(DataPage, ref sbJSONText, false);
        }

        //根据DataTable, 产生提供给报表需要的JSON文本数据
        public static string GenDetailText(DataTable dt)
        {
            StringBuilder sbJSONText = new StringBuilder("{\"recordset\":[\n");
            foreach (DataRow dr in dt.Rows)
            {
                sbJSONText.Append('{');
                //bool IsFirst = true;
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    if (!dr.IsNull(i))
                    {
                        string Value;
                        if (dt.Columns[i].DataType.IsArray)
                        {
                            Value = Convert.ToBase64String((byte[])dr[i]);
                        }
                        else
                        {
                            Value = dr[i].ToString();
                            PrepareValueText(ref Value);
                        }
                        sbJSONText.AppendFormat("\"{0}\":\"{1}\",", dt.Columns[i].ColumnName, Value);
                    }
                }
                sbJSONText.Remove(sbJSONText.Length - 1, 1); //去掉每笔记录最后一个字段后面的","
                sbJSONText.Append("},\n");
            }
            sbJSONText.Remove(sbJSONText.Length - 2, 1); //去掉最后一条记录后面的","
            sbJSONText.Append("]}");

            return sbJSONText.ToString();
        }

        //将 DataReader 中的数据打包为报表需要的参数数据包形式
        public static string GenParameterText(IDataReader drParamer)
        {
            StringBuilder sbJSONText = new StringBuilder();
            if (drParamer.Read())
            {
                for (int i = 0; i < drParamer.FieldCount; ++i)
                {
                    if (drParamer.IsDBNull(i))
                        continue;

                    string Value;
                    if (drParamer.GetFieldType(i).IsArray)
                    {
                        long DataSize = drParamer.GetBytes(i, 0, null, 0, int.MaxValue);
                        byte[] buffer = new byte[DataSize];
                        drParamer.GetBytes(i, 0, buffer, 0, (int)DataSize);
                        Value = Convert.ToBase64String(buffer);
                    }
                    else
                    {
                        Value = drParamer.GetValue(i).ToString();
                        PrepareValueText(ref Value);
                    }

                    sbJSONText.AppendFormat("\"{0}\":\"{1}\",", drParamer.GetName(i), Value);
                }
            }
            sbJSONText.Remove(sbJSONText.Length - 1, 1); //去掉最后一个字段后面的","
            return sbJSONText.ToString();
        }

        //如果数据中包含有JSON规范中的特殊字符(" \ \r \n \t)，多特殊字符加 \ 编码
        public static void PrepareValueText(ref string ValueText)
        {
            bool HasSpecialChar = false;
            foreach (char ch in ValueText)
            {
                if (ch == '"' || ch == '\\' || ch == '\r' || ch == '\n' || ch == '\t')
                {
                    HasSpecialChar = true;
                    break;
                }
            }
            if (HasSpecialChar)
            {
                StringBuilder NewValueText = new StringBuilder();
                foreach (char ch in ValueText)
                {
                    if (ch == '"' || ch == '\\' || ch == '\r' || ch == '\n' || ch == '\t')
                    {
                        NewValueText.Append('\\');
                        if (ch == '"' || ch == '\\')
                            NewValueText.Append(ch);
                        else if (ch == '\r')
                            NewValueText.Append('r');
                        else if (ch == '\n')
                            NewValueText.Append('n');
                        else if (ch == '\t')
                            NewValueText.Append('t');
                    }
                    else
                    {
                        NewValueText.Append(ch);
                    }
                }
                ValueText = NewValueText.ToString();
            }
        }
    }

}