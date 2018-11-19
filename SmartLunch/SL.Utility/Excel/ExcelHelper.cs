using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NLog;

namespace SL.Utility
{

    /// <summary>
    /// 本版本功能：支持Excel2003、Excel2007的导入导出
    /// Create By setyg
    /// 2015年1月29日10:49:35
    /// </summary>
    public static class ExcelHelper
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #region 导出 Excel

        /// <summary>
        /// 生成Excel文件，保存的文件名以.xls结尾，则生成2003版文件，以.xlsx结尾生成2007版文件
        /// </summary>
        /// <param name="path">文件保存路径</param>
        /// <param name="dtList">生成sheet对应的数据Grid</param>
        /// <param name="strSheet">sheet名</param>
        /// <param name="fileColumnsName">Excel文件列名</param>
        /// <param name="dtColumnsName">DataTable列名</param>
        public static void ExportXlsExcel(string path, DataTable dtList, string strSheet, string[] fileColumnsName, string[] dtColumnsName)
        {
            string extension = Path.GetExtension(path);
            IWorkbook workBook = extension.ToLower() == ".xls" ? (IWorkbook)new HSSFWorkbook() : new XSSFWorkbook();
            CreateSheet(workBook, strSheet, dtList, fileColumnsName, dtColumnsName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                workBook.Write(file); //创建Excel文件。
                file.Close();
            }
        }

        /// <summary>
        /// 生成SHEET
        /// </summary>
        /// <param name="workBook">EXCEL文件</param>
        /// <param name="sheetName">EXCEL选项卡名称</param>
        /// <param name="dt">需要显示的DT源</param>
        /// <param name="fileColumnsName">Excel中显示的列名</param>
        /// <param name="dtColumnsName">DataTable中显示的列名</param>
        /// <returns></returns>
        private static void CreateSheet(IWorkbook workBook, string sheetName, DataTable dt, string[] fileColumnsName, string[] dtColumnsName)
        {
            ISheet sheet = workBook.CreateSheet(sheetName);

            //创建EXCEL的标题
            IRow Row = sheet.CreateRow(0);
            for (int iColumnIndex = 0; iColumnIndex < fileColumnsName.Length; iColumnIndex++)
            {
                Row.CreateCell(iColumnIndex).SetCellValue(fileColumnsName[iColumnIndex]);
            }

            var cellRows = 0;
            for (int iRowIndex = 0; iRowIndex < dt.Rows.Count; iRowIndex++)
            {
                Row = sheet.CreateRow(iRowIndex + 1);
                for (int iColumnIndex = 0; iColumnIndex < dtColumnsName.Length; iColumnIndex++)
                {
                    Row.CreateCell(iColumnIndex).SetCellValue(dt.Rows[iRowIndex][dtColumnsName[iColumnIndex]].ToString());

                    cellRows = GetCellRowCount(dt.Rows[iRowIndex][dtColumnsName[iColumnIndex]].ToString());
                    if (cellRows > 1)
                    {
                        //将目前字段的CellStyle设定为自动换行
                        var cs = workBook.CreateCellStyle();
                        cs.WrapText = true;
                        Row.GetCell(iColumnIndex).CellStyle = cs;

                        //因为换行所以自动设设置相应倍数的行高度
                        Row.HeightInPoints = cellRows * sheet.DefaultRowHeight / 20;
                    }
                }
            }
        }

        private static int GetCellRowCount(string argCellText)
        {
            if (!argCellText.Contains("\n"))
            {
                return 1;
            }

            var arrCount = argCellText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return arrCount.Length;
        }

        #endregion

        #region 导出 Csv

        /// <summary>
        /// Export the data from datatable to CSV file
        /// </summary>
        /// <param name="grid"></param>
        public static void ExportDataToCSV(DataTable dt, string path, string[] fileColumnsName, string[] dtColumnsName)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            string data = "";
            for (int i = 0; i < fileColumnsName.Length; i++)
            {
                data += fileColumnsName[i];
                if (i < fileColumnsName.Length - 1)
                {
                    data += ",";
                }
            }

