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
		
		public void InitUserAdmin()
		{
			UserAdmin.InitDB();

			rbEnable.IsChecked = UserAdmin.UserAdminEnabled;
			rbDisable.IsChecked = !UserAdmin.UserAdminEnabled;
			gpUserAdmin.IsEnabled = (bool)rbEnable.IsChecked;
			gpUserPrivilege.IsEnabled = (bool)rbEnable.IsChecked;
			
			userInfoSet.RefreshUserList();
		}

		//是否启用用户权限管理
		void RadioBtnEnable_Click(object sender, RoutedEventArgs e)
		{
			if((bool)rbEnable.IsChecked)
			{
				gpUserAdmin.IsEnabled = true;
				gpUserPrivilege.IsEnabled = true;
				UserAdmin.UserAdminEnabled = true;
				WindowAdmin.notify.SetStatusMessage("已经将用户权限管理功能打开！");
			}
			else
			{
				gpUserAdmin.IsEnabled = false;
				gpUserPrivilege.IsEnabled = false;
				UserAdmin.UserAdminEnabled = false;
				WindowAdmin.notify.SetStatusMessage("已经将用户权限管理功能关闭！");
			}
		}
	}
}