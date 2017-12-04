/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 11/30/2017
 * 时间: 16:12
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for SoftRegWindow.xaml
	/// </summary>
	public partial class SoftRegWindow : UserControl
	{
		public SoftRegWindow()
		{
			InitializeComponent();
		}
		
		private void BtnRegInfo_Click(object sender, RoutedEventArgs e)
		{
			if(txtxCom.Text == "")
			{
				MessageBox.Show("公司/单位信息必须填写！");
				txtxCom.Focus();
			}
			else if(txtxUsr.Text == "")
			{
				MessageBox.Show("用户信息必须填写！");
				txtxUsr.Focus();
			}
			else
			{
				SaveFileDialog saveFileDlg = new SaveFileDialog();
			    saveFileDlg.Title = "选择将会员树导出为文件的位置";
				saveFileDlg.Filter = "待注册信息文件|*.reginfo";
			    saveFileDlg.FileName = Dns.GetHostName();
			    if (saveFileDlg.ShowDialog() == true)
			    {
			    	SoftReg.getRegInfo(saveFileDlg.FileName, txtxCom.Text, txtxUsr.Text);
			    }
			}
		}
		
		private void BtnBrowser_Click(object sender, RoutedEventArgs e)
		{
			
		}
		
		private void BtnRegKey_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}