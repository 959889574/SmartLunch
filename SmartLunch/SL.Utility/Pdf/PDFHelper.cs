using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace SL.Utility
{
   public sealed  class PDFHelper
    {
 
 
       public static Document CreateDocument(string path)
       {
           Document doc = new Document();
           PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
           doc.Open();
           return doc;
       }
       public static Font GetFont(float size, int style, Color color)
       {
           BaseFont bfChinese1 = BaseFont.CreateFont(AppDomain.CurrentDomain.BaseDirectory + "Content\\ARIALUNI.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
           Font font2 = new Font(bfChinese1, size, style, color);
           return font2;
       }

       /// <summary>
       /// 添加列
       /// </summary>
       /// <param name="table"></param>
       /// <param name="text">添加的文本</param>
       /// <param name="colSpan">合并列默认1</param>
       /// <param name="horizontalAlignment">水平对齐 默认1    0左 1中 2右</param>
       /// <param name="showBorder">是否显示边框1 暂时未用</param>
       public static void AddCell(ref PdfPTable table, string text, int colSpan = 1,
           int horizontalAlignment=1,bool showBorder=true)
       {
           var font= GetFont( 10, Font.NORMAL, Color.BLACK);
           PdfPCell cell = new PdfPCell(new Phrase(text,font));

           cell.VerticalAlignment = 1;
           cell.Colspan = colSpan;
           cell.HorizontalAlignment = horizontalAlignment;
           cell.Padding=5f;
           //if (showBorder)
           //    cell.Border = 1;
           //else
           //    cell.Border = 0;
           //cell.BorderWidthLeft = 1;
           //cell.BorderWidthBottom = 1;
           table.AddCell(cell);
           
       }

       public static void AddCell(ref PdfPTable table, Font font, string text, int colSpan = 1,
           int horizontalAlignment=0)
       {
           PdfPCell cell = new PdfPCell(new Phrase(text,font));
           cell.VerticalAlignment = 1;
           cell.Border = 0;
           cell.Colspan = colSpan;
           cell.HorizontalAlignment = horizontalAlignment;
           cell.Padding = 4f;
           table.AddCell(cell);
       }

      
       public static void SetText(string text, ref Document doc, int alignment=0,Font font=null)
       {
           if (font==null)
           {
               font = GetFont(10, Font.NORMAL, Color.BLACK);
           }
           Paragraph p = new Paragraph(new Chunk(text , font));

           p.Alignment = alignment;
           doc.Add(p);
       }
       public static void SetText2(string text, ref Document doc, float leading=0, int alignment = 0, Font font = null)
       {
           if (font == null)
           {
               font = GetFont(12, Font.NORMAL, Color.BLACK);
           }
           Paragraph p = new Paragraph(leading, text, font);

           p.Alignment = alignment;
           doc.Add(p);
       }
     

       public static void ImageDirect(string imagePath, ref Document doc,float x,float y)
       {
           Image img = Image.GetInstance(imagePath);
           img.Alignment = 0;
           //宽高
           img.ScaleAbsolute(200, 200);
           img.SetAbsolutePosition(x, y);
            
           doc.Add(img);

       }

    }
  
}
