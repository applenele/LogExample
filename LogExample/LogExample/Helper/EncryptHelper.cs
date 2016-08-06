using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LogExample.Helper
{
    public class EncryptHelper
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public static readonly string _Key = "smallcode";
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <Param name="encryptString">待加密的字符串</Param>
        /// <Param name="Key">8位加密Key</Param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DESEncode(string encryptString, string Key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Key) || Key.Length != 8)
                {
                    Key = _Key;
                }
                var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var des = new DESCryptoServiceProvider();
                des.Key = Encoding.ASCII.GetBytes(Key);
                des.Mode = CipherMode.ECB;
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception)
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <Param name="encryptString">待加密的字符串</Param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DESEncode(string encryptString)
        {
            try
            {
                return DESEncode(encryptString, "");
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <Param name="decryptString">待解密的字符串</Param>
        /// <Param name="Key">8位解密Key</Param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DESDecode(string decryptString, string Key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Key) || Key.Length != 8)
                {
                    Key = _Key;
                }
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                var des = new DESCryptoServiceProvider();
                des.Key = Encoding.ASCII.GetBytes(Key);
                des.Mode = CipherMode.ECB;
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, des.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <Param name="decryptString">待解密的字符串</Param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DESDecode(string decryptString)
        {
            try
            {
                return DESDecode(decryptString, "");
            }
            catch
            {
                return decryptString;
            }
        }
    }
}