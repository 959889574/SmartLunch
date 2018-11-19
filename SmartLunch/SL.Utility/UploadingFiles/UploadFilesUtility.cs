using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using SL.Utility;

namespace SL.Utility
{
    public class UploadFilesUtility
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="file">文件</param>
        /// <returns>item1:是否成功 item2:文件名称/异常信息 item3:虚拟路径</returns>
        public static Tuple<bool, string, string> UploadFiles(FileType type, HttpPostedFileBase file)
        {
            string path = string.Empty;
            string fileName = string.Empty;
            bool flag = false;
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string filePath = HttpRuntime.AppDomainAppPath;
                    fileName = Guid.NewGuid().ToString() + file.FileName.Remove(0, file.FileName.LastIndexOf('.'));

                    path = "/UploadFiles/" + type.ToString() + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                    filePath += path; //AppDomain.CurrentDomain.BaseDirectory HttpServerUtilityBase.Server.MapPath(string.Format("~/{0}", "File"));
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    path += fileName;
                    file.SaveAs(Path.Combine(filePath, fileName));
                    flag = true;
                }
                else
                {
                    fileName = "上传文件为空";
                }
            }
            catch (Exception ex)
            {
                fileName = ex.Message;
                LogManage.ErrorLog("上传文件异常", ex);
            }
            return Tuple.Create(flag, fileName, path);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="file">文件</param>
        /// <returns>item1:是否成功 item2:文件名称/异常信息 item3:虚拟路径</returns>
        public static Tuple<bool, string, string> UploadImg(HttpPostedFileBase file)
        {
            return UploadFiles(FileType.Image, file);
        }
    }
    public enum FileType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Image = 0,
        /// <summary>
        /// 压缩文件
        /// </summary>
        Zip = 1,
        /// <summary>
        /// 文档
        /// </summary>
        Text = 2,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 3,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 4

    }
}
