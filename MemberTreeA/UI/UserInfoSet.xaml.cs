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
using System.Globalization;
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
		
		private void SetEnabled(bool enabled, params Control[] controls)
		{
			foreach (Control control in controls) 
			{
				control.IsEnabled = enabled;
			}
		}
		
		private bool CheckTxtIsNotNull(TextBox txtBox, string txtTip)
		{
			if(txtBox.Text == "")
			{
				txtBox.BorderBrush = Brushes.Red;
				MessageBox.Show(txtTip + "不能为空！");
				WindowAdmin.notify.SetStatusMessage(txtTip + "不能为空！");
				return false;
			}
			else
			{
				txtBox.BorderBrush = Brushes.LightBlue;
				return true;
			}
		}
		
		//选择用户变化
		void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UserInfo userInfo = userList.SelectedItem as UserInfo;
			if(userInfo != null)
			{
				SetEnabled(true, btnModify, btnResetPwd, btnDelete);

				txtID.Text = userInfo.ID;
				txtName.Text = userInfo.Name;
				txtRemark.Text = userInfo.Remark;
				WindowAdmin.notify.SetStatusMessage("当前选中的用户ID："+userInfo.ID+"，用户姓名："+userInfo.Name);
			}
			else
			{
				SetEnabled(false, btnModify, btnResetPwd, btnDelete);
			}
		}
		
		//新增用户
		void BtnNew_Click(object sender, RoutedEventArgs e)
		{
			gridUserInfo.Visibility = Visibility.Visible;
			userList.IsEnabled = false;
			userList.SelectedIndex = -1;
			btnNew.IsEnabled = false;
			txtID.IsEnabled = true;
			txtID.Clear();
			txtName.Clear();
			txtRemark.Clear();
			txtPwd1.Clear();
			txtPwd2.Clear();
			WindowAdmin.notify.SetStatusMessage("开始新增用户。。。");
		}
		
		//修改用户信息
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			gridUserInfo.Visibility = Visibility.Visible;
			SetEnabled(false, btnNew, btnModify, btnResetPwd, btnDelete, userList);
			WindowAdmin.notify.SetStatusMessage("正在修改用户信息。。。");
		}
		
		//重置用户密码
		void BtnResetPwd_Click(object sender, RoutedEventArgs e)
		{
			gridUserPwd.Visibility = Visibility.Visible;
			SetEnabled(false, btnNew, btnModify, btnResetPwd, btnDelete, userList);
			WindowAdmin.notify.SetStatusMessage("正在重置用户密码。。。");
		}
		
		//保存用户信息
		void BtnSaveUser_Click(object sender, RoutedEventArgs e)
		{
			if(CheckTxtIsNotNull(txtID, "用户ID") && CheckTxtIsNotNull(txtName, "用户姓名"))
			{
				if(!txtID.IsEnabled) //修改现有的用户信息
				{
					UserAdmin.UpdateUserInfo(txtID.Text, txtName.Text, txtRemark.Text);
					RefreshUserList();
					gridUserInfo.Visibility = Visibility.Collapsed;
					userList.IsEnabled = true;
					btnNew.IsEnabled = true;
					WindowAdmin.notify.SetStatusMessage("修改用户信息成功!");
				}
				else if(UserAdmin.GetUserInfo(txtID.Text) != null) //首先判断ID是否已存在
				{
					txtID.BorderBrush = Brushes.Red;
					txtID.SelectAll();
					MessageBox.Show("当前用户ID在数据库中已存在，请使用其他ID！");
					WindowAdmin.notify.SetStatusMessage("当前用户ID在数据库中已存在，请使用其他ID！");
				}
				else //增加新的用户
				{
					gridUserInfo.Visibility = Visibility.Collapsed;
					gridUserPwd.Visibility = Visibility.Visible;
				}
			}
		}
		
		//修改用户密码
		void BtnSavePwd_Click(object sender, RoutedEventArgs e)
		{
			if(txtPwd1.Password == txtPwd2.Password && txtPwd1.Password != "")
			{
				if(txtID.IsEnabled)
				{
					UserInfo userInfo = new UserInfo(txtID.Text, txtName.Text, EncryptHelper.Encrypt(txtPwd1.Password), txtRemark.Text);
					UserAdmin.AddUserInfo(userInfo);
					RefreshUserList();
					txtID.IsEnabled = false;
					WindowAdmin.notify.SetStatusMessage("增加新的用户成功!");
				}
				else
				{
					UserAdmin.UpdateUserPwd(txtID.Text, EncryptHelper.Encrypt(txtPwd1.Password));
					SetEnabled(true, btnModify, btnResetPwd, btnDelete);
					WindowAdmin.notify.SetStatusMessage("重置用户密码成功!");
				}
				gridUserPwd.Visibility = Visibility.Collapsed;
				userList.IsEnabled = true;
				btnNew.IsEnabled = true;
			}
			else
			{
				MessageBox.Show("两次输入的密码不一致！");
				WindowAdmin.notify.SetStatusMessage("两次输入的密码不一致!");
			}
		}
		
		//取消保存用户信息或修改用户密码
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			if(txtID.IsEnabled) //增加新的用户
			{
				txtID.IsEnabled = false;
				WindowAdmin.notify.SetStatusMessage("已经取消新增用户！");
			}
			else  //修改现有的用户信息
			{
				SetEnabled(true, btnModify, btnResetPwd, btnDelete);
				WindowAdmin.notify.SetStatusMessage("已经取消修改用户信息！");
			}
			Button btn = sender as Button;
			Grid grid = btn.Parent as Grid;
			grid.Visibility = Visibility.Collapsed;
			userList.IsEnabled = true;
			btnNew.IsEnabled = true;
		}
		
		//修改是否启用
		void Enable_Check(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = e.Source as CheckBox;
			if(checkBox != null)
			{
				UserAdmin.UpdateUserEnabled(checkBox.Tag.ToString(), (bool)checkBox.IsChecked);
				if((bool)checkBox.IsChecked)
				{
					WindowAdmin.notify.SetStatusMessage("已经将用户启用!");
				}
				else
				{
					WindowAdmin.notify.SetStatusMessage("已经将用户停用!");
				}
			}
		}
		
		//删除
		void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			string tipTxt = "确定要删除该用户?\n用户ID："+txtID.Text+"，用户姓名："+txtName.Text;
			if(MessageBox.Show(tipTxt, "删除确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				UserAdmin.DeleteUserInfo(txtID.Text);
				RefreshUserList();
				WindowAdmin.notify.SetStatusMessage("删除用户成功！被删除的用户ID："+txtID.Text+"，用户姓名："+txtName.Text);
			}
		}
	}
}