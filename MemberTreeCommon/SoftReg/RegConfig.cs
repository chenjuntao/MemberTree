/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 2017/12/5
 * 时间: 18:46
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using RSACommon;

namespace MemberTree
{
	/// <summary>
	/// Description of RegConfig.
	/// </summary>
	public static class RegConfig
	{
		public static Dictionary<string, string> config = new Dictionary<string, string>();
		
		public static bool InitConfig(RegistryKey retkey)
		{
			try 
        	{
				string[] retSqlNames = retkey.GetValueNames();
				foreach (string retKey in retSqlNames) 
				{
					string retVal = retkey.GetValue(retKey).ToString();
					string key = RSAHelper.DecryptString(retKey);
					string val = RSAHelper.DecryptString(retVal);
					config.Add(key, val);
				}
				return true;
			}
        	catch (Exception ex)
        	{
        		MessageBox.Show("读取注册子信息列表失败！\n"+ex.Message);
        	}
        	
        	return false;
		}
	}
}
