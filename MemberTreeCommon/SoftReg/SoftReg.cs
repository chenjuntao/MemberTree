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
using Microsoft.Win32;
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
        
        //对注册密钥文件进行解密
		public static bool DecryptRegKey(string file)
		{
			try 
			{
				string regMsg = EncryptHelper.FileDecrypt(file);
				string[] regList = regMsg.Split(new String[]{getCpu}, StringSplitOptions.RemoveEmptyEntries);
				string mNum = RSAHelper.DecryptString(regList[0]);
				if(mNum == getMNum())
				{
					Com = RSAHelper.DecryptString(regList[1]);
					Usr = RSAHelper.DecryptString(regList[2]);
					return true;
				}
				else
				{
					MessageBox.Show("注册密钥机器码不正确！");
					return false;
				}
			} 
			catch (Exception ex)
        	{
        		MessageBox.Show("解析注册密钥文件失败！\n"+ex.Message);
        		return false;
        	}
		}
		
		//安装注册密钥
		public static bool InstallRegKey(string file)
		{
			try 
			{
				string regMsg = EncryptHelper.FileDecrypt(file);
				string[] regList = regMsg.Split(new String[]{getCpu}, StringSplitOptions.RemoveEmptyEntries);
				RegistryKey retkey = Registry.CurrentUser.OpenSubKey("software", true).CreateSubKey("MemTree");
				retkey.SetValue("MNum", regList[0]);
				retkey.SetValue("Com", regList[1]);
				retkey.SetValue("Usr", regList[2]);
				for (int i = 3; i < regList.Length; i++) 
				{
					string[] retItems = regList[i].Split(new String[]{GetDisk}, StringSplitOptions.RemoveEmptyEntries);
					RegistryKey retkeySql = retkey.CreateSubKey("SQL");
					retkeySql.SetValue(retItems[0], retItems[1]);
				}
				return true;
			} 
			catch (Exception ex)
        	{
        		MessageBox.Show("安装注册密钥失败！\n"+ex.Message);
        		return false;
        	}
		}
		
		//判断是否已经注册
        public static bool hasReged()
        {
        	try 
        	{
	        	RegistryKey retkey = Registry.CurrentUser.OpenSubKey("software").OpenSubKey("MemTree");
	        	if(retkey != null)
	        	{
	        		string mNum = retkey.GetValue("MNum").ToString();
	        		string com = retkey.GetValue("Com").ToString();
	        		string usr = retkey.GetValue("Usr").ToString();
	        		if(mNum!=null && com!=null && usr!=null)
	        		{
	        			Com = RSAHelper.DecryptString(com);
	        			Usr = RSAHelper.DecryptString(usr);
	        			mNum = RSAHelper.DecryptString(mNum);
	        			if(mNum == getMNum())
	        			{
	        				RegistryKey retkeySql = retkey.OpenSubKey("SQL");
	        				if(retkeySql!=null)
	        				{
	        					if(RegConfig.InitConfig(retkeySql))
	        					{
	        						return true;
	        					}
	        				}
	        			}
	        			else
	        			{
	        				MessageBox.Show("注册码不正确！");
	        			}
	        		}
	        	}
        	} 
        	catch (Exception ex)
        	{
        		MessageBox.Show("读取注册信息失败！\n"+ex.Message);
        	}
        	
        	return false;
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
