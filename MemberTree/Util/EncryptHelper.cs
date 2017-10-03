/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/03/2017
 * 时间: 16:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MemberTree
{

	/// <summary>
    /// 加密解密
    /// </summary>
    public static class EncryptHelper
    {
        /// <summary>
        /// 加密字符
        /// </summary>
        private const string SECRETKEY = "HNPOLICE";

         /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="filename">文件存放路径</param>
        /// <param name="soce">加密内容</param>
        internal static void FileEncrypt(string filename, string soce)
        {
            FileStream fsEncrypted = new FileStream(filename,FileMode.Create, FileAccess.Write);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(SECRETKEY);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(SECRETKEY);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,  desencrypt, CryptoStreamMode.Write);
            byte[] fsInput = Encoding.UTF8.GetBytes(soce);
            cryptostream.Write(fsInput, 0, fsInput.Length);
            cryptostream.Close();
            fsEncrypted.Close();
        }
        
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filename">打开文件路径</param>
        /// <returns>返回加密文件的内容</returns>
        internal static string FileDecrypt(string filename)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(SECRETKEY);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(SECRETKEY);
            FileStream fsread = new FileStream(filename,  FileMode.Open,  FileAccess.Read);
            ICryptoTransform desdecrypt = DES.CreateDecryptor();
            CryptoStream cryptostreamDecr = new CryptoStream(fsread,  desdecrypt,  CryptoStreamMode.Read);
            StreamReader read = new StreamReader(cryptostreamDecr, Encoding.UTF8);
            string reft = read.ReadToEnd();
            fsread.Flush();
            fsread.Close();
            return reft;
        }
    }
}
