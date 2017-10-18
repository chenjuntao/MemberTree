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
		
		private void ClearTxt()
		{
			txtID.Clear();
			txtID.BorderBrush = Brushes.LightBlue;
			txtName.Clear();
			txtName.BorderBrush = Brushes.LightBlue;
			txtPwd1.Clear();
			txtPwd1.BorderBrush = Brushes.LightBlue;
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
				txtID.IsEnabled = false;
				txtID.Text = userInfo.ID;
				txtName.Text = userInfo.Name;
				txtPwd1.Password = EncryptHelper.Decrypt(userInfo.Pwd);
				txtRemark.Text = userInfo.Remark;
			}
			else
			{
				gridUserInfo.IsEnabled = false;
				ClearTxt();
			}
		}
		
		//新增用户
		void BtnNew_Click(object sender, RoutedEventArgs e)
		{
			gridUserInfo.Visibility = Visibility.Visible;
			
			userList.SelectedIndex = -1;
			txtID.IsEnabled = true;
			btnNew.IsEnabled = false;
			btnModify.IsEnabled = false;
			btnDelete.IsEnabled = false;
		}
		
		//修改用户信息
		void BtnModify_Click(object sender, RoutedEventArgs e)
		{
			gridUserInfo.IsEnabled = true;
			btnNew.IsEnabled = false;
			btnModify.IsEnabled = false;
			btnDelete.IsEnabled = false;
		}
		
		//重置用户密码
		void BtnResetPwd_Click(object sender, RoutedEventArgs e)
		{
			gridUserPwd.Visibility = Visibility.Visible;
			gridUserInfo.IsEnabled = true;
			btnNew.IsEnabled = false;
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
			if(txtPwd1.Password == "")
			{
				txtPwd1.BorderBrush = Brushes.Red;
				MessageBox.Show("用户密码不能为空！");
				return;
			}
			else
			{
				txtPwd1.BorderBrush = Brushes.LightBlue;
			}
			#endregion
			
			if(!txtID.IsEnabled)
			{
				UserAdmin.UpdateUserInfo(txtID.Text, txtName.Text, EncryptHelper.Encrypt(txtPwd1.Password), txtRemark.Text);
			}
			else if(UserAdmin.GetUserInfo(txtID.Text) != null)
			{
				txtID.BorderBrush = Brushes.Red;
				txtID.SelectAll();
				MessageBox.Show("当前用户ID在数据库中已存在，请使用其他ID！");
			} 
			else
			{
				UserInfo userInfo = new UserInfo(txtID.Text, txtName.Text, EncryptHelper.Encrypt(txtPwd1.Password), txtRemark.Text);
				UserAdmin.AddUserInfo(userInfo);
			}
		
			gridUserInfo.IsEnabled = false;
			btnNew.IsEnabled = true;
			txtID.IsEnabled = false;
			RefreshUserList();
		}
		
		//是否启用
		void Enable_Check(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = e.Source as CheckBox;
			if(checkBox != null)
			{
				UserAdmin.UpdateUserEnabled(checkBox.Tag.ToString(), (bool)checkBox.IsChecked);
			}
		}
		
		//删除
		void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			ClearTxt();
		}
		
	}
	
/// <summary>  
/// Value converter between bool and Visibility  
/// </summary>  
[ValueConversion(typeof(bool), typeof(Visibility))]  
public class VisibilityConverter : IValueConverter  
{  
    #region IValueConverter Members  
    /// <summary>  
    /// Converts a value.  
    /// </summary>  
    /// <param name="value">The value produced by the binding source.</param>  
    /// <param name="targetType">The type of the binding target property.</param>  
    /// <param name="parameter">The converter parameter to use.</param>  
    /// <param name="culture">The culture to use in the converter.</param>  
    /// <returns>  
    /// A converted value. If the method returns null, the valid null value is used.  
    /// </returns>  
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)  
    {  
        return ((bool)value)? Visibility.Visible : Visibility.Hidden;
    }  
  
    /// <summary>  
    /// Converts a value back.  
    /// </summary>  
    /// <param name="value">The value that is produced by the binding target.</param>  
    /// <param name="targetType">The type to convert to.</param>  
    /// <param name="parameter">The converter parameter to use.</param>  
    /// <param name="culture">The culture to use in the converter.</param>  
    /// <returns>  
    /// A converted value. If the method returns null, the valid null value is used.  
    /// </returns>  
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)  
    {  
        return (Visibility)value == Visibility.Visible;  
    }  
    #endregion IValueConverter Members  
}  
}