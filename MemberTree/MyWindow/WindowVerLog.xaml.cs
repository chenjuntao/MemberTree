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

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for WindowVerLog.xaml
	/// </summary>
	public partial class WindowVerLog : Window
	{
		public WindowVerLog(bool isAdmin)
		{
			InitializeComponent();
			this.welcomView.InitVerLog(isAdmin);
		}
	}
}