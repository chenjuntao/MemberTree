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
			UserAdmin.InitDB(MyTrees.treeDB);

			ckEnable.IsChecked = UserAdmin.UserAdminEnabled;
			gridContent.IsEnabled = (bool)ckEnable.IsChecked;
			toolBar.IsEnabled = (bool)ckEnable.IsChecked;
		}

		//是否启用用户权限管理
		void CkEnable_Click(object sender, RoutedEventArgs e)
		{
			if((bool)ckEnable.IsChecked)
			{
				gridContent.IsEnabled = true;
				toolBar.IsEnabled = true;
				UserAdmin.UserAdminEnabled = true;
				WindowAdmin.notify.SetStatusMessage("已经将用户权限管理功能打开！");
			}
			else
			{
				gridContent.IsEnabled = false;
				toolBar.IsEnabled = false;
				UserAdmin.UserAdminEnabled = false;
				WindowAdmin.notify.SetStatusMessage("已经将用户权限管理功能关闭！");
			}
		}
		
		//切换页面
		void switchTabPage_Checked(object sender, RoutedEventArgs e)
		{
			if((bool)btnUserAdmin.IsChecked)
			{
				gridContent.Children.Clear();
				UserInfoSet user = new UserInfoSet();
				gridContent.Children.Add(user);
				user.RefreshUserList();
				WindowAdmin.notify.SetStatusMessage("开始进行用户基本信息管理！");
			}
			else if((bool)btnUsrDst.IsChecked)
			{
				gridContent.Children.Clear();
				UserPrivilege1 us2ds = new UserPrivilege1();
				gridContent.Children.Add(us2ds);
				WindowAdmin.notify.SetStatusMessage("开始进行用户——>数据集权限管理！");
			}
			else if((bool)btnDstUsr.IsChecked)
			{
				gridContent.Children.Clear();
				UserPrivilege2 ds2us = new UserPrivilege2();
				gridContent.Children.Add(ds2us);
				WindowAdmin.notify.SetStatusMessage("开始进行数据集——>用户权限管理！");
			}
		}

	}
}