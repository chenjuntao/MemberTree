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
	/// Interaction logic for UserInfoSet.xaml
	/// </summary>
	public partial class UserInfoSet : UserControl
	{
		public UserInfoSet()
		{
			InitializeComponent();
		}
		 
		public void RefreshUserList()
		{
			userList.ItemsSource = UserAdmin.GetUserInfoList();
		}
		
		private void ClearTxt()
		{
			txtID.Clear();
			txtID.BorderBrush = Brushes.LightBlue;
			txtName.Clear();
			txtName.BorderBrush = Brushes.LightBlue;
			txtPwd.Clear();
			txtPwd.BorderBrush = Brushes.LightBlue;
			txtRemark.Clear();
		}
		
		//选择用户变化
		void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UserInfo userInfo = userList.SelectedItem as UserInfo;
			if(userInfo != null)
			{
				btnModify.IsEnabled = true;
				btnDelete.IsEnabled = true;
				txtID.IsReadOnly = true;
				txtID.Text = userInfo.ID;
				txtName.Text = userInfo.Name;
				txtPwd.Password = EncryptHelper.Decrypt(userInfo.Pwd);
				txtRemark.Text = userInfo.Remark;
			}
			else
			{
				gridUserInfo.IsEnabled = false;
				checkEnable.IsChecked = true;
				btnSave.IsEnabled = true;
				ClearTxt();
			}
		}
		
		//新增用户
		void BtnNew_Click(object sender, RoutedEventArgs e)
		{
			userList.SelectedIndex = -1;
			gridUserInfo.IsEnabled = true;
			txtID.IsReadOnly = false;
			checkEnable.IsChecked = true;
			btnNew.IsEnabled = false;
			btnSave.IsEnabled = true;
			btnModify.IsEnabled = false;
			btnDelete.IsEnabled = false;
		}
		
		//修改
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			gridUserInfo.IsEnabled = true;
			btnNew.IsEnabled = false;
			btnSave.IsEnabled = true;
			btnModify.IsEnabled = false;
			btnDelete.IsEnabled = false;
		}
		
		//保存
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			#region 判断是否为空
			if(txtID.Text == "")
			{
				txtID.BorderBrush = Brushes.Red;
				MessageBox.Show("用户ID不能为空！");
				return;
			}
			else
			{
				txtID.BorderBrush = Brushes.LightBlue;
			}
			if(txtName.Text == "")
			{
				txtName.BorderBrush = Brushes.Red;
				MessageBox.Show("用户姓名不能为空！");
				return;
			}
			else
			{
				txtName.BorderBrush = Brushes.LightBlue;
			}
			if(txtPwd.Password == "")
			{
				txtPwd.BorderBrush = Brushes.Red;
				MessageBox.Show("用户密码不能为空！");
				return;
			}
			else
			{
				txtPwd.BorderBrush = Brushes.LightBlue;
			}
			#endregion
			
			if(txtID.IsReadOnly)
			{
				UserAdmin.UpdateUserInfo(txtID.Text, txtName.Text, EncryptHelper.Encrypt(txtPwd.Password), txtRemark.Text);
			}
			else if(UserAdmin.GetUserInfo(txtID.Text) != null)
			{
				txtID.BorderBrush = Brushes.Red;
				txtID.SelectAll();
				MessageBox.Show("当前用户ID在数据库中已存在，请使用其他ID！");
			} 
			else
			{
				UserInfo userInfo = new UserInfo();
				userInfo.ID = txtID.Text;
				userInfo.Name = txtName.Text;
				userInfo.Pwd = EncryptHelper.Encrypt(txtPwd.Password);
				userInfo.Remark = txtRemark.Text;
				userInfo.Status = checkEnable.IsEnabled ? "启用":"停用";
				userInfo.CreateDate = DateTime.Now;
				userInfo.LoginTimes = 0;
				userInfo.OnlineTime = 0;
				UserAdmin.AddUserInfo(userInfo);
			}
		
			gridUserInfo.IsEnabled = false;
			btnSave.IsEnabled = false;
			txtID.IsReadOnly = true;
			RefreshUserList();
		}
		
		//删除
		void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			ClearTxt();
		}
		void UserList_MouseEnter(object sender, MouseEventArgs e)
		{
			
		}
	}
}