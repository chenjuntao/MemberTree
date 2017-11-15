/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 10/23/2017
 * 时间: 16:09
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
using System.Windows.Media.Imaging;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for BtnUserInfo.xaml
	/// </summary>
	public partial class BtnUserInfo : UserControl
	{
		public BtnUserInfo(UserInfo user)
		{
			InitializeComponent();
			this.txtId.Text = user.ID;
			this.txtName.Text = user.Name;
			this.ToolTip = user.Remark;
			if(!user.Enable)
			{
				txtStatus.Visibility = Visibility.Visible;
				mainGrid.Background = Brushes.LightGray;
			}
		}
		
		public string UserId
		{
			get{return txtId.Text;}
		}
		
		public bool isSelected
		{
			get{return this.BorderThickness.Bottom == 1.0;;}
			set
			{
				if(value)
				{
					this.BorderThickness = new Thickness(1);
					this.img.Source = new BitmapImage(new Uri("/MemberTreeCommon;component/Image/user1.png", UriKind.Relative));
				}
				else
				{
					this.BorderThickness = new Thickness(0);
					this.img.Source = new BitmapImage(new Uri("/MemberTreeCommon;component/Image/user.png", UriKind.Relative));
				}
			}
		}
	}
}