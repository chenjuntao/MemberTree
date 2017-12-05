/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/11/30
 * 时间: 11:12
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MemberTree;
using Microsoft.Win32;
using RSACommon;

namespace SoftRegister
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private string cpu, disk, com, usr;
		private string publicKey ="AwEAAdRuUg26vNnnSLp1JRPsDWQWjr/S+fX67Blv2Er5XiIuksPWbBq9L7WpcPN2yiiQdlOlqhLgMigKaDaHwRp2ob8y2aCCja1Vi3nZymFK23h9wWwdfPuV0vfnuQ74EcF7K6vOTw6iOcaOUTvDe3tZuS9raCgdfaLrPKzwotc0Jn31";
		private string privateKey = "gBwg95CF15fq/kBiXqSCr0s/iWtxHlQqA7Vij/tthb90904jSHFJ99VQOHqkkiRI7MIqv5h8Q2f16NK/qxw79TLEJZOYqH6l+EUzS/kOvGKDJ1zUEZqhNCye+J7X4/hfPCSy9fGIkDXfSkAiYPZpDM9QPA6Drj7VcL3jvfA2ZwsJ1G5SDbq82edIunUlE+wNZBaOv9L59frsGW/YSvleIi6Sw9ZsGr0vtalw83bKKJB2U6WqEuAyKApoNofBGnahvzLZoIKNrVWLednKYUrbeH3BbB18+5XS9+e5DvgRwXsrq85PDqI5xo5RO8N7e1m5L2toKB19ous8rPCi1zQmffU=";
		public Window1()
		{
			InitializeComponent();
			RegConfig.Init();
		}
		
		private void BtnBrowser_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "选择需要进行注册的用户文件(*.reginfo)";
            openfileDlg.Filter = "注册信息文件|*.reginfo";
            if (openfileDlg.ShowDialog() == true)
            {
            	txtRegInfo.Text = openfileDlg.FileName;
            	if(DecryptRegInfo(openfileDlg.FileName))
            	{
            		txtRegMsg.Text = string.Format("授权给（公司/单位：{0}，用户：{1}）", com, usr);
            		txtRegKey.Text = openfileDlg.FileName.Replace(".reginfo", ".regkey");
            		btnOK.IsEnabled = true;
            	}
            	else
            	{
            		txtRegMsg.Text = "注册信息文件不正确！";
            		txtRegKey.Text = "";
            		btnOK.IsEnabled = false;
            	}
            }
		}
		
		//对注册信息文件进行解密
		private bool DecryptRegInfo(string file)
		{
			string regMsg = EncryptHelper.FileDecrypt(file);
        	int subIndex = 1;
        	disk = regMsg.Substring(0, subIndex);
        	while(ContainsTimes(regMsg, disk) >= 4)
        	{
        		subIndex++;
        		disk = regMsg.Substring(0, subIndex);
        	}
        	disk = regMsg.Substring(0, subIndex-1);
        	if(ContainsTimes(regMsg, disk) == 4)
        	{
        		string[] regList = regMsg.Split(new String[]{disk}, StringSplitOptions.RemoveEmptyEntries);
        		cpu = RSAHelper.DecryptString(regList[0], privateKey);
        		com = RSAHelper.DecryptString(regList[1], privateKey);
        		usr = RSAHelper.DecryptString(regList[2], privateKey);
        		return true;
        	}
        	return false;
		}

		//判断字符串包含次数
		private int ContainsTimes(string MainStr, string subStr)
		{
			Regex ex = new Regex(subStr);
			return ex.Matches(MainStr).Count;
		}

		//生成注册密钥文件
		private void BtnOK_Click(object sender, RoutedEventArgs e)
		{
			try 
        	{
				//加密处理生成密钥文件*.regkey
				List<string> regList = new List<string>();
				regList.Add(RSAHelper.EncryptString(cpu+disk, privateKey));
				regList.Add(RSAHelper.EncryptString(com, privateKey));
				regList.Add(RSAHelper.EncryptString(usr, privateKey));
	        	foreach (string confKey in RegConfig.config.Keys) 
	        	{
	        		string key = RSAHelper.EncryptString(confKey, privateKey);
	        		string val = RSAHelper.EncryptString(RegConfig.config[confKey], privateKey);
	        		regList.Add(key + disk + val);
	        	}
	        	string regMsg = string.Join(cpu, regList);
	        	EncryptHelper.FileEncrypt(txtRegKey.Text, regMsg);
	        	MessageBox.Show("生成注册信息文件成功！\n");
        	} 
        	catch (Exception ex)
        	{
        		MessageBox.Show("生成注册信息文件失败！\n"+ex.Message);
        	}
		}
	}
}