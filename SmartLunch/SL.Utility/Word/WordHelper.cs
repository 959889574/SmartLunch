using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using AW = Aspose.Words;
using AWTables = Aspose.Words.Tables;
using AWCells = Aspose.Cells;
using Aspose.Words;
using Aspose.Words.Fields;

namespace SL.Utility
{
    /// <summary>
    /// word 版本
    /// </summary>
    public enum WordHelperEnumWordVersion
    {
        w2003 = 2003,
        w2007 = 2007
    }
    /// <summary>
    /// 字体大小
    /// </summary>
    public enum WordHelperEnumFontSize
    {
        初号 = 42,
        小初 = 36,
        一号 = 26,
        小一号 = 24,
        二号 = 22,
        小二号 = 18,
        三号 = 16,
        小三号 = 15,
        四号 = 14,
        小四号 = 12,
        五号 = 10,
        小五号 = 9,
        六号 = 7,
        小六号 = 6,
        七号 = 5,
        八号 = 5
    }

    public class WordHelper
    {
        #region 单例写法
        //private volatile static WordHelper _wordHelper = null;
        //private static readonly object lockHelper = new object();
        //private WordHelper() { }
        //public static WordHelper Generate()
        //{
        //    if (_wordHelper == null)
        //    {
        //        lock (lockHelper)
        //        {
        //            if (_wordHelper == null)
        //                _wordHelper = new WordHelper();
        //        }
        //    }
        //    return _wordHelper;
        //}
        #endregion

        private AW.Document wDoc = null;

        private AW.DocumentBuilder wDocBuilder = null;

        #region 初始化
        public WordHelper()
        {
            if (wDoc == null)
            {
                wDoc = new AW.Document();
            }
            if (wDoc != null && wDocBuilder == null)
            {
                wDocBuilder = new AW.DocumentBuilder(wDoc);
            }
        }

        public WordHelper(string fileName, AW.LoadOptions loadOptions = null)
        {
            if (wDoc == null)
            {
                wDoc = loadOptions == null ? new AW.Document(fileName) : new AW.Document(fileName, loadOptions);
            }
            if (wDoc != null && wDocBuilder == null)
            {
                wDocBuilder = new AW.DocumentBuilder(wDoc);
            }
        }

        public WordHelper(Stream stream, AW.LoadOptions loadOptions = null)
        {
            if (wDoc == null)
            {
                wDoc = loadOptions == null ? new AW.Document(stream) : new AW.Document(stream, loadOptions);
            }
            if (wDoc != null && wDocBuilder == null)
            {
                wDocBuilder = new AW.DocumentBuilder(wDoc);
            }
        }

        #endregion


        public AW.Document GetDocument()
        {
            return wDoc;
        }


        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="strFileName"></param>
        public void SaveAs(string fileName, WordHelperEnumWordVersion version = WordHelperEnumWordVersion.w2007)
        {

            if (version == WordHelperEnumWordVersion.w2007)
            {
                wDoc.Save(fileName, AW.SaveFormat.Docx);
            }
            else if (version == WordHelperEnumWordVersion.w2003)
            {
                wDoc.Save(fileName, AW.SaveFormat.Doc);
            }
        }

        #region 文档处理

        /// <summary>
        /// 换行
        /// </summary>
        /// <param name="nline"></param>
        public void InsertLineBreak(int nline)
        {
            for (int i = 0; i < nline; i++)
                wDocBuilder.InsertBreak(AW.BreakType.LineBreak);
        }

        #endregion



