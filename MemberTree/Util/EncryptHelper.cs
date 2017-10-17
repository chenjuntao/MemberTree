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
        
         #region 加密字符串  
        /// <summary> /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string Encrypt(string str)  
        {    
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象
            byte[] key = Encoding.ASCII.GetBytes(SECRETKEY); //定义字节数组，用来存储密钥
            byte[] data = Encoding.ASCII.GetBytes(str);//定义字节数组，用来存储要加密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      
            CStream.FlushFinalBlock();              //释放加密流      
            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }  
        #endregion 
  
        #region 解密字符串   
        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt(string str)  
        {      
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象
            byte[] key = Encoding.ASCII.GetBytes(SECRETKEY); //定义字节数组，用来存储密钥    
            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);   
            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     
            CStream.FlushFinalBlock();               //释放解密流      
            return Encoding.ASCII.GetString(MStream.ToArray());       //返回解密后的字符串  
        }  
        #endregion 
    }
}
