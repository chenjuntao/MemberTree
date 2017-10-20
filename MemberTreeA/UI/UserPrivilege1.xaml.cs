﻿/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/16/2017
 * 时间: 21:49
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
	/// Interaction logic for UserPrivilege.xaml
	/// </summary>
	public partial class UserPrivilege1 : UserControl
	{
		private Button selectedUserBtn;
		private List<string> addDataPrivileges = new List<string>();
		private List<string> removeDataPrivileges = new List<string>();
		public UserPrivilege1()
		{
			InitializeComponent();
			
			RefreshUserList();
			RefreshDatasetList();
		}
		
		public void RefreshUserList()
		{
			panelUser.Children.Clear();
			List<UserInfo> userList = UserAdmin.GetUserInfoList();
			foreach (UserInfo user in userList) {
				Button btn = new Button();
				btn.Background = Brushes.Azure;
				btn.Margin = new Thickness(3);
				btn.Width = 150;
				btn.Height = 30;
				btn.Content = user.ToShortString();
				btn.Tag = user.ID;
				btn.ToolTip = user.Remark;
				btn.Click += btnUser_Click;
				panelUser.Children.Add(btn);
			}
		}
		
		public void RefreshDatasetList()
		{
			panelDataset.Children.Clear();
			List<DatasetInfo> dbList = MyTrees.treeDB.GetDatasets();
			foreach (DatasetInfo db in dbList) {
				CheckBox cbx = new CheckBox();
				cbx.Margin = new Thickness(5);
				cbx.Content = db.Name;
				cbx.ToolTip = db.GetOtherString();	
				panelDataset.Children.Add(cbx);
			}
		}
		
		//选中某个用户时，可以修改该用户对应的数据集权限
		private void btnUser_Click(object sender, RoutedEventArgs e)
		{
			Button btnUser = sender as Button;
			if(selectedUserBtn != null)
			{
				selectedUserBtn.Background = Brushes.Azure;
			}
			selectedUserBtn = btnUser;
			btnUser.Background = Brushes.LightBlue;
			btnModify.IsEnabled = true;
			
			//根据用户ID获取对应的数据权限
			List<string> allowData = UserAdmin.GetAllowDataByUser(btnUser.Tag.ToString());
			foreach (CheckBox cbx in panelDataset.Children)
			{
				cbx.IsChecked = (allowData.Contains(cbx.Content.ToString()));
			}
		}
		
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			btnSelectAll.IsEnabled = true;
			btnSelectNone.IsEnabled = true;
			btnSave.IsEnabled = true;
			panelDataset.IsEnabled = true;
			panelUser.IsEnabled = false;
			btnModify.IsEnabled = false;
		}
		
		void BtnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (CheckBox cbx in panelDataset.Children)
			{
				cbx.IsChecked = true;
			}
		}
	
		void BtnSelectNone_Click(object sender, RoutedEventArgs e)
		{
			foreach (CheckBox cbx in panelDataset.Children)
			{
				cbx.IsChecked = false;
			}
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			//根据用户ID获取对应的数据权限
			string usrId = selectedUserBtn.Tag.ToString();
			List<string> allowData = UserAdmin.GetAllowDataByUser(usrId);
			foreach (CheckBox cbx in panelDataset.Children)
			{
				string dsName = cbx.Content.ToString();
				if((bool)cbx.IsChecked)
				{
					if(!allowData.Contains(dsName))
					{
						UserAdmin.AddUserPrivilege(usrId, dsName);
					}
				}
				else
				{
					if(allowData.Contains(dsName))
					{
						UserAdmin.DeleteUserPrivilege(usrId, dsName);
					}
				}
			}
			
			btnSelectAll.IsEnabled = false;
			btnSelectNone.IsEnabled = false;
			btnSave.IsEnabled = false;
			panelDataset.IsEnabled = false;
			panelUser.IsEnabled = true;
			btnModify.IsEnabled = true;
			WindowAdmin.notify.SetStatusMessage(string.Format("成功修改了用户{0}的所对应的数据集权限！", usrId));
		}
	}
}