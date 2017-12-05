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
		
        private static string diskStr = "";
        private static string GetDisk
        {
        	get{
        		if(diskStr == "")
        		{
					//获取当前系统磁盘符方法1，返回：C:
					string sysVolume = Environment.GetEnvironmentVariable("systemdrive");
		            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
		            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\""+ sysVolume +"\"");
		            disk.Get();
		            diskStr = disk.GetPropertyValue("VolumeSerialNumber").ToString();
        		}
        		return diskStr;
        	}
        }

        private static string cpuStr = "";
        private static string getCpu
        {
        	get{
	        	if(cpuStr == "")
	        	{
		            ManagementClass myCpu = new ManagementClass("win32_Processor");
		            ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
		            foreach (ManagementObject myObject in myCpuConnection)
		            {
		                cpuStr = myObject.Properties["Processorid"].Value.ToString();
		                break;
		            }
	        	}
	            return cpuStr;
        	}
        }

        //生成机器码
        public static string getMNum()
        {
            string strNum = getCpu + GetDisk;//机器码=Cpu序列号+硬盘序列号
            return strNum;
        }
        
//        Microsoft.Win32.RegistryKey retkey1 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software", true).CreateSubKey("WXK").CreateSubKey("WXK.INI");
//            foreach (string strName in retkey1.GetSubKeyNames())//判断注册码是否过期
//            {
//                if (strName == textBox3.Text)//如果注册表中已经存在
//                {
//                    MessageBox.Show("此注册码已经过期");
//                    return;
//                }
//            }
//            //在注册表中创建子项
//            Microsoft.Win32.RegistryKey retkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software", true).CreateSubKey("WXK").CreateSubKey("WXK.INI").CreateSubKey(textBox3.Text.TrimEnd());
//            retkey.SetValue("UserName", textBox2.Text);//向注册表中写入公司名称
//            retkey.SetValue("capataz", textBox1.Text);//向注册表中写入用户名称
//            retkey.SetValue("Code", textBox3.Text);//向注册表中写入注册码
        
        //判断是否已经注册
        public static bool hasReged()
        {
        	if(File.Exists("reg.dll"))
        	{
        		try 
        		{
        			string regMsg = EncryptHelper.FileDecrypt("reg.dll", GetDisk);
        			string[] regList = regMsg.Split(new String[]{getCpu}, StringSplitOptions.None);
        			if(regList.Length == 3)
        			{
        				if(RSAHelper.DecryptString(regList[0]) == GetDisk)
        				{
		        			Com = RSAHelper.DecryptString(regList[1]);
		        			Usr = RSAHelper.DecryptString(regList[2]);
		        			return true;
        				}
        				else
        				{
        					MessageBox.Show("注册密钥无效！");
        				}
        			}
        			else
        			{
        				MessageBox.Show("注册密钥无效！");
        			}
        		} 
        		catch (Exception ex)
        		{
        			MessageBox.Show("注册密钥无效！\n"+ex.Message);
        		}
        	}
        	return false;
        }
        
        //生成注册信息文件
        public static void getRegInfo(string filePath, string com, string usr)
        {
        	try 
        	{
        		string cpu = RSAHelper.EncryptString(getCpu);
        		com = RSAHelper.EncryptString(com);
	        	usr = RSAHelper.EncryptString(usr);
	        	string regMsg = GetDisk + cpu + GetDisk + com + GetDisk + usr + GetDisk;
	        	EncryptHelper.FileEncrypt(filePath, regMsg);
	        	MessageBox.Show("生成注册信息文件成功！\n");
        	} 
        	catch (Exception ex)
        	{
        		MessageBox.Show("生成注册信息文件失败！\n"+ex.Message);
        	}
        }

        //----------------------------------------------------------------------------------------------------------------
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
