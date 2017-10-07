/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/9/12
 * 时间: 9:03
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for WindowColsCheck.xaml
	/// </summary>
	public partial class WindowColsCheck : Window
	{
		public WindowColsCheck(string[] heads)
		{
			InitializeComponent();
			
			if(heads.Length > 2)
			{
				btnName.Content = "会员姓名：" + heads[2];
				if(heads.Length > 1)
				{
					btnTopid.Content = "上级会员ID：" + heads[1];
					if(heads.Length > 0)
					{
						btnSysid.Content = "会员ID：" + heads[0];
					}
				}
			}
			
			if(heads.Length<3)
			{
				txtMessage.Text = "文件格式不正确，最少必须包含三列，前三列为“会员id,上级会员id,会员姓名”且顺序固定，请重新选择正确的文件！";
				btnOK.Visibility = Visibility.Collapsed;
			}
			else
			{
				grpOptCols.Header = "可选列（共" + (heads.Length-3) + "列）";
				for (int i = 3; i < heads.Length; i++) {
					Button btn = new Button();
					btn.Content = heads[i];
					optColsPanel.Children.Add(btn);
				}
			}
		}
		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}