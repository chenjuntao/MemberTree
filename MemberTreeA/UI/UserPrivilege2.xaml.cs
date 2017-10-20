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
	public partial class UserPrivilege2 : UserControl
	{
		private Button selectedDatasetBtn;
		private List<string> addUserPrivileges = new List<string>();
		private List<string> removeUserPrivileges = new List<string>();
		public UserPrivilege2()
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
				CheckBox cbx = new CheckBox();
				cbx.Margin = new Thickness(5);
				cbx.Content = user.ToShortString();
				cbx.Tag = user.ID;
				cbx.ToolTip = user.Remark;
				panelUser.Children.Add(cbx);
			}
		}
		
		public void RefreshDatasetList()
		{
			panelDataset.Children.Clear();
			List<DatasetInfo> dbList = MyTrees.treeDB.GetDatasets();
			foreach (DatasetInfo db in dbList) {
				Button btn = new Button();
				btn.Background = Brushes.Azure;
				btn.Margin = new Thickness(3);
				btn.Width = 150;
				btn.Height = 30;
				btn.Content = db.Name;
				btn.ToolTip = db.GetOtherString();
				btn.Click += btnDataset_Click;
				panelDataset.Children.Add(btn);
			}
		}
		
		//选中某个数据集时，可以修改该数据集对应的用户权限
		private void btnDataset_Click(object sender, RoutedEventArgs e)
		{
			Button btnDataset = sender as Button;
			if(selectedDatasetBtn != null)
			{
				selectedDatasetBtn.Background = Brushes.Azure;
			}
			selectedDatasetBtn = btnDataset;
			btnDataset.Background = Brushes.LightBlue;
			btnModify.IsEnabled = true;
			
			//根据数据集名字获取对应的用户权限
			List<string> allowUser = UserAdmin.GetAllowUserByData(btnDataset.Content.ToString());
			foreach (CheckBox cbx in panelUser.Children)
			{
				cbx.IsChecked = (allowUser.Contains(cbx.Tag.ToString()));
			}
		}
		
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			btnSelectAll.IsEnabled = true;
			btnSelectNone.IsEnabled = true;
			btnSave.IsEnabled = true;
			panelUser.IsEnabled =true;
			panelDataset.IsEnabled = false;
			btnModify.IsEnabled = false;
		}
		
		void BtnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (CheckBox cbx in panelUser.Children)
			{
				cbx.IsChecked = true;
			}
		}
		
		void BtnSelectNone_Click(object sender, RoutedEventArgs e)
		{
			foreach (CheckBox cbx in panelUser.Children)
			{
				cbx.IsChecked = false;
			}
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			//根据数据集名字获取对应的用户权限
			string dsName = selectedDatasetBtn.Content.ToString();
			List<string> allowUser = UserAdmin.GetAllowUserByData(dsName);
			foreach (CheckBox cbx in panelUser.Children)
			{
				string usrId = cbx.Tag.ToString();
				if((bool)cbx.IsChecked)
				{
					if(!allowUser.Contains(usrId))
					{
						UserAdmin.AddUserPrivilege(usrId, dsName);
					}
				}
				else
				{
					if(allowUser.Contains(usrId))
					{
						UserAdmin.DeleteUserPrivilege(usrId, dsName);
					}
				}
			}
			
			btnSelectAll.IsEnabled = false;
			btnSelectNone.IsEnabled = false;
			btnSave.IsEnabled = false;
			panelUser.IsEnabled = false;
			panelDataset.IsEnabled = true;
			btnModify.IsEnabled = true;
			WindowAdmin.notify.SetStatusMessage(string.Format("成功修改了数据集{0}的所对应的用户权限！", dsName));
		}
		
	}
}