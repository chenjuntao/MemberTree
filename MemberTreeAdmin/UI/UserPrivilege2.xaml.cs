/*
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
		private BtnDataset selectedDatasetBtn;
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
				BtnUserInfo cbx = new BtnUserInfo(user);
				cbx.MouseDown += cbxUser_Click;
				panelUser.Children.Add(cbx);
			}
		}
		
		public void RefreshDatasetList()
		{
			panelDataset.Children.Clear();
			List<DatasetInfo> dbList = MyTrees.treeDB.GetDatasets();
			foreach (DatasetInfo db in dbList) {
				BtnDataset btn = new BtnDataset(db);
				btn.MouseDown += btnDataset_Click;
				panelDataset.Children.Add(btn);
			}
		}
		
		//选中某个数据集时，可以修改该数据集对应的用户权限
		private void btnDataset_Click(object sender, RoutedEventArgs e)
		{
			BtnDataset btnDataset = sender as BtnDataset;
			if(selectedDatasetBtn != null)
			{
				selectedDatasetBtn.isSelected = false;
			}
			selectedDatasetBtn = btnDataset;
			btnDataset.isSelected = true;
			btnModify.IsEnabled = true;
			
			//根据数据集名字获取对应的用户权限
			List<string> allowUser = UserAdmin.GetAllowUserByData(btnDataset.DatasetName);
			foreach (BtnUserInfo cbx in panelUser.Children)
			{
				cbx.isSelected = (allowUser.Contains(cbx.UserId));
			}
		}
		
		//点击某个用户时，自动改变其选中状态
		private void cbxUser_Click(object sender, RoutedEventArgs e)
		{
			BtnUserInfo cbx = sender as BtnUserInfo;
			cbx.isSelected = !cbx.isSelected;
		}
		
		
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			btnSelectAll.IsEnabled = true;
			btnSelectNone.IsEnabled = true;
			btnSave.IsEnabled = true;
			btnCancel.IsEnabled = true;
			panelUser.IsEnabled =true;
			panelDataset.IsEnabled = false;
			btnModify.IsEnabled = false;
		}
		
		void BtnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (BtnUserInfo cbx in panelUser.Children)
			{
				cbx.isSelected = true;
			}
		}
		
		void BtnSelectNone_Click(object sender, RoutedEventArgs e)
		{
			foreach (BtnUserInfo cbx in panelUser.Children)
			{
				cbx.isSelected = false;
			}
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			//根据数据集名字获取对应的用户权限
			string dsName = selectedDatasetBtn.DatasetName;
			List<string> allowUser = UserAdmin.GetAllowUserByData(dsName);
			foreach (BtnUserInfo cbx in panelUser.Children)
			{
				string usrId = cbx.UserId;
				if((bool)cbx.isSelected)
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
			btnCancel.IsEnabled = false;
			panelUser.IsEnabled = false;
			panelDataset.IsEnabled = true;
			btnModify.IsEnabled = true;
			WindowAdmin.notify.SetStatusMessage(string.Format("成功修改了数据集{0}的所对应的用户权限！", dsName));
		}
		
		void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			btnSelectAll.IsEnabled = false;
			btnSelectNone.IsEnabled = false;
			btnSave.IsEnabled = false;
			btnCancel.IsEnabled = false;
			panelDataset.IsEnabled = true;
			panelUser.IsEnabled = false;
			btnModify.IsEnabled = true;
			
			//根据数据集名字获取对应的用户权限
			List<string> allowUser = UserAdmin.GetAllowUserByData(selectedDatasetBtn.DatasetName);
			foreach (BtnUserInfo cbx in panelUser.Children)
			{
				cbx.isSelected = (allowUser.Contains(cbx.UserId));
			}
		}
	}
}