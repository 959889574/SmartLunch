using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Data;
using System.IO;
using System.Web;


namespace SL.Utility
{
    public static class ExpExcelFromTemplete
    {
        public static void ExpDeliyMoney(string expFilePath, string templeteFilePath, string sheetName, string[] dtColumns, DataTable dt)
        {
            IWorkbook workBook =  ExcelHelper.GetSheetFromTemplete(templeteFilePath, sheetName);
            ISheet sheet = workBook.GetSheet(sheetName);

            for (int iRowIndex = 0; iRowIndex < dt.Rows.Count; iRowIndex++)
            {
                IRow Row = sheet.CreateRow(iRowIndex + 1);
                for (int iColumnIndex = 0; iColumnIndex < dtColumns.Length; iColumnIndex++)
                {
                    Row.CreateCell(iColumnIndex).SetCellValue(dt.Rows[iRowIndex][dtColumns[iColumnIndex]].ToString());
                }
            }

            using (FileStream file = new FileStream(expFilePath, FileMode.Create))
            {
                workBook.Write(file);
                file.Close();
            }
        }

        public static ICellStyle GetCellStype(IWorkbook wk)
        {
            ICellStyle cellStyle = wk.CreateCellStyle();
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            return cellStyle;
        }

        /// <summary>
        /// Exps the statistics all.
        /// </summary>
        /// <param name="expFilePath">The exp file path.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="dtTotal">The dt total.</param>
        /// <param name="dtColumnsName">Name of the dt columns.</param>
        /// <param name="fileColumnsName">Name of the file columns.</param>
        /// <param name="argPayCount">The argument pay count.</param>
        /// <param name="argReceiveCount">The argument receive count.</param>
        ///  Created By Ny6000
        ///  Created Date:2016/12/27 15:59:53
        //public static void ExpStatisticsAll(string expFilePath, string sheetName
        //    , DataTable dtTotal, string[] dtColumnsName, string[] fileColumnsName
        //    , int argPayCount, int argReceiveCount)
        //{
        //    IWorkbook workBook =  new HSSFWorkbook();
        //    ISheet sheet = workBook.CreateSheet(sheetName);

        //    IRow Row = sheet.CreateRow(0);
        //    IRow Row2 = sheet.CreateRow(1);
        //    var isPay = false;
        //    var isReceive = false;
        //    for (int iColumnIndex = 0,j = fileColumnsName.Length; iColumnIndex < j; iColumnIndex++)
        //    {
        //        if (!dtColumnsName[iColumnIndex].StartsWith("应付") 
        //            && !dtColumnsName[iColumnIndex].StartsWith("应收")
        //            && !dtColumnsName[iColumnIndex].StartsWith("RECEIVEMONEY")
        //            && !dtColumnsName[iColumnIndex].StartsWith("PAYMONEY"))
        //        {
        //            Row.CreateCell(iColumnIndex).SetCellValue(fileColumnsName[iColumnIndex]);
        //            //合并表头当前列的标题行2行为一行
        //            sheet.AddMergedRegion(new CellRangeAddress(0, 1, iColumnIndex, iColumnIndex));
        //        }
        //        else
        //        {
        //            //if (dtColumnsName[iColumnIndex].StartsWith("合计费用"))
        //            //{
        //                Row2.CreateCell(iColumnIndex).SetCellValue(fileColumnsName[iColumnIndex]);
        //            //}
        //            //else
        //            //{
        //            //    Row2.CreateCell(iColumnIndex).SetCellValue(fileColumnsName[iColumnIndex]);
        //            //}
        //            if (dtColumnsName[iColumnIndex].StartsWith("应付") && isPay.Equals(false))
        //            {
        //                Row.CreateCell(iColumnIndex).SetCellValue("应付");
        //                sheet.AddMergedRegion(new CellRangeAddress(0, 0, iColumnIndex, iColumnIndex + argPayCount));
        //                isPay = true;
        //            }

        //            if (dtColumnsName[iColumnIndex].StartsWith("应收") && isReceive.Equals(false))
        //            {
        //                Row.CreateCell(iColumnIndex).SetCellValue("应收");
        //                sheet.AddMergedRegion(new CellRangeAddress(0, 0, iColumnIndex, iColumnIndex + argReceiveCount));
        //                isReceive = true;
        //            }
        //        }
        //    }

        //    decimal rowMoney = 0;
        //    var tColumns = dtColumnsName.Length;
        //    for (int iRowIndex = 0,tRowTotal = dtTotal.Rows.Count; iRowIndex < tRowTotal; iRowIndex++)
        //    {
        //        Row = sheet.CreateRow(iRowIndex + 2);
        //        for (int iColumnIndex = 0; iColumnIndex < tColumns; iColumnIndex++)
        //        {
        //            Row.CreateCell(iColumnIndex).SetCellValue(dtTotal.Rows[iRowIndex][dtColumnsName[iColumnIndex]].ToString());
        //        }
        //    }

        //    using (FileStream file = new FileStream(expFilePath, FileMode.Create))
        //    {
        //        workBook.Write(file);
        //        file.Close();
        //    }
        //}





    }
}