            sw.WriteLine(data);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dtColumnsName.Length; j++)
                {
                    data += dt.Rows[i][dtColumnsName[j]].ToString();
                    if (j < dtColumnsName.Length - 1)
                    {
                        data += ",";
                    }
                }

                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            OpenFileByName(path);
        }

        public static void OpenFileByName(string argFilePath)
        {
            // 打开下载文件
            //定义一个ProcessStartInfo实例
            ProcessStartInfo info = new ProcessStartInfo();

            //设置启动进程的应用程序或文档名
            info.FileName = argFilePath;

            //设置启动进程的参数
            info.Arguments = "";

            //启动由包含进程启动信息的进程资源
            try
            {
                Process.Start(info);
            }
            catch (Win32Exception we)
            {
                throw new Exception(we.Message);
            }
        }

        #endregion

        #region 文件导入及校验规则

        /// <summary>
        /// 导入文件的方法
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="xmlPath">XML配置文件的路径</param>
        /// <param name="itemName">导入模块的名称</param>
        /// <param name="splitflag">导入文件的分隔标志'|' 或 ','</param>
        /// <param name="strError">返回的错误信息</param>
        public static DataTable GetDataFromExcelOrCsv(string filePath, string xmlPath, string itemName, char splitflag, ref string strError)
        {
            DataTable dt = new DataTable();
            try
            {
                FileInfo fi = new FileInfo(filePath);
                string strExtension = fi.Extension;
                if (strExtension.ToLower().EndsWith("csv"))
                {
                    #region Csv

                    dt = GetTableColumn(xmlPath, itemName); //初始化dt列字段
                    IList list;
                    StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("GBK"));
                    int j = 0;
                    try
                    {
                        ExcelHelperXml excelHelperXml = new ExcelHelperXml();

                        //string strHasHead = excelHelperXml.GetXmlValue("HasHead");   //判断是否从第一行开始读取数据
                        string strHasHead = new ExcelHelperXml().GetXmlValue(itemName).ToLower(); //判断是否从第一行开始读取数据
                        if (strHasHead.ToUpper().Equals("TRUE"))
                        {
                            sr.ReadLine();
                        }

                        string noEmpty = string.Empty;
                        list = excelHelperXml.ReadXml(itemName, xmlPath, ref noEmpty);
                        string[] noList = noEmpty.Split('|');
                        while (!sr.EndOfStream)
                        {
                            j = j + 1;

                            DataRow dr = dt.NewRow();
                            string[] strContent = sr.ReadLine().Split(splitflag);

                            for (int i = 0; i < list.Count; i++)
                            {
                                string value = "";
                                ExcelHelperItem item = list[i] as ExcelHelperItem;
                                if (item.Seq == -1)
                                    value = item.Default_Value;
                                else
                                {
                                    if (item.Seq < strContent.Length)
                                    {
                                        value = strContent[item.Seq].Trim();
                                        string strTmp = ValidateData(value, item);
                                        if (strTmp != "")
                                        {
                                            strError += "第" + j + "行数据 " + strTmp + "\n";
                                        }
                                        if (string.IsNullOrEmpty(strContent[item.Seq].Trim()) && item.Default_Value != "")
                                            value = item.Default_Value;
                                    }
                                    else
                                    {
                                        value = item.Default_Value;
                                    }
                                }
                                dr[item.FieldName] = value;
                            }

                            dt.Rows.Add(dr);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sr.Close();
                        sr.Dispose();
                    }

                    #endregion
                }
                else if (strExtension.ToLower().EndsWith("xls"))
                    dt = GetXlsSheetData(filePath, itemName, xmlPath, ref strError);
                else if (strExtension.ToLower().EndsWith("xlsx"))
                    dt = GetXlsxSheetData(filePath, itemName, xmlPath, ref strError);
                else
                    strError += "不支持所选格式,无法导入。";

                return dt;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用") > -1)
                {
                    strError = "请关闭导入文件后重试";
                }
                else
                {
                    throw ex;
                }

                return null;
            }
        }

        /// <summary>
        /// Create by Setyg
        /// 导入文件为Excel2003调用
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="itemName">配置文件项名</param>
        /// <param name="configFile"></param>
        /// <param name="strError">错误</param>
        /// <returns></returns>
        public static DataTable GetXlsSheetData(string filePath, string itemName, string configFile, ref string strError)
        {
            DataTable dt = null;
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ExcelHelperXml ExcelHelperXml = new ExcelHelperXml(configFile);
            string hasHead = ExcelHelperXml.GetXmlValue(itemName).ToLower(); //判断是否从第一行开始读取数据
            dt = GetTableColumn(configFile, itemName);
            int columnsCount = dt.Columns.Count;
            if (columnsCount <= 0)
            {
                strError = "系统无配置项[" + itemName + "],导出操作失败";
                return dt;
            }

            string noEmpty = string.Empty;
            IList list = ExcelHelperXml.ReadXml(itemName, configFile, ref noEmpty);
            string[] noList = noEmpty.Split('|');
            int[] intList = new int[noList.Length];
            for (int j = 0; j < intList.Length; j++)
            {
                intList[j] = Convert.ToInt32(noList[j]);
            }

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            int rowFlag = 0;
            int rowNum = 0;
            StringBuilder sbValidate = new StringBuilder();
            while (rows.MoveNext())
            {
                rowNum++;
                HSSFRow row = (HSSFRow)rows.Current;
                if (hasHead == "true")
                {
                    if (rowFlag == 0)
                    {
                        rowFlag++;
                        continue;
                    }
                }

                DataRow dr = dt.NewRow();
                if (intList.Any(t => row.GetCell(t - 1) == null || string.IsNullOrEmpty(row.GetCell(t - 1).ToString())))
                {
                    strError += sbValidate.ToString();
                    return dt;
                }

                for (int j = 0; j < columnsCount; j++)
                {
                    string cellValue = string.Empty;
                    ExcelHelperItem item = list[j] as ExcelHelperItem;
                    ICell cell = row.GetCell(j);
                    string errorMessage = string.Empty;
                    if (item.Seq == -1)
                        cellValue = item.Default_Value;
                    else
                    {
                        if (cell != null)
                        {
                            cellValue = cell.ToString();
                        }
                        errorMessage = ValidateData(cellValue, item);
                        if (!string.IsNullOrEmpty(errorMessage))
                            sbValidate.AppendFormat("第{0}行数据 {1}\n", rowNum - 1, errorMessage);
                    }
                    dr[item.FieldName] = cellValue;
                }

                dt.Rows.Add(dr);
            }

            strError += sbValidate.ToString();
            return dt;
        }

        /// <summary>   
        /// Create by Setyg
        /// 导入文件为Excel2007调用
        /// 2015年1月29日12:27:27
        /// </summary>   
        /// <param name="filePath">上传文件路径</param>
        /// <param name="itemName">配置项名称</param>
        /// <param name="configFile">配置文件路径</param>
        /// <param name="strError">返回的错误信息</param>
        /// <returns></returns>   
        public static DataTable GetXlsxSheetData(string filePath, string itemName, string configFile, ref string strError)
        {
            DataTable dt = new DataTable();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);
                ExcelHelperXml ExcelHelperXml = new ExcelHelperXml();
                string hasHead = ExcelHelperXml.GetXmlValue(itemName).ToLower(); //判断是否从第一行开始读取数据
                dt = GetTableColumn(configFile, itemName);
                int columnsCount = dt.Columns.Count;
                if (columnsCount <= 0)
                {
                    strError = "系统无配置项[" + itemName + "],导出操作失败";
                    return dt;
                }

                string noEmpty = string.Empty;
                IList list = ExcelHelperXml.ReadXml(itemName, configFile, ref noEmpty);
                string[] noList = noEmpty.Split('|');
                ISheet sheet = xssfworkbook.GetSheetAt(0);
                int readRow = 0;
                if (hasHead == "true")
                    readRow = 1;

                int[] stopCells = new int[noList.Length];
                for (int i = 0; i < noList.Length; i++)
                    stopCells[i] = Convert.ToInt32(noList[i]);

                StringBuilder sbValidate = new StringBuilder();
                for (int i = readRow; i <= sheet.LastRowNum; i++)
                {
                    IRow nextRow = sheet.GetRow(i);
                    if (nextRow == null || stopCells.Any(t => nextRow.GetCell(t - 1) == null || string.IsNullOrEmpty(nextRow.GetCell(t - 1).ToString())))
                    {
                        strError += sbValidate.ToString();
                        return dt;
                    }

                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string cellValue = "";
                        ExcelHelperItem item = list[j] as ExcelHelperItem;
                        ICell cell = nextRow.GetCell(j);
                        if (item.Seq == -1)
                            cellValue = item.Default_Value;
                        else
                        {
                            cellValue = cell == null ? cellValue : cell.ToString().Trim();
                            var errorMsg = ValidateData(cellValue, item);
                            if (!string.IsNullOrEmpty(errorMsg))
                                sbValidate.AppendFormat("第{0}行数据 {1}\n", i - 1, errorMsg);
                        }
                        dr[item.FieldName] = cellValue;
                    }

                    dt.Rows.Add(dr);
                }

                strError += sbValidate.ToString();
            }

            return dt;
        }

        //校验数据
        public static string ValidateData(string value, ExcelHelperItem item)
        {
            if (item != null)
            {
                if (string.IsNullOrEmpty(value.Replace("　", " ").Trim()))
                {
                    if (item.Empty)
                        return string.Format("{0}不允许为空！", item.Memo);
                    else
                        return "";
                }

                switch (item.Type.ToUpper())
                {
                    case "STRING":
                        if (value.GetBytesLength() > Convert.ToInt32(item.Length))
                            return string.Format("{0}的长度不能超过{1}！", item.Memo, item.Length);

                        break;
                    case "INT":
                        int temp;
                        if (!Int32.TryParse(value, out temp))
                            return string.Format("{0}不是合法数字！", item.Memo);

                        break;
                    case "DOUBLE":
                        double tempDecimal;
                        if (!double.TryParse(value, out tempDecimal))
                            return string.Format("{0}不是合法数字！", item.Memo);
                        else
                        {
                            string[] str = item.Length.Split('.');
                            int configIntLength = Convert.ToInt32(str[0]);
                            int configDecLength = Convert.ToInt32(str[1]);
                            int pointIndex = value.IndexOf('.');
                            if (pointIndex != -1)
                            {
                                int intLength = value.Trim().Substring(0, pointIndex).Length;
                                int decLength = value.Trim().Substring(pointIndex + 1).Length;
                                if (intLength > configIntLength)
                                    return string.Format("{0}整数的长度不能超过{1}位！", item.Memo, configIntLength);
                                if (decLength > configDecLength)
                                    return string.Format("{0}小数的长度不能超过{1}位！", item.Memo, configDecLength);
                            }
                        }

                        break;
                    case "DATE":
                        DateTime dtTime;
                        if (!DateTime.TryParse(value, out dtTime))
                            return string.Format("{0}不是合法日期格式！", item.Memo);

                        break;
                }
            }

            return "";
        }

        /// <summary>
        /// 获取配置文件，取的列名
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static DataTable GetTableColumn(string xmlpath, string itemName)
        {
            XmlDocument doc = new XmlDocument();
            DataTable dt = new DataTable();
            doc.Load(xmlpath);
            XmlNodeList xmlNodeList = doc.SelectNodes("descendant::Items/Item[@Name='" + itemName + "']");
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                XmlNode rootNode = xmlNodeList[0];
                dt.TableName = rootNode.Attributes["tablename"].Value;
                for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                {
                    XmlNode tempNode = rootNode.ChildNodes[i];
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = tempNode.Attributes["Name"].Value;
                    dc.Caption = tempNode.Attributes["memo"].Value;
                    dt.Columns.Add(dc);
                }

                dt.AcceptChanges();
                return dt;
            }

            return dt;
        }

        /// <summary>
        /// Create by Setyg
        /// 导入文件为Excel2003或者2007调用
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="itemName">配置文件项名</param>
        /// <param name="xmlpath"></param>
        /// <param name="strError">错误</param>
        /// <returns></returns>
        public static DataTable GetXlsSheetFromDynamicColumns(string filePath, string xmlpath, string itemName, ref string strError)
        {
            DataTable dtTemp = null;
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string extension = Path.GetExtension(filePath);
                if (extension == ".xls")
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                else
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }

                dtTemp = GetTableColumn(xmlpath, itemName);
                int columnsCount = dtTemp.Columns.Count;
                if (columnsCount <= 0)
                {
                    strError = "系统无配置项[" + itemName + "],导出操作失败";
                    return dtTemp;
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);
                int dtTempColumnsCount = dtTemp.Columns.Count;
                string[] columns = new string[dtTempColumnsCount];
                for (int i = 0; i < dtTempColumnsCount; i++)
                {
                    columns[i] = dtTemp.Columns[i].ToString().Trim().ToLower();
                }

                IRow rowHead = sheet.GetRow(0);

                DataTable dtReturn = new DataTable();
                if (rowHead == null)
                {
                    strError = "导入文件无数据，请重新选择";
                    return null;
                }
                else
                {
                    for (int i = 0; i < rowHead.LastCellNum; i++)
                    {
                        ICell cell = rowHead.GetCell(i);
                        string column = cell.ToString().Trim().ToLower();
                        DataColumn dc = new DataColumn(column);
                        dtReturn.Columns.Add(dc);
                    }
                }

                StringBuilder sbValidate = new StringBuilder();
                string noEmpty = string.Empty;
                IList list = new ExcelHelperXml().ReadXml(itemName, xmlpath, ref noEmpty);
                string[] emptyCellsReturnList = noEmpty.Split('|');
                for (int i = 1; i < sheet.LastRowNum + 1; i++)
                {
                    IRow nextRow = sheet.GetRow(i);
                    DataRow dr = dtTemp.NewRow();
                    for (int j = 0; j < dtReturn.Columns.Count; j++)
                    {
                        if (dtTemp.Columns.Contains(dtReturn.Columns[j].ColumnName))
                        {
                            string cellValue = "";
                            ICell cell = nextRow.GetCell(j);
                            string errorMsg = "";
                            int count = 0;
                            for (int k = 0; k < dtTemp.Columns.Count; k++)
                            {
                                if (dtTemp.Columns[k].ColumnName.ToLower() == dtReturn.Columns[j].ColumnName)
                                    count = k;
                            }

                            ExcelHelperItem item = list[count] as ExcelHelperItem;
                            if (item.Seq == -1)
                                cellValue = item.Default_Value;
                            else
                            {
                                if (cell != null)
                                {
                                    cellValue = cell.ToString().Trim();
                                    if (cellValue.Length > 0)
                                    {
                                        errorMsg = ValidateData(cellValue, item);
                                        if (!string.IsNullOrEmpty(errorMsg))
                                            sbValidate.AppendFormat("第{0}行数据 {1}\n", i - 1, errorMsg);
                                    }
                                }
                            }
                            dr[item.FieldName] = cellValue;
                        }
                    }
                    foreach (string t1 in emptyCellsReturnList)
                    {
                        string[] innerList = t1.Split('-');
                        if (innerList.Length > 1)
                        {
                            bool back = innerList.All(t => dr[t].ToString().Length == 0);
                            if (back)
                                return dtTemp;
                        }
                        else
                        {
                            //为空立即终止循环退出
                            if (string.IsNullOrEmpty(dr[t1].ToString()))
                                return dtTemp;
                        }
                    }

                    dtTemp.Rows.Add(dr);
                }
            }

            return dtTemp;
        }

        #endregion

        #region 文件下载

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="filePath">文件物理路径</param>
        /// <param name="saveFileName">文件保存名称</param>
        public static void DownLoadFile(string filePath, string saveFileName)
        {
            Stream iStream = null;
            byte[] buffer = new Byte[10000];
            int length;
            long dataToRead;
            string filepath = filePath;
            try
            {
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                dataToRead = iStream.Length;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(saveFileName, UTF8Encoding.UTF8));
                while (dataToRead > 0)
                {
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
            }
        }

        #endregion

        /// <summary>
        /// 导出多个SHEET
        /// </summary>
        /// <param name="path"></param>
        /// <param name="strSheet">多个,隔开</param>
        /// <param name="dtList"></param>
        public static void ExportToExcels(string path, string strSheet, params DataTable[] dtList)
        {
            string extension = Path.GetExtension(path).ToLower();
            IWorkbook workBook = extension == ".xls" ? (IWorkbook)new HSSFWorkbook() : new XSSFWorkbook();

            string[] s = strSheet.Split(',');
            if (s.Length > 0)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    CreateSheetByDt(workBook, s[i], dtList[i]);
                }
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                workBook.Write(file); //创建Excel文件。
                file.Close();
            }
        }

        /// <summary>
        /// 导出多sheet的Excel
        /// </summary>
        /// <param name="path">文件保存的路径</param>
        /// <param name="dtList">DataTable列表</param>
        /// <param name="strSheet">多个sheet以逗号分隔</param>
        /// <param name="fileColumnsNames">Sheet列名</param>
        /// <param name="dtColumnsNames">DataTable列名</param>
        public static void ExportDtToExcel(string path, List<DataTable> dtList, string strSheet, List<string[]> fileColumnsNames, List<string[]> dtColumnsNames)
        {
            string extension = Path.GetExtension(path).ToLower();
            IWorkbook workBook = extension == ".xls" ? (IWorkbook)new HSSFWorkbook() : new XSSFWorkbook();
            string[] sheetName = strSheet.Split(',');
            for (int i = 0; i < sheetName.Length; i++)
            {
                CreateSheet(workBook, sheetName[i], dtList[i], fileColumnsNames[i], dtColumnsNames[i]);
            }

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Write))
            {
                workBook.Write(file); //创建Excel文件。
                file.Close();
            }
        }

        /// <summary>
        /// 将DT生成为EXCEL的SHEET
        /// </summary>
        /// <param name="workBook">EXCEL文件</param>
        /// <param name="sheetName">EXCEL选项卡名称</param>
        /// <param name="dt">需要显示的DT源</param>
        /// <returns></returns>
        private static void CreateSheetByDt(IWorkbook workBook, string sheetName, DataTable dt)
        {
            ISheet sheet = workBook.CreateSheet(sheetName);

            //创建EXCEL的标题
            IRow Row = sheet.CreateRow(0);
            for (int iColumnIndex = 0; iColumnIndex < dt.Columns.Count; iColumnIndex++)
            {
                Row.CreateCell(iColumnIndex).SetCellValue(dt.Columns[iColumnIndex].ColumnName);
            }

            for (int iRowIndex = 0; iRowIndex < dt.Rows.Count; iRowIndex++)
            {
                Row = sheet.CreateRow(iRowIndex + 1);
                for (int iColumnIndex = 0; iColumnIndex < dt.Columns.Count; iColumnIndex++)
                {
                    Row.CreateCell(iColumnIndex).SetCellValue(dt.Rows[iRowIndex][iColumnIndex].ToString());
                }
            }
        }

        #region 读取excel中多个sheet到dataset

        /// <summary>
        /// 读取多个sheet到dataset
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xmlPath"></param>
        /// <param name="itemNames">多个ItemName用逗号隔开</param>
        /// <param name="splitflag"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static DataSet GetDataFromExcel(string filePath, string xmlPath, string itemNames, ref string strError)
        {
            DataSet ds = new DataSet();
            try
            {
                FileInfo fi = new FileInfo(filePath);
                string strExtension = fi.Extension;
                if (strExtension.ToLower().EndsWith("xls"))
                {
                    ds = GetXlsSheetDatas(filePath, itemNames, xmlPath, ref strError);
                }
                else if (strExtension.ToLower().EndsWith("xlsx"))
                {
                    ds = GetXlsxSheetDatas(filePath, itemNames, xmlPath, ref strError);
                }
                else
                {
                    strError += "不支持所选格式,无法导入。";
                }
                return ds;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用") > -1)
                {
                    strError = "请关闭导入文件后重试";
                }
                else
                {
                    Log.Error(ex);
                    strError = ex.Message;
                }

                return null;
            }
        }

        /// <summary>
        /// 根据配置的多个itemsName，从excel2003中多个sheet的获取dataset
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="itemNames">多个ItemName用逗号隔开</param>
        /// <param name="configFile"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static DataSet GetXlsSheetDatas(string filePath, string itemNames, string configFile, ref string strError)
        {
            DataSet ds = new DataSet();
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ExcelHelperXml ExcelHelperXml = new ExcelHelperXml();

            //string hasHead = ExcelHelperXml.GetXmlValue("HasHead").ToLower(); //判断是否从第一行开始读取数据
            string[] itemNameArr = itemNames.Split(',');
            int sheetNumber = 0;
            var errorInfo = "";
            foreach (string itemName in itemNameArr)
            {
                DataTable dt = GetTableColumn(configFile, itemName);
                ds.Tables.Add(dt);
                int columnsCount = dt.Columns.Count;
                if (columnsCount <= 0)
                {
                    strError = "系统无配置项[" + itemName + "],导出操作失败";
                    return ds;
                }
                dt.Columns.Add("ErrorInfo");

                string noEmpty = string.Empty;
                IList list = ExcelHelperXml.ReadXml(itemName, configFile, ref noEmpty);
                string[] noList = noEmpty.Split('|');
                int[] intList = new int[noList.Length];
                for (int j = 0; j < intList.Length; j++)
                {
                    intList[j] = Convert.ToInt32(noList[j]);
                }

                ISheet sheet = hssfworkbook.GetSheetAt(sheetNumber);
                IEnumerator rows = sheet.GetRowEnumerator();
                int rowFlag = 0;
                int rowNum = 0;
                int rowTotal = sheet.LastRowNum;

                string hasHead = ExcelHelperXml.GetXmlValue(itemName).ToLower(); //判断是否从第一行开始读取数据
                while (rows.MoveNext())
                {
                    bool finishSheet = false;
                    rowNum++;
                    HSSFRow row = (HSSFRow)rows.Current;
                    if (hasHead == "true")
                    {
                        if (rowFlag == 0)
                        {
                            rowFlag++;
                            continue;
                        }
                    }
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < intList.Length; k++)
                    {
                        if (row.GetCell(intList[k] - 1) == null || string.IsNullOrEmpty(row.GetCell(intList[k] - 1).ToString()))
                        {
                            finishSheet = true;
                            break;
                        }
                    }

                    if (finishSheet)
                    {
                        break;
                    }

                    errorInfo = "";
                    for (int j = 0; j < columnsCount; j++)
                    {
                        string value = "";
                        ExcelHelperItem item = list[j] as ExcelHelperItem;
                        ICell cell = row.GetCell(j);
                        string sResult = "";
                        if (item.Seq == -1)
                            value = item.Default_Value;
                        else
                        {
                            if (cell != null)
                            {
                                if (item.Type == "date")
                                {
                                    try
                                    {
                                        value = cell.DateCellValue + "";
                                    }
                                    catch
                                    {
                                        value = cell.ToString();
                                    }
                                }
                                else
                                {
                                    value = cell.ToString();
                                }
                            }
                        }
                        sResult = ValidateData(value, item);
                        if (!string.IsNullOrEmpty(sResult))
                        {
                            errorInfo += "第" + (sheetNumber + 1) + "表格第" + (rowNum - 1) + "行数据 " + sResult + "\n";
                        }
                        dr[item.FieldName] = value;
                    }
                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        strError += errorInfo;
                        dr["ErrorInfo"] = errorInfo;
                    }
                    dt.Rows.Add(dr);

                }

                //ds.Tables.Add(dt);
                sheetNumber += 1;
            }

            return ds;
        }

        /// <summary>
        /// 根据配置的多个itemsName，从excel2007中多个sheet的获取dataset
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="itemNames">多个ItemName用逗号隔开</param>
        /// <param name="configFile"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static DataSet GetXlsxSheetDatas(string filePath, string itemNames, string configFile, ref string strError)
        {
            DataSet ds = new DataSet();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);
                ExcelHelperXml ExcelHelperXml = new ExcelHelperXml();

                //string hasHead = ExcelHelperXml.GetXmlValue("HasHead").ToLower(); //判断是否从第一行开始读取数据
                string[] itemNameArr = itemNames.Split(',');
                int sheetNumber = 0;
                var errorInfo = "";
                foreach (string itemName in itemNameArr)
                {
                    DataTable dt = GetTableColumn(configFile, itemName);
                    ds.Tables.Add(dt);
                    int columnsCount = dt.Columns.Count;
                    if (columnsCount <= 0)
                    {
                        strError = "系统无配置项[" + itemName + "],导出操作失败";
                        return ds;
                    }
                    dt.Columns.Add("ErrorInfo");

                    string noEmpty = string.Empty;
                    IList list = ExcelHelperXml.ReadXml(itemName, configFile, ref noEmpty);
                    string[] noList = noEmpty.Split('|');
                    int[] intList = new int[noList.Length];
                    for (int j = 0; j < intList.Length; j++)
                    {
                        intList[j] = Convert.ToInt32(noList[j]);
                    }

                    ISheet sheet = xssfworkbook.GetSheetAt(sheetNumber);
                    IEnumerator rows = sheet.GetRowEnumerator();
                    int rowFlag = 0;
                    int rowNum = 0;
                    int rowTotal = sheet.LastRowNum;
                    string hasHead = ExcelHelperXml.GetXmlValue(itemName).ToLower(); //判断是否从第一行开始读取数据

                    bool finishSheet = false;
                    while (rows.MoveNext())
                    {
                        rowNum++;
                        IRow row = (IRow)rows.Current;
                        if (hasHead == "true")
                        {
                            if (rowFlag == 0)
                            {
                                rowFlag++;
                                continue;
                            }
                        }

                        for (int k = 0; k < intList.Length; k++)
                        {
                            if (row.GetCell(intList[k] - 1) == null || string.IsNullOrEmpty(row.GetCell(intList[k] - 1).ToString()))
                            {
                                finishSheet = true;
                                break;
                            }
                        }

                        if (finishSheet)
                        {
                            break;
                        }

                        errorInfo = "";
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnsCount; j++)
                        {
                            string value = "";
                            ExcelHelperItem item = list[j] as ExcelHelperItem;
                            ICell cell = row.GetCell(j);
                            string sResult = "";
                            if (item.Seq == -1)
                                value = item.Default_Value;
                            else
                            {
                                if (cell != null)
                                {
                                    if (item.Type == "date")
                                    {
                                        try
                                        {
                                            value = cell.DateCellValue + "";
                                        }
                                        catch
                                        {
                                            value = cell.ToString();
                                        }
                                    }
                                    else
                                    {
                                        value = cell.ToString();
                                    }
                                }
                            }
                            sResult = ValidateData(value, item);
                            if (!string.IsNullOrEmpty(sResult))
                            {
                                errorInfo += "第" + (sheetNumber + 1) + "表格第" + (rowNum - 1) + "行数据 " + sResult + "\n";
                            }
                            dr[item.FieldName] = value;
                        }
                        if (!string.IsNullOrEmpty(errorInfo))
                        {
                            strError += errorInfo;
                            dr["ErrorInfo"] = errorInfo;
                        }

                        dt.Rows.Add(dr);
                    }

                    //ds.Tables.Add(dt);
                    sheetNumber += 1;
                }
            }

            return ds;
        }

        #endregion

        /// <summary>
        /// 读取Excel模板文件Sheet
        /// </summary>
        /// <param name="filePath">模板文件路径</param>
        /// <param name="sheetName">SheetName</param>
        /// <returns></returns>
        public static IWorkbook GetSheetFromTemplete(string filePath, string sheetName)
        {
            FileStream fileTemplateStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IWorkbook hssfWorkBook;
            string extension = Path.GetExtension(filePath);
            if (extension == ".xls")
            {
                hssfWorkBook = new HSSFWorkbook(fileTemplateStream);
            }
            else
            {
                hssfWorkBook = new XSSFWorkbook(fileTemplateStream);
            }
            return hssfWorkBook;
        }

        public static string ExportErrorFile(string argExportFilePath, string argError, string itemNames, params int[] argSheetIndexs)
        {
            try
            {
                FileInfo fi = new FileInfo(argExportFilePath);
                string strExtension = fi.Extension;
                var result = "OK";
                if (strExtension.ToLower().EndsWith("xls"))
                {
                    result = ExportErrorInfoOfXls(argExportFilePath, argError, itemNames, argSheetIndexs);
                }
                else if (strExtension.ToLower().EndsWith("xlsx"))
                {
                    result = ExportErrorInfoOfXlsx(argExportFilePath, argError, itemNames, argSheetIndexs);
                }
                else
                {
                    result = "不支持所选格式,无法导出";
                }
                return result;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用") > -1)
                {
                    return "请关闭导出文件后重试";
                }
                else
                {
                    Log.Error(ex);
                    return "文件操作异常" + ex.Message;
                }

            }
        }

        private static string ExportErrorInfoOfXlsx(string argExportFilePath, string argError, string itemNames, params int[] argSheetIndexs)
        {
            ExcelHelperXml ExcelHelperXml = new ExcelHelperXml();
            int[] infoColValue = new int[argSheetIndexs.Length];

            if (itemNames.IndexOf(",") != -1)
            {
                var itemName = itemNames.Split(',');
                for (int i=0; i < itemName.Length; i++)
                {
                    if (string.IsNullOrEmpty(itemName[i]))
                    {
                        continue;
                    }
                    infoColValue[i] = ExcelHelperXml.GetXmlErrorInfoColValue(itemName[i]);
                }
            }
            foreach (int key in infoColValue)
            {
                if (key <= 0)
                {
                    return "导出错误信息存储列未配置";
                }
            }

            XSSFWorkbook xssfworkbook;
            using (FileStream fs = new FileStream(argExportFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                xssfworkbook = new XSSFWorkbook(fs);
            }

            var errorList = argError.Split('\n');
            int rowIndex = 0;
            int columnIndex = 0;
            int sheetSeqNo = 0;
            foreach (int sheetIndex in argSheetIndexs)
            {
                sheetSeqNo = sheetIndex - 1;
                ISheet sheet = xssfworkbook.GetSheetAt(sheetSeqNo);
                if (sheet == null)
                {
                    continue;
                }

                foreach (string error in errorList)
                {
                    if (error.IndexOf("第" + sheetIndex + "表格") >= 0)
                    {
                        rowIndex = Convert.ToInt32(error.Substring(5, error.IndexOf("行数据") - 5));
                        XSSFRow row = (XSSFRow)sheet.GetRow(rowIndex);
                        if (row != null)
                        {
                            row.CreateCell(row.LastCellNum).SetCellValue(error.Substring(error.IndexOf("行数据") + 4));
                        }
                    }
                }
            }

            using (FileStream fs = new FileStream(argExportFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                xssfworkbook.Write(fs); //创建Excel文件。
                fs.Close();
            }

            using (FileStream fs = new FileStream(argExportFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                xssfworkbook = new XSSFWorkbook(fs);
            }

            //删除正确数据行
            foreach (int sheetIndex in argSheetIndexs)
            {
                sheetSeqNo = sheetIndex - 1;
                ISheet sheet = xssfworkbook.GetSheetAt(sheetSeqNo);
                if (sheet == null)
                {
                    continue;
                }

                for (int j = sheet.LastRowNum; j > 0; j--)
                {
                    IRow row = sheet.GetRow(j);
                    ICell cellLast = row.GetCell(infoColValue[sheetSeqNo]);
                    if (cellLast == null || cellLast.StringCellValue.Length == 0)
                    {
                        int ls = sheet.LastRowNum;
                        if (j < ls)
                        {
                            sheet.ShiftRows(j + 1, ls, -1);
                        }
                        else
                        {
                            sheet.RemoveRow(row);
                        }
                    }
                }
            }

            using (FileStream fs = new FileStream(argExportFilePath, FileMode.Open, FileAccess.Write))
            {
                xssfworkbook.Write(fs); //创建Excel文件。
                fs.Close();
            }

            return "OK";
        }

        private static string ExportErrorInfoOfXls(string argExportFilePath, string argError, string itemNames, params int[] argSheetIndexs)
        {
            ExcelHelperXml ExcelHelperXml = new ExcelHelperXml();
            int[] infoColValue = new int[argSheetIndexs.Length];

            if (itemNames.IndexOf(",") != -1)
            {
                var itemName = itemNames.Split(',');
                for (int i=0; i < itemName.Length; i++)
                {
                    if (string.IsNullOrEmpty(itemName[i]))
                    {
                        continue;
                    }
                    infoColValue[i] = ExcelHelperXml.GetXmlErrorInfoColValue(itemName[i]);
                }
            }
            else
            {
                infoColValue[0] = ExcelHelperXml.GetXmlErrorInfoColValue(itemNames);
            }

            foreach (int key in infoColValue)
            {
                if (key <= 0)
                {
                    return "导出错误信息存储列未配置";
                }
            }

            HSSFWorkbook workBook;
            using (FileStream file = new FileStream(argExportFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                workBook = new HSSFWorkbook(file);
            }

            var errorList = argError.Split('\n');
            int rowIndex = 0;
            int columnIndex = 0;
            int sheetSeqNo = 0;
            foreach (int sheetIndex in argSheetIndexs)
            {
                sheetSeqNo = sheetIndex - 1;
                ISheet sheet = workBook.GetSheetAt(sheetSeqNo);
                if (sheet == null)
                {
                    continue;
                }


                foreach (string error in errorList)
                {
                    if (error.IndexOf("第" + sheetIndex + "表格") >= 0)
                    {
                        rowIndex = Convert.ToInt32(error.Substring(5, error.IndexOf("行数据") - 5));
                        IRow row = sheet.GetRow(rowIndex);
                        if (row != null)
                        {
                            ICell cellLast = row.GetCell(infoColValue[sheetSeqNo]);
                            if (cellLast == null)
                            {
                                cellLast = row.CreateCell(infoColValue[sheetSeqNo]);
                            }
                            cellLast.SetCellValue(cellLast.StringCellValue + error.Substring(error.IndexOf("行数据") + 4));
                        }
                    }
                }
            }
            using (FileStream file = new FileStream(argExportFilePath, FileMode.Open, FileAccess.Write))
            {
                workBook.Write(file); //创建Excel文件。
                file.Close();
            }

            using (FileStream file = new FileStream(argExportFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                workBook = new HSSFWorkbook(file);
            }

            //删除正确数据行
            foreach (int sheetIndex in argSheetIndexs)
            {
                sheetSeqNo = sheetIndex - 1;
                ISheet sheet = workBook.GetSheetAt(sheetSeqNo);
                if (sheet == null)
                {
                    continue;
                }

                for (int j = sheet.LastRowNum; j > 0; j--)
                {
                    IRow row = sheet.GetRow(j);
                    ICell cellLast = row.GetCell(infoColValue[sheetSeqNo]);
                    if (cellLast == null || cellLast.StringCellValue.Length == 0)
                    {
                        int ls = sheet.LastRowNum;
                        if (j < ls)
                        {
                            sheet.ShiftRows(j + 1, ls, -1);
                        }
                        else
                        {
                            sheet.RemoveRow(row);
                        }
                    }
                }
            }

            using (FileStream file = new FileStream(argExportFilePath, FileMode.Open, FileAccess.Write))
            {
                workBook.Write(file); //创建Excel文件。
                file.Close();
            }

            return "OK";
        }

        public static string ExportErrorInfo(string argImportFilePath, string argError, params int[] argSheetIndex)
        {
            return ExportErrorInfo(argImportFilePath, argError, "", argSheetIndex); ;
        }
        /// <summary>
        /// Exports the error information.
        /// </summary>
        /// <param name="argImportFilePath">The argument import file path.</param>
        /// <param name="argError">The argument error.</param>
        /// <param name="argSheetIndex">Index of the argument sheet.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2016/6/17 10:28:02
        public static string ExportErrorInfo(string argImportFilePath, string argError, string itemNames, params int[] argSheetIndex)
        {
            if (string.IsNullOrEmpty(argError)) return "导入数据无错误";

            if (!File.Exists(argImportFilePath)) return "数据导入文件已不存在";

            var exportFilePath = argImportFilePath.ToLower().EndsWith("xlsx") ? argImportFilePath.Replace(".xlsx", "-Error.xlsx") : argImportFilePath.Replace(".xls", "-Error.xls");

            try
            {
                if (File.Exists(exportFilePath)) File.Delete(exportFilePath);

                File.Copy(argImportFilePath, exportFilePath);
            }
            catch
            {
                return "文件操作异常";
            }

            var result = ExportErrorFile(exportFilePath, argError, itemNames, argSheetIndex);

            return result.Equals("OK") ? "OK" + exportFilePath : result;
        }

        /// <summary>
        /// 获取datatable，增加error_msg列,by mbh,请勿修改
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="itemName"></param>
        /// <param name="configFile"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static DataTable GetDataTableFromExcel(string filePath, string itemName, string configFile, ref string strError)
        {
            string errorColumn = "error_msg";
            DataTable dt = null;
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string extension = Path.GetExtension(filePath);
                if (extension == ".xls")
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                else
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
            }
            ExcelHelperXml ExcelHelperXml = new ExcelHelperXml(configFile);
            string hasHead = ExcelHelperXml.GetXmlValue(itemName).ToLower(); //判断是否从第一行开始读取数据
            dt = GetTableColumn(configFile, itemName);
            dt.Columns.Add(errorColumn);
            dt.AcceptChanges();
            int columnsCount = dt.Columns.Count - 1;
            if (columnsCount <= 0)
            {
                strError = "系统无配置项[" + itemName + "],导出操作失败";
                return dt;
            }

            string noEmpty = string.Empty;
            IList list = ExcelHelperXml.ReadXml(itemName, configFile, ref noEmpty);
            string[] noList = noEmpty.Split('|');
            int[] intList = new int[noList.Length];
            for (int j = 0; j < intList.Length; j++)
            {
                intList[j] = Convert.ToInt32(noList[j]);
            }

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            int rowFlag = 0;
            int rowNum = 0;
            StringBuilder sbValidate = new StringBuilder();

            while (rows.MoveNext())
            {
                StringBuilder rowMsg = new StringBuilder();
                rowNum++;
                HSSFRow row = (HSSFRow)rows.Current;
                if (hasHead == "true")
                {
                    if (rowFlag == 0)
                    {
                        rowFlag++;
                        continue;
                    }
                }

                DataRow dr = dt.NewRow();
                if (intList.Any(t => row.GetCell(t - 1) == null || string.IsNullOrEmpty(row.GetCell(t - 1).ToString())))
                {
                    strError += sbValidate.ToString();
                    return dt;
                }

                for (int j = 0; j < columnsCount; j++)
                {
                    string cellValue = string.Empty;
                    ExcelHelperItem item = list[j] as ExcelHelperItem;
                    ICell cell = row.GetCell(j);
                    string errorMessage = string.Empty;
                    if (item.Seq == -1)
                        cellValue = item.Default_Value;
                    else
                    {
                        if (cell != null)
                        {
                            if (cell.CellType == CellType.Numeric && HSSFDateUtil.IsCellDateFormatted(cell))
                            {
                                cellValue = cell.DateCellValue.ToString("yyyy-MM-dd");
                            }
                            else if (item.Type.ToLower() == "date")
                            {
                                cellValue = Convert.ToDateTime(cell.StringCellValue).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                cellValue = cell.ToString();
                            }
                        }
                        errorMessage = ValidateData(cellValue, item);
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            sbValidate.AppendFormat("第{0}行数据 {1}\n", rowNum - 1, errorMessage);
                            rowMsg.Append(errorMessage).Append(";");
                        }
                    }
                    dr[item.FieldName] = cellValue;
                }

                dr[errorColumn] = rowMsg.ToString();
                dt.Rows.Add(dr);
            }

            strError += sbValidate.ToString();
            return dt;
        }

    }

}