        #region 写入word存文字内容
        /// <summary>
        /// 写入文字内容
        /// </summary>
        /// <param name="content">文字内容</param>
        /// <param name="isBold">是否粗体</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="isWriteLine">是否换行</param>
        /// <param name="blankLineNum">空行数量</param>
        /// <param name="paragraphAlignment">内容对齐格式</param>
        /// <param name="fontSize">字体大小</param>
        public void WriteText(DocumentBuilder wDocBuilder, string content, bool isBold, Color fontColor, bool isWriteLine = false, int blankLineNum = 0, AW.ParagraphAlignment paragraphAlignment = AW.ParagraphAlignment.Left, WordHelperEnumFontSize fontSize = WordHelperEnumFontSize.四号, bool isTitle = false)
        {

            if (fontColor == null)
            {
                fontColor = Color.Black;
            }
            wDocBuilder.Bold = isBold;
            wDocBuilder.Font.Color = fontColor;
            wDocBuilder.Font.Size = (double)fontSize;
            wDocBuilder.ParagraphFormat.Alignment = paragraphAlignment;
            if (isWriteLine)
            {
                wDocBuilder.Writeln(content);
                if (blankLineNum <= 0)
                {
                    blankLineNum = 0;
                }
                else
                {
                    for (int i = 0; i < blankLineNum; i++)
                    {
                        wDocBuilder.InsertBreak(AW.BreakType.ParagraphBreak);
                    }
                }
            }
            else
            {
                wDocBuilder.Write(content);
            }
        }
        public void WriteText(string content)
        {
            WriteText(content, false, Color.Black);
        }
        public void WriteText(string content, bool isBold)
        {
            WriteText(content, isBold, Color.Black);
        }
        public void WriteText(string content, string argMarkName, AW.ParagraphAlignment paragraphAlignment = AW.ParagraphAlignment.Left)
        {
            wDocBuilder.Bold = false;
            wDocBuilder.Font.Color = Color.Black;
            wDocBuilder.Font.Size = double.Parse(((int)WordHelperEnumFontSize.小四号).ToString());
            wDocBuilder.ParagraphFormat.Alignment = paragraphAlignment;

            if (wDoc.Range.Bookmarks["" + argMarkName] != null)
            {
                Bookmark mark = wDoc.Range.Bookmarks["" + argMarkName];
                mark.Text = "" + content;
            }

        }

       public void WriteText(string content, bool isBold, Color fontColor)
        {
            WriteText(content, isBold, fontColor,false,0);
        }
        public void WriteText(string content, bool isBold, Color fontColor, bool isWriteLine, int blankLineNum)
        {
            WriteText(wDocBuilder, content, isBold, fontColor, isWriteLine, blankLineNum);
        }

        #endregion

        #region 写入表格内容

        private void InsertCell(string argContent, double argWidth)
        {
            wDocBuilder.InsertCell();
            wDocBuilder.Font.Size = (double)WordHelperEnumFontSize.小四号;
            wDocBuilder.Bold = false;

            wDocBuilder.ParagraphFormat.Alignment = AW.ParagraphAlignment.Left;
            wDocBuilder.CellFormat.VerticalAlignment = AWTables.CellVerticalAlignment.Center;
            wDocBuilder.CellFormat.VerticalMerge = AWTables.CellMerge.First;
            wDocBuilder.CellFormat.Borders.LineStyle = AW.LineStyle.Single;
            wDocBuilder.CellFormat.Width = argWidth;
            wDocBuilder.Write(argContent + "");

        }

        private string GetNumberValue(string argValueInput, int iXiaoShu)
        {
            if (string.IsNullOrEmpty(argValueInput))
                return string.Empty;

            double dReturn = 0.0d;
            double.TryParse(argValueInput, out dReturn);

            return dReturn.ToString("f" + iXiaoShu.ToString());
        }

