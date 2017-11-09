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
					gridSessionInfo.IsEnabled = true;
					this.DataContext = dbSession;
					this.txtDBPwd.Password = dbSession.Password;
					btnSave.IsEnabled = false;
					btnTest.IsEnabled = true;
					btnDelete.IsEnabled = true;
					return;
				}
			}
			this.DataContext = "";
			this.txtDBPwd.Password = "";
			btnSave.IsEnabled = false;
			btnTest.IsEnabled = false;
			btnDelete.IsEnabled = false;
			btnConnect.IsEnabled = false;
			gridSessionInfo.IsEnabled = false;
		}
		
		//会话名称和备注信息改变时，保存修改按钮enable可用
		private void SessionChanged(object sender, TextChangedEventArgs e)
		{
			btnSave.IsEnabled = true;
		}
		
		//服务器IP、用户名、端口号改变时，保存修改按钮enable可用，连接数据库按钮enable不可用
		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			btnSave.IsEnabled = true;
			btnConnect.IsEnabled = false;
		}
		
		//密码改变时，保存修改按钮enable可用，连接数据库按钮enable不可用
		private void TxtPwd_PasswordChanged(object sender, RoutedEventArgs e)
		{
			btnSave.IsEnabled = true;
			btnConnect.IsEnabled = false;
		}
		
		private void btnNew_Click(object sender, RoutedEventArgs e)
		{
			DBSession dbSession = new DBSession();
			this.DataContext = dbSession;
			DBSession.SaveSession(dbSession);
			sessionList.Items.Add(dbSession.SessionName);
			sessionList.SelectedItem = dbSession.SessionName;
		}
				
		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if(sessionList.SelectedIndex != -1)
			{
				if(MessageBox.Show("确定要删除吗?", "确认", MessageBoxButton.OKCancel)==MessageBoxResult.OK)
				{
					DBSession.DeleteSession(sessionList.SelectedItem.ToString());
					sessionList.Items.RemoveAt(sessionList.SelectedIndex);
				}
			}
		}
						
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if(DBSession.GetSessionNames().Contains(txtSessionName.Text) &&
			  txtSessionName.Text != sessionList.SelectedItem.ToString())
			{
				MessageBox.Show("会话“"+txtSessionName.Text+"”已经存在，请修改会话名称!");
			}
			else
			{
				DBSession dbSession = new DBSession()
				{
					SessionName = txtSessionName.Text,
					SessionRemark = txtSessionRemark.Text,
					ServerIP = txtDBServer.Text,
					UserID = txtDBUserID.Text,
					Password = txtDBPwd.Password,
					Port = txtDBPort.Text
				};
				DBSession.SaveSession(dbSession);
				btnSave.IsEnabled = false;
				
				if(txtSessionName.Text != sessionList.SelectedItem.ToString())
				{
					DBSession.DeleteSession(sessionList.SelectedItem.ToString());
					sessionList.Items[sessionList.SelectedIndex] = txtSessionName.Text;
				}
			}
		}
		
		private void btnTest_Click(object sender, RoutedEventArgs e)
		{
			DBSession dbSession = new DBSession()
			{
				SessionName = txtSessionName.Text,
				SessionRemark = txtSessionRemark.Text,
				ServerIP = txtDBServer.Text,
				UserID = txtDBUserID.Text,
				Password = txtDBPwd.Password,
				Port = txtDBPort.Text
			};
			if(treeDB.TestPing(dbSession))
			{
				btnConnect.IsEnabled = true;
			}
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			this.startupDelegate();
		}
	}
}