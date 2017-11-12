using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MemberTree
{
    /// <summary>
    ///  判断文件的编码，全角半角的相互转换  
    /// </summary>  
    public class TextUtilTool
    {
    	public const string CSVInHeader = "会员id,上级会员id,会员姓名,会员账号,身份证号码,电话号码,email,开户行,银行账户,登记住址,已完成赠与总额,已完成受助总额,已完成赠与次数,已完成受助次数,已支出善种子,已支出善心币,已支出善金币,出局钱包已支出,管理钱包已支出,剩余善种子,剩余善心币,剩余善金币,出局钱包剩余,管理钱包剩余";
    	public const string CSVOutHeader = "会员id,上级id,会员姓名,所在层级,下级层级,直接下级,总下级会员数,会员账号,身份证号,电话号码,email,开户行,银行账户,登记住址,已完成赠与总额,已完成受助总额,已完成赠与次数,已完成受助次数,已支出善种子,已支出善心币,已支出善金币,出局钱包已支出,管理钱包已支出,剩余善种子,剩余善心币,剩余善金币,出局钱包剩余,管理钱包剩余";
    	
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
        /// <param name="input"></param>  
        /// <returns></returns>  
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
        /// <param name="input"></param>  
        /// <returns></returns>  
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

