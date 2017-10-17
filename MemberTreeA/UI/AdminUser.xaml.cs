/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/01/2017
 * 时间: 21:39
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
	/// Interaction logic for AdminUser.xaml
	/// </summary>
	public partial class AdminUser : UserControl
	{
		public AdminUser()
		{
			InitializeComponent();
		}
		
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if(UserAdmin.I != null)
			{
				checkIsEnableUser.IsChecked = UserAdmin.I.UserAdminEnabled;
				gpUserAdmin.IsEnabled = (bool)checkIsEnableUser.IsChecked;
				gpUserPrivilege.IsEnabled = (bool)checkIsEnableUser.IsChecked;
			}
		}

		//是否启用用户权限管理
		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			gpUserAdmin.IsEnabled = (bool)checkIsEnableUser.IsChecked;
			gpUserPrivilege.IsEnabled = (bool)checkIsEnableUser.IsChecked;
			UserAdmin.I.UserAdminEnabled = (bool)checkIsEnableUser.IsChecked;
		}
		
	}
}