using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MemberTree
{
	//大小写转换
	public enum EnumUpperLower
	{
		None=0, //不转换
		Lower=1,//转换为小写
		Upper=2	//转换为大写
	};
	
	//全角半角转换
	public enum EnumDBCSBC
	{
		None=0, //不转换
		DBC=1,	//转换为全角符号
		SBC=2	//转换为半角符号
	};
	
	//剔除空白字符
	public enum EnumTrim
	{
		None=0, //不作处理
		All=1,	//剔除首尾空白字符
		Start=2, //剔除首部空白字符
		End=3	//剔除尾部空白字符
	};
    	
    /// <summary>
    ///  判断文件的编码，全角半角的相互转换  
    /// </summary>  
    public static class TextUtil
    {
    	public static EnumUpperLower enUpperLower;
    	public static EnumDBCSBC enDBCSBC;
    	public static EnumTrim enTrim;
    	
        //判断文件编码
        public static Encoding GetFileEncodeType(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            fs.Close();
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }
                else
                {
                    return Encoding.Default;
                }
            }
            else
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// 半角转成全角  
        /// 半角空格32,全角空格12288  
        /// 其他字符半角33~126,其他字符全角65281~65374,相差65248  
        /// </summary>  
        public static string DBCToSBC(string input)
        {
            char[] cc = input.ToCharArray();
            for (int i = 0; i < cc.Length; i++)
            {
                if (cc[i] == 32)
                {
                    // 表示空格  
                    cc[i] = (char)12288;
                    continue;
                }
                if (cc[i] < 127 && cc[i] > 32)
                {
                    cc[i] = (char)(cc[i] + 65248);
                }
            }
            return new string(cc);
        }

        /// <summary>
        /// 全角转半角  
        /// 半角空格32,全角空格12288  
        /// 其他字符半角33~126,其他字符全角65281~65374,相差65248  
        /// </summary>  
        public static string SBCToDBC(string input)
        {
            char[] cc = input.ToCharArray();
            for (int i = 0; i < cc.Length; i++)
            {
                if (cc[i] == 12288)
                {
                    // 表示空格  
                    cc[i] = (char)32;
                    continue;
                }
                if (cc[i] > 65280 && cc[i] < 65375)
                {
                    cc[i] = (char)(cc[i] - 65248);
                }

            }
            return new string(cc);
        }
    }  
}

