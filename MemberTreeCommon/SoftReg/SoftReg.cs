/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 11/30/2017
 * 时间: 16:06
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Management;
using System.Windows;
using RSACommon;

namespace MemberTree
{
	public class SoftReg
    {
		public static string Com = "";
		public static string Usr = "";
		
        // 取得设备硬盘的卷标号
        private static string GetDiskVolumeSerialNumber()
        {
			//获取当前系统磁盘符方法1，返回：C:
			string sys_path = Environment.GetEnvironmentVariable("systemdrive");
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\""+ sys_path +"\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        //获得CPU的序列号
        private static string getCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuConnection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
                break;
            }
            return strCpu;
        }

        //生成机器码
        public static string getMNum()
        {
            string strNum = getCpu() + GetDiskVolumeSerialNumber();//获得24位Cpu和硬盘序列号
            string strMNum = strNum.Substring(0,24);//从生成的字符串中取出前24个字符做为机器码
            return strMNum;
        }
        
        //判断是否已经注册
        public static bool hasReged()
        {
        	string mNum = getMNum();
        	if(File.Exists("reg.dll"))
        	{
        		try 
        		{
        			string regMsg = EncryptHelper.FileDecrypt("reg.dll", mNum);
        			string[] regList = regMsg.Split(new String[]{mNum}, StringSplitOptions.None);
        			Com = RSAHelper.DecryptString(regList[0]);
        			Usr = RSAHelper.DecryptString(regList[1]);
        			return true;
        		} 
        		catch (Exception ex)
        		{
        			MessageBox.Show("注册密钥不正确！\n"+ex.Message);
        			return false;
        		}
        	}
        	return false;
        }

        public int[] intCode = new int[127];//存储密钥
        public int[] intNumber = new int[25];//存机器码的Ascii值
        public char[] Charcode = new char[25];//存储机器码字

        public void setIntCode()//给数组赋值小于10的数
        {
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i % 9;
            }
        }

        //生成注册码
        public string getRNum(string str)
        {
            setIntCode();//初始化127位数组
            for (int i = 1; i < Charcode.Length; i++)//把机器码存入数组中
            {
                Charcode[i] = Convert.ToChar(str.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++)//把字符的ASCII值存入一个整数组中。
            {
                intNumber[j] = intCode[Convert.ToInt32(Charcode[j])] + Convert.ToInt32(Charcode[j]);
            }
            string strAsciiName = "";//用于存储注册码
            for (int j = 1; j < intNumber.Length; j++)
            {
                if (intNumber[j] >= 48 && intNumber[j] <= 57)//判断字符ASCII值是否0－9之间
                {
                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                }
                else if (intNumber[j] >= 65 && intNumber[j] <= 90)//判断字符ASCII值是否A－Z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                }
                else if (intNumber[j] >= 97 && intNumber[j] <= 122)//判断字符ASCII值是否a－z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                }
                else//判断字符ASCII值不在以上范围内
                {
                    if (intNumber[j] > 122)//判断字符ASCII值是否大于z
                    {
                        strAsciiName += Convert.ToChar(intNumber[j] - 10).ToString();
                    }
                    else
                    {
                        strAsciiName += Convert.ToChar(intNumber[j] - 9).ToString();
                    }
                }
            }
            return strAsciiName;
        }
    }
}
