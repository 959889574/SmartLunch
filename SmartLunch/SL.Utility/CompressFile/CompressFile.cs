using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SL.Utility
{
    public static class CompressFile
    {


        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="dirPath">压缩文件夹的路径</param>
        /// <param name="fileName">生成的zip文件路径</param>
        /// <param name="level">压缩级别 0 - 9 0是存储级别 9是最大压缩</param>
        /// <param name="bufferSize">读取文件的缓冲区大小</param>
        private static void CompressDirectory(string dirPath, string fileName, int level, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            using (ZipOutputStream s = new ZipOutputStream(File.Create(fileName)))
            {
                s.SetLevel(level);
                CompressDirectory(dirPath, dirPath, s, buffer);
                s.Finish();
                s.Close();
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="root">压缩文件夹路径</param>
        /// <param name="path">压缩文件夹内当前要压缩的文件夹路径</param>
        /// <param name="s"></param>
        /// <param name="buffer">读取文件的缓冲区大小</param>
        private static void CompressDirectory(string root, string path, ZipOutputStream s, byte[] buffer)
        {
            root = root.TrimEnd('/') + "//";
            string[] fileNames = Directory.GetFiles(path);
            string[] dirNames = Directory.GetDirectories(path);
            string relativePath = path.Replace(root, "");
            if (relativePath != "")
            {
                relativePath = relativePath.Replace("//", "/") + "/";
            }
            int sourceBytes;
            foreach (string file in fileNames)
            {

                ZipEntry entry = new ZipEntry(relativePath + Path.GetFileName(file));
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(file))
                {
                    do
                    {
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, sourceBytes);
                    } while (sourceBytes > 0);
                }
            }

            foreach (string dirName in dirNames)
            {
                string relativeDirPath = dirName.Replace(root, "");
                ZipEntry entry = new ZipEntry(relativeDirPath.Replace("//", "/") + "/");
                s.PutNextEntry(entry);
                CompressDirectory(root, dirName, s, buffer);
            }
        }




        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="directoryToZip">要压缩的文件夹路径</param>
        /// <param name="zipedDirectory">压缩后的文件夹路径</param>
        public static void ZipDerctory(string directoryToZip, string zipedDirectory, int compressionLevel, int bufferSize)
        {
            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedDirectory)))
            {
                byte[] buffer = new byte[bufferSize];
                //得到文件夹下的所有文件夹及
                ArrayList fileList = GetFileList(directoryToZip);
                int directoryNameLength = (Directory.GetParent(directoryToZip)).ToString().Length;

                zipStream.SetLevel(compressionLevel);
                ZipEntry zipEntry = null;

                //循环压缩

                foreach (string fileName in fileList)
                {

                    string file = fileName.Remove(0, directoryNameLength);
                    zipEntry = new ZipEntry(file.Substring(1, file.Length - 1));
                    zipStream.PutNextEntry(zipEntry);

                    if (!fileName.EndsWith(@"/"))
                    {
                        FileStream fileStream = null;
                        fileStream = File.OpenRead(fileName);
                        while (true)
                        {
                            int readCount = fileStream.Read(buffer, 0, buffer.Length);
                            if (readCount < 1)
                            {
                                fileStream.Close();
                                break;
                            }
                            zipStream.Write(buffer, 0, readCount);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 得到文件下的所有文件
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <returns></returns>
        public static ArrayList GetFileList(string directory)
        {
            ArrayList fileList = new ArrayList();
            bool isEmpty = true;
            foreach (string file in Directory.GetFiles(directory))
            {
                fileList.Add(file);
                isEmpty = false;
            }
            if (isEmpty)
            {
                if (Directory.GetDirectories(directory).Length == 0)
                {
                    fileList.Add(directory + @"/");
                }
            }
            foreach (string dirs in Directory.GetDirectories(directory))
            {
                foreach (object obj in GetFileList(dirs))
                {
                    fileList.Add(obj);
                }
            }
            return fileList;
        }


        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        public static void DeCompressFile(string sourceFile, string destinationFile)
        {
            if (!File.Exists(sourceFile)) throw new FileNotFoundException();
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                byte[] quartetBuffer = new byte[4];
                const int bufferLength = 1024 * 64;
                /*压缩文件的流的最后四个字节保存的是文件未压缩前的长度信息，
                 * 把该字节数组转换成int型，可获取文件长度。
                 * int position = (int)sourceStream.Length - 4;
                sourceStream.Position = position;
                sourceStream.Read(quartetBuffer, 0, 4);
                sourceStream.Position = 0;
                int checkLength = BitConverter.ToInt32(quartetBuffer, 0);*/
                byte[] buffer = new byte[1024 * 64];
                using (GZipStream decompressedStream = new GZipStream(sourceStream, CompressionMode.Decompress, true))
                {
                    using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create))
                    {
                        //int total = 0;
                        int bytesRead = 0;
                        while ((bytesRead = decompressedStream.Read(buffer, 0, bufferLength)) >= bufferLength)
                        {
                            destinationStream.Write(buffer, 0, bufferLength);
                        }
                        destinationStream.Write(buffer, 0, bytesRead);
                        destinationStream.Flush();
                    }
                }
            }

        }
    }
}
