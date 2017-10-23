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
using System.Windows.Media.Imaging;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for UserPrivilege.xaml
	/// </summary>
	public partial class UserPrivilege1 : UserControl
	{
		private BtnUserInfo selectedUserBtn;
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
				BtnUserInfo btn = new BtnUserInfo(user);
				btn.MouseDown += btnUser_Click;
				panelUser.Children.Add(btn);
			}
		}
		
		public void RefreshDatasetList()
		{
			panelDataset.Children.Clear();
			List<DatasetInfo> dbList = MyTrees.treeDB.GetDatasets();
			foreach (DatasetInfo db in dbList) {
				BtnDataset cbx = new BtnDataset(db);
				cbx.MouseDown += cbxDataset_Click;
				panelDataset.Children.Add(cbx);
			}
		}
		
		//选中某个用户时，可以修改该用户对应的数据集权限
		private void btnUser_Click(object sender, RoutedEventArgs e)
		{
			BtnUserInfo btnUser = sender as BtnUserInfo;
			if(selectedUserBtn != null)
			{
				selectedUserBtn.isSelected = false;
			}
			selectedUserBtn = btnUser;
			btnUser.isSelected = true;
			btnModify.IsEnabled = true;
			
			//根据用户ID获取对应的数据权限
			List<string> allowData = UserAdmin.GetAllowDataByUser(btnUser.UserId);
			foreach (BtnDataset cbx in panelDataset.Children)
			{
				cbx.isSelected = (allowData.Contains(cbx.DatasetName));
			}
		}
		
		//点击某个数据集时，自动改变其选中状态
		private void cbxDataset_Click(object sender, RoutedEventArgs e)
		{
			BtnDataset cbx = sender as BtnDataset;
			cbx.isSelected = !cbx.isSelected;
		}
		
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			btnSelectAll.IsEnabled = true;
			btnSelectNone.IsEnabled = true;
			btnSave.IsEnabled = true;
			btnCancel.IsEnabled = true;
			panelDataset.IsEnabled = true;
			panelUser.IsEnabled = false;
			btnModify.IsEnabled = false;
		}
		
		void BtnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (BtnDataset cbx in panelDataset.Children)
			{
				cbx.isSelected = true;
			}
		}
	
		void BtnSelectNone_Click(object sender, RoutedEventArgs e)
		{
			foreach (BtnDataset cbx in panelDataset.Children)
			{
				cbx.isSelected = false;
			}
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			//根据用户ID获取对应的数据权限
			string usrId = selectedUserBtn.UserId;
			List<string> allowData = UserAdmin.GetAllowDataByUser(usrId);
			foreach (BtnDataset cbx in panelDataset.Children)
			{
				string dsName = cbx.DatasetName;
				if((bool)cbx.isSelected)
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
			btnCancel.IsEnabled = false;
			panelDataset.IsEnabled = false;
			panelUser.IsEnabled = true;
			btnModify.IsEnabled = true;
			WindowAdmin.notify.SetStatusMessage(string.Format("成功修改了用户{0}的所对应的数据集权限！", usrId));
		}
		
		void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			btnSelectAll.IsEnabled = false;
			btnSelectNone.IsEnabled = false;
			btnSave.IsEnabled = false;
			btnCancel.IsEnabled = false;
			panelDataset.IsEnabled = false;
			panelUser.IsEnabled = true;
			btnModify.IsEnabled = true;
			
			//根据用户ID获取对应的数据权限
			List<string> allowData = UserAdmin.GetAllowDataByUser(selectedUserBtn.UserId);
			foreach (BtnDataset cbx in panelDataset.Children)
			{
				cbx.isSelected = (allowData.Contains(cbx.DatasetName));
			}
		}
	}
}