        public void WriteTable(DataTable source)
        {
            AW.Tables.Table table = wDocBuilder.StartTable();
            for (int y = 0; y < source.Columns.Count; y++)
            {
                wDocBuilder.InsertCell();
                wDocBuilder.ParagraphFormat.Alignment = AW.ParagraphAlignment.Left;
                wDocBuilder.CellFormat.VerticalAlignment = AWTables.CellVerticalAlignment.Center;
                wDocBuilder.CellFormat.VerticalMerge = AWTables.CellMerge.First;
                wDocBuilder.CellFormat.Borders.LineStyle = AW.LineStyle.Single;
                wDocBuilder.Write(source.Columns[y].ColumnName + "");
            }
            wDocBuilder.EndRow();
            for (int x = 0; x < source.Rows.Count; x++)
            {
                wDocBuilder.RowFormat.Height = 25;
                for (int y = 0; y < source.Columns.Count; y++)
                {
                    wDocBuilder.InsertCell();
                    wDocBuilder.Font.Size = (double)WordHelperEnumFontSize.四号;
                    wDocBuilder.Bold = false;

                    wDocBuilder.ParagraphFormat.Alignment = AW.ParagraphAlignment.Left;
                    wDocBuilder.CellFormat.VerticalAlignment = AWTables.CellVerticalAlignment.Center;
                    wDocBuilder.CellFormat.VerticalMerge = AWTables.CellMerge.First;
                    wDocBuilder.CellFormat.Borders.LineStyle = AW.LineStyle.Single;
                    wDocBuilder.Write(source.Rows[x][y] + "");
                }
                wDocBuilder.EndRow();
            }
            wDocBuilder.EndTable();
        }

        /// <summary>
        /// 写入表格列
        /// </summary>
        /// <param name="cellnfo"></param>
        public void WriteTableCell(List<WordHelperAWTableCellInfo> cellnfo)
        {
            if (cellnfo != null && cellnfo.Count > 0)
            {
                wDocBuilder.StartTable();
                foreach (WordHelperAWTableCellInfo info in cellnfo)
                {
                    wDocBuilder.ParagraphFormat.Alignment = AW.ParagraphAlignment.Left;
                    wDocBuilder.InsertCell();
                    if (info.Width > 0)
                        wDocBuilder.CellFormat.Width = info.Width;
                    wDocBuilder.Write(info.Value);
                }
                wDocBuilder.EndRow();
                wDocBuilder.EndTable();



            }
        }

        /// <summary>
        /// 写入表格行
        /// </summary>
        /// <param name="rowInfos"></param>
        /// <param name="isHorizontal"></param>
        public void WriteTableRow(List<WordHelperAWTableRowInfo> rowInfos)
        {
            if (rowInfos != null && rowInfos.Count > 0)
            {
                foreach (WordHelperAWTableRowInfo row in rowInfos)
                {
                    WriteTableCell(row.Cells);
                }
            }
        }

        #endregion

        #region 新增空白页
        public void AddBlankPage(int count)
        {
            if (count > 0)
            {
                AW.Paragraph par = null;
                AW.Run run = null;
                for (int i = 0; i < count; i++)
                {
                    par = new AW.Paragraph(wDoc);
                    run = new AW.Run(wDoc, AW.ControlChar.PageBreak);
                    par.AppendChild(run);
                    wDoc.LastSection.Body.AppendChild(par);
                }
            }
        }
        #endregion



        #region 写入书签
        public void WriteBookmark(string bookmarkName, string bookmarkContent)
        {
            wDocBuilder.StartBookmark(bookmarkName);
            wDocBuilder.Write(bookmarkContent);
            wDocBuilder.EndBookmark(bookmarkName);
            AW.Bookmark bm = wDoc.Range.Bookmarks[bookmarkName];
        }
        #endregion

        #region 写入域目录

        public void WriteDiectory()
        {
            wDoc.FirstSection.Body.PrependChild(new AW.Paragraph(wDoc)); //添加新段落
            wDocBuilder.MoveToDocumentStart(); //移除文档光标

        }

        #endregion





    }

    public class WordHelperAWTableCellInfo
    {
        public string Value { get; set; }

        public double Width { get; set; }
    }

    public class WordHelperAWTableRowInfo
    {
        public List<WordHelperAWTableCellInfo> Cells { get; set; }
    }

}
