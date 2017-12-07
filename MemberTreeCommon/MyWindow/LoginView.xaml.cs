/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 12/07/2017
 * 时间: 11:08
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
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
	/// Interaction logic for WindowLogin.xaml
	/// </summary>
	public partial class LoginView : UserControl
	{
		private InvokeBoolDelegate startupDelegate;
		
		public LoginView()
		{
			InitializeComponent();
		}
		
		//显示修改数据库窗体
        public void InitSelectDB(bool isAdmin, InvokeBoolDelegate startupDelegate)
        {
        	welcomeView.Init(isAdmin);
            this.startupDelegate = startupDelegate;
        }
        
		void BtnSqlite_Click(object sender, RoutedEventArgs e)
		{
			startupDelegate.Invoke(true);
		}
		
		void BtnMysql_Click(object sender, RoutedEventArgs e)
		{
			startupDelegate.Invoke(false);
		}
	}
}