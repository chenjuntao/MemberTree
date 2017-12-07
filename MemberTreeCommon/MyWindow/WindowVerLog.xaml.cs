/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 09/20/2017
 * 时间: 14:15
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for WindowVerLog.xaml
	/// </summary>
	public partial class WindowVerLog : Window
	{
		private Dictionary<string, string> verLogs = new Dictionary<string, string>();
		
		public WindowVerLog(bool isAdmin)
		{
			InitializeComponent();
			
			welcomeView.Init(isAdmin);
			ReadVerLog();
			foreach (string ver in verLogs.Keys) 
			{
				TabItem item = new TabItem();
				item.Header = ver;
				verTab.Items.Add(item);
			}
		}
		
		private void VerTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabItem selectItem = verTab.SelectedItem as TabItem;
			string selectVer = selectItem.Header.ToString();
			if(verLogs.ContainsKey(selectVer))
			{
				verContent.Text = verLogs[selectVer];
			}
		}
		
		//读取日志历史文件
		private void ReadVerLog()
		{
			string logFile = "verlog.dll";
			if(File.Exists(logFile))
			{
				using(StreamReader mysr = new StreamReader(logFile, Encoding.UTF8))
				{
					string verHead = "";
					string verContent = "";
					while(!mysr.EndOfStream)
	                {
						string line = mysr.ReadLine();
						if(line.StartsWith("ver"))
						{
							if(verContent != "" && !verLogs.ContainsKey(verHead))
							{
								verLogs.Add(verHead, verContent);
								verContent = "";
							}
							verHead = line;
						}
						else
						{
							verContent += Environment.NewLine + line;
						}
					}
				}
			}
		}
	}
}