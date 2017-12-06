/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/12/6
 * 时间: 15:44
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace SoftUnRegister
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			RegistryKey retkey = Registry.CurrentUser.OpenSubKey("software").OpenSubKey("MemTree");
			if(retkey==null)
			{
				txtMsg.Text = "当前机器没有注册，不需要清理！";
				btnClear.IsEnabled = false;
			}
			else
			{
				txtMsg.Text = "";
			}
		}
		
		//清理当前机器上的注册信息，仅供测试使用
		private void BtnClearReg_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult reslut = MessageBox.Show("确定要清除本机的注册信息吗？","清理确认", MessageBoxButton.OKCancel);
			if(reslut == MessageBoxResult.OK)
			{
				Registry.CurrentUser.OpenSubKey("software", true).DeleteSubKeyTree("MemTree");
				txtMsg.Text = "当前机器注册信息清理完毕！";
				btnClear.IsEnabled = false;
			}
		}
	}
}