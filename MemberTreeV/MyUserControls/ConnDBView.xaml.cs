/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 01/30/2015
 * 时间: 17:35
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Windows.Threading;

namespace MemberTree
{
	/// <summary>
	/// 连接到Mysql数据库
	/// </summary>
	public partial class ConnDBView : UserControl
	{
		private InvokeDelegate startupDelegate;
		private MyTreeDBMysql treeDB;
		public ConnDBView(InvokeDelegate startupDelegate, IMyTreeDB treeDB)
		{
			InitializeComponent();
			this.startupDelegate = startupDelegate;
			this.treeDB = treeDB as MyTreeDBMysql;

			//会话列表
			List<string> sessionNames = DBSession.GetSessionNames();
			foreach (string session in sessionNames) {
				sessionList.Items.Add(session);
			}
		}

		//会话列表选择项改变时，右侧会话信息改变
		private void SessionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(sessionList.SelectedIndex != -1)
			{
				DBSession dbSession = DBSession.GetSession(sessionList.SelectedItem.ToString());
				if(dbSession != null)
				{
					this.DataContext = dbSession;
					if(treeDB.TestPing(dbSession))
					{
						MyTrees.treeDB.ConnectDB("");
						if(UserAdmin.UserAdminEnabled)
						{
							lblTip.Text = "该数据库启用了权限控制，需输入用户名和密码才能登陆!";
							lblTip.Foreground = Brushes.DarkRed;
							lblUserID.Visibility = Visibility.Visible;
							lblPwd.Visibility = Visibility.Visible;
							txtUserID.Visibility = Visibility.Visible;
							txtPwd.Visibility = Visibility.Visible;
							btnConnect.IsEnabled = false;
						}
						else
						{
							lblTip.Text = "该数据库没有启用权限控制，请直接点击按钮进行登陆!";
							lblTip.Foreground = Brushes.DarkGreen;
							lblUserID.Visibility = Visibility.Collapsed;
							lblPwd.Visibility = Visibility.Collapsed;
							txtUserID.Visibility = Visibility.Collapsed;
							txtPwd.Visibility = Visibility.Collapsed;
							btnConnect.IsEnabled = true;
						}
					}
					else
					{
						lblTip.Text = "该数据库无法连接，请与管理员联系，检查数据库配置文件是否正确!";
						lblTip.Foreground = Brushes.Red;
						lblUserID.Visibility = Visibility.Collapsed;
						lblPwd.Visibility = Visibility.Collapsed;
						txtUserID.Visibility = Visibility.Collapsed;
						txtPwd.Visibility = Visibility.Collapsed;
						btnConnect.IsEnabled = false;
					}
				}
			}
		}
		
		//服务器IP、用户名、端口号改变时，保存修改按钮enable可用，连接数据库按钮enable不可用
		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			if(txtUserID.Text =="" || txtPwd.Password == "")
			{
				btnConnect.IsEnabled = false;
			}
			else
			{
				btnConnect.IsEnabled = true;
			}
			
		}
		
		//密码改变时，保存修改按钮enable可用，连接数据库按钮enable不可用
		private void TxtPwd_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if(txtUserID.Text =="" || txtPwd.Password == "")
			{
				btnConnect.IsEnabled = false;
			}
			else
			{
				btnConnect.IsEnabled = true;
			}
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			txtUserID.BorderBrush = Brushes.LightBlue;
			txtPwd.BorderBrush = Brushes.LightBlue;
			if(txtUserID.Visibility != Visibility.Collapsed)
			{
				UserInfo userInfo = UserAdmin.GetUserInfo(txtUserID.Text);
				if(userInfo == null)
				{
					lblTip.Text = "输入的用户ID不存在!";
					txtUserID.BorderBrush = Brushes.Red;
					return;
				}
				else if(userInfo.Pwd != EncryptHelper.Encrypt(txtPwd.Password))
				{
					lblTip.Text = "密码不正确!";
					txtPwd.BorderBrush = Brushes.Red;
					return;
				}
			}
			
			lblTip.Text = "登陆成功！正在进入程序。。。";
			DoEvents();
			WindowView.UserID = txtUserID.Text;
			this.startupDelegate();
		}
		
		private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate(object f)
                {
                    (f as DispatcherFrame).Continue = false;

                    return null;
                }
            ), frame);
            Dispatcher.PushFrame(frame);
        }
	}
}