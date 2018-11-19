/********************************************************************************
** auth： Ny6000
** date： 2016/11/21 11:53:18
** desc： 加密解密代码类
** Ver.:  V1.0.0
*********************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SL.Utility
{

    /// <summary>
    /// 字符串加密/解密推荐方式,密文为大写字母和数字组合
    /// </summary>
    public sealed class DesCodeHelper
    {
        //加密密钥,要求最短长度8
        private static string encryptKey = "EP.2017.LQ";

        /// <summary>
        /// 加密,密文为大写字母和数字组合
        /// </summary>
        /// <param name="value">待加密明文字符串</param>
        /// <returns></returns>
        public static string EncryptDes(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            try
            {
                StringBuilder sbNew = new StringBuilder();
                DESCryptoServiceProvider DSP = new DESCryptoServiceProvider();
                DSP.Key = Encoding.ASCII.GetBytes(encryptKey.Substring(0, 8));
                DSP.IV = Encoding.ASCII.GetBytes(encryptKey.Substring(0, 8));
                byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(value);
                MemoryStream MS = new MemoryStream();
                CryptoStream CS = new CryptoStream(MS, DSP.CreateEncryptor(), CryptoStreamMode.Write);
                CS.Write(bytes, 0, bytes.Length);
                CS.FlushFinalBlock();
                foreach (byte num in MS.ToArray())
                {
                    sbNew.AppendFormat("{0:X2}", num);
                }
                MS.Close();
                return sbNew.ToString();
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="value">等解密字符串</param>
        /// <param name="argErrorValue">解密错误时返回值</param>
        /// <returns></returns>
        public static string DecryptDes(string value, string argErrorValue = "")
        {
            if (string.IsNullOrEmpty(value))
                return "";

            try
            {
                DESCryptoServiceProvider DSP = new DESCryptoServiceProvider();
                DSP.Key = Encoding.ASCII.GetBytes(encryptKey.Substring(0, 8));
                DSP.IV = Encoding.ASCII.GetBytes(encryptKey.Substring(0, 8));
                byte[] buffer = new byte[value.Length / 2];
                for (int i = 0; i < value.Length / 2; i++)
                {
                    int num2 = Convert.ToInt32(value.Substring(i * 2, 2), 0x10);
                    buffer[i] = (byte)num2;
                }
                MemoryStream Ms = new MemoryStream();
                CryptoStream CS = new CryptoStream(Ms, DSP.CreateDecryptor(), CryptoStreamMode.Write);
                CS.Write(buffer, 0, buffer.Length);
                CS.FlushFinalBlock();
                Ms.Close();
                return Encoding.GetEncoding("GB2312").GetString(Ms.ToArray());
            }
            catch
            {
                return argErrorValue;
            }
        }

        /// <summary>
        /// 加密整数
        /// </summary>
        /// <param name="argEncryptInt">The argument encrypt int.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/6/27 10:44:17
        public static string EncryptDES(int argEncryptInt)
        {
            return EncryptDes(argEncryptInt.DBColToString());
        }

        /// <summary>
        /// 加密浮点数
        /// </summary>
        /// <param name="argEncryptDecimal">The argument encrypt decimal.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/6/27 10:45:21
        public static string EncryptDES(decimal argEncryptDecimal)
        {
            return EncryptDes(argEncryptDecimal.DBColToString());
        }
    }

    /// <summary>
    /// MD5取序列化值/验证
    /// </summary>
    public sealed class Md5Helper
    {
        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="strText">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        private static string GetMD5Encrypt(string argWaitToEncrypt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(argWaitToEncrypt));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                // 以十六进制格式格式化  
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// MD5s the check.
        /// </summary>
        /// <param name="agrInputValue">The agr input value.</param>
        /// <param name="argMD5Value">The argument m d5 value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/04/11 15:10:00
        public static bool Md5Check(string agrInputValue, string argMD5Value)
        {
            var md5Key = GetMD5Encrypt(agrInputValue);

            return !md5Key.Equals(argMD5Value) ? false : true;
        }
    }



    /// <summary>
    /// 字符串加密/解密(报关EDI库数据加密方式)
    /// </summary>
    public class Encryptor
    {
        private static string key = "adgaw334^*^&#$#$W2343qwreqwr12";
        private static SymmetricAlgorithm mobjCryptoService = new RijndaelManaged();

        /// <summary>
        /// 字符串解密,返回明文
        /// </summary>
        /// <param name="Source">The source.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/1/23 11:21:59
        public static string Decrypt(string Source)
        {
            if (string.IsNullOrEmpty(Source))
                return string.Empty;
            byte[] buffer = Convert.FromBase64String(Source);
            MemoryStream stream = new MemoryStream(buffer, 0, buffer.Length);
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform transform = mobjCryptoService.CreateDecryptor();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(stream2);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 字符串加密,返回加密后密文
        /// </summary>
        /// <param name="Source">The source.</param>
        /// <returns>System.String.</returns>
        ///  Created By Ny6000
        ///  Created Date:2017/1/23 11:22:21
        public static string Encrypt(string Source)
        {
            if (string.IsNullOrEmpty(Source))
                return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(Source);
            MemoryStream stream = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform transform = mobjCryptoService.CreateEncryptor();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream.Close();
            return Convert.ToBase64String(stream.ToArray());
        }


        private static byte[] GetLegalIV()
        {
            string s = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            int length = mobjCryptoService.IV.Length;
            if (s.Length > length)
            {
                s = s.Substring(0, length);
            }
            else if (s.Length < length)
            {
                s = s.PadRight(length, ' ');
            }
            return Encoding.ASCII.GetBytes(s);
        }

        private static byte[] GetLegalKey()
        {
            string key = Encryptor.key;
            mobjCryptoService.GenerateKey();
            int length = mobjCryptoService.Key.Length;
            if (key.Length > length)
            {
                key = key.Substring(0, length);
            }
            else if (key.Length < length)
            {
                key = key.PadRight(length, ' ');
            }
            return Encoding.ASCII.GetBytes(key);
        }



    }

}
