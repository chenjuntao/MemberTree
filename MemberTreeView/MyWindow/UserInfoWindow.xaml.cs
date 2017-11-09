/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 10/24/2017
 * 时间: 11:23
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

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for UserInfo.xaml
	/// </summary>
	public partial class UserInfoWindow : Window
	{
		public UserInfoWindow(string userId)
		{
			InitializeComponent();
			this.DataContext = UserAdmin.GetUserInfo(userId);
		}
		
		void ButtonModifyPwd_Click(object sender, RoutedEventArgs e)
		{
			grpModifyPwd.Visibility = Visibility.Visible;
			grpRemark.Visibility = Visibility.Hidden;
			btnModiryPwd.Visibility = Visibility.Hidden;
			txtOldPwd.Clear();
			txtPwd1.Clear();
			txtPwd2.Clear();
		}
		
		void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			UserInfo userInfo = UserAdmin.GetUserInfo(txtUserId.Text);
			if(txtOldPwd.Password != EncryptHelper.Decrypt(userInfo.Pwd))
			{
				MessageBox.Show("输入的旧密码不正确！");
				return;
			}
			else if(txtPwd1.Password == "" || txtPwd2.Password == "")
			{
				MessageBox.Show("输入的新密码不能为空！");
				return;
			}
			else if(txtPwd1.Password != txtPwd2.Password)
			{
				MessageBox.Show("两次输入的新密码不一致，请重新输入！");
				return;
			}
			else
			{
				UserAdmin.UpdateUserPwd(txtUserId.Text, EncryptHelper.Encrypt(txtPwd1.Password));
				grpModifyPwd.Visibility = Visibility.Hidden;
				grpRemark.Visibility = Visibility.Visible;
				btnModiryPwd.Visibility = Visibility.Visible;
			}
		}
		
		void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			grpModifyPwd.Visibility = Visibility.Hidden;
			grpRemark.Visibility = Visibility.Visible;
			btnModiryPwd.Visibility = Visibility.Visible;
		}
		
	}
}