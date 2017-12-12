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
using System.Windows.Markup;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// 连接到Sqlserver并导出数据
	/// </summary>
	public partial class WindowConnDB : Window
	{
		private IConnDB connDB;
		
		public WindowConnDB(string db)
		{
			InitializeComponent();
			if(db == "mysql")
			{
				connDB = new ConnMysql();
			}
			else if(db == "sqlserver")
			{
				connDB = new ConnSqlserver();
				this.Title="连接到Sqlserver";
				txtblk.Text="连本机直接输入“.”";
				txtblk.HorizontalAlignment=HorizontalAlignment.Left;
				txtPort.Visibility = Visibility.Collapsed;
				txtUserName.Text = "sa";
			}
		}
		
		private void SetEnabled(bool enabled, params Control[] controls)
		{
			foreach (Control control in controls) 
			{
				control.IsEnabled = enabled;
			}
		}
		
		private void ClearItems(params IAddChild[] controls)
		{
			foreach (IAddChild control in controls)
			{
				if(control is ComboBox)
				{
					(control as ComboBox).Items.Clear();
				}
				else if(control is WrapPanel)
				{
					(control as WrapPanel).Children.Clear();
				}
			}	
		}
		
		//服务器IP、端口号、用户名、密码改变时，保存修改按钮enable可用
		private void TextChanged(object sender, RoutedEventArgs e)
		{
			if(btnConnect != null)
			{
				if(txtDBServer.Text!="" &&
				   txtUserName.Text!="" &&
				   txtPwd.Password!=""&&
				   txtPort.Text!="")
				{
					SetEnabled(true, btnConnect);
				}
				else
				{
					SetEnabled(false, btnConnect, btnExport, btnCompute, txtDBName, txtTable, txtSysid, txtTopid, txtName);
					ClearItems(optColsPanel);
				}
			}
		}
		
		private void btnDisConnect_Click(object sender, RoutedEventArgs e)
		{
			SetEnabled(true, btnConnect, txtDBServer, txtUserName, txtPwd);
			SetEnabled(false, btnDisConnect, txtDBName, txtTable, txtSysid, txtTopid, txtName, btnExport, btnCompute);
			ClearItems(txtDBName, txtTable, txtSysid, txtTopid, txtName, optColsPanel);
		}
		
		private bool Connect()
		{
			return connDB.Connect(txtDBServer.Text, txtUserName.Text, txtPwd.Password, "3306");
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			if(Connect())
			{
				SetEnabled(true, btnDisConnect, txtDBName);
				SetEnabled(false, btnConnect, txtDBServer, txtUserName, txtPwd);
				
				List<string> dbList = connDB.GetAllDB();
				foreach (string db in dbList) 
				{
					txtDBName.Items.Add(db);
				}
			}
		}
		
		//数据库名变化时，更新表名下拉框
		private void DB_SelectChange(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Count>0)
			{
				SetEnabled(true, txtTable);
				SetEnabled(false, btnExport, btnCompute);
				ClearItems(txtTable, txtSysid, txtTopid, txtName, optColsPanel);
				
				string db = e.AddedItems[0].ToString();
				List<string> tabList = connDB.GetAllTab(db);
				foreach (string tab in tabList) 
				{
					txtTable.Items.Add(tab);
				}
			}
		}
		
		//表名变化时，更新列名下拉框
		private void Table_SelectChange(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Count>0)
			{
				SetEnabled(true, txtSysid, txtTopid, txtName);
				SetEnabled(false, btnExport, btnCompute);
				ClearItems(txtSysid, txtTopid, txtName, optColsPanel);
	            string tb = e.AddedItems[0].ToString();
				List<string> colList = connDB.GetAllCol(tb);
				foreach (string col in colList) 
				{
					ComboBoxItem item = new ComboBoxItem();
			    	item.Content = col;
			    	txtSysid.Items.Add(item);
			    	ComboBoxItem item2 = new ComboBoxItem();
			    	item2.Content = col;
			    	txtTopid.Items.Add(item2);
			    	ComboBoxItem item3 = new ComboBoxItem();
			    	item3.Content = col;
			    	txtName.Items.Add(item3);
			    	Button btn = new Button();
					btn.Content = col;
					btn.Margin = new Thickness(2);
					btn.Padding = new Thickness(2);
					btn.IsEnabled = true;
					optColsPanel.Children.Add(btn);
				}
			}
		}
		
		//选择的列变化时，更新其他列名下拉框
		private void Col_SelectChange(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Count>0)
			{
				if(sender == txtSysid)
				{
					int addItemIndex = txtSysid.Items.IndexOf(e.AddedItems[0]);
					(txtTopid.Items[addItemIndex] as ComboBoxItem).Visibility = Visibility.Collapsed;
					(txtName.Items[addItemIndex] as ComboBoxItem).Visibility = Visibility.Collapsed;
					(optColsPanel.Children[addItemIndex] as Button).Visibility = Visibility.Collapsed;
					if(e.RemovedItems.Count > 0)
					{
						int removeItemIndex = txtSysid.Items.IndexOf(e.RemovedItems[0]);
						(txtTopid.Items[removeItemIndex] as ComboBoxItem).Visibility = Visibility.Visible;
						(txtName.Items[removeItemIndex] as ComboBoxItem).Visibility = Visibility.Visible;
						(optColsPanel.Children[removeItemIndex] as Button).Visibility = Visibility.Visible;
					}
				}
				else if(sender == txtTopid)
				{
					int addItemIndex = txtTopid.Items.IndexOf(e.AddedItems[0]);
					(txtSysid.Items[addItemIndex] as ComboBoxItem).Visibility = Visibility.Collapsed;
					(txtName.Items[addItemIndex] as ComboBoxItem).Visibility = Visibility.Collapsed;
					(optColsPanel.Children[addItemIndex] as Button).Visibility = Visibility.Collapsed;
					if(e.RemovedItems.Count > 0)
					{
						int removeItemIndex = txtTopid.Items.IndexOf(e.RemovedItems[0]);
						(txtSysid.Items[removeItemIndex] as ComboBoxItem).Visibility = Visibility.Visible;
						(txtName.Items[removeItemIndex] as ComboBoxItem).Visibility = Visibility.Visible;
						(optColsPanel.Children[removeItemIndex] as Button).Visibility = Visibility.Visible;
					}
				}
				else if(sender == txtName)
				{
					int addItemIndex = txtName.Items.IndexOf(e.AddedItems[0]);
					(txtSysid.Items[addItemIndex] as ComboBoxItem).Visibility = Visibility.Collapsed;
					(txtTopid.Items[addItemIndex] as ComboBoxItem).Visibility = Visibility.Collapsed;
					(optColsPanel.Children[addItemIndex] as Button).Visibility = Visibility.Collapsed;
					if(e.RemovedItems.Count > 0)
					{
						int removeItemIndex = txtName.Items.IndexOf(e.RemovedItems[0]);
						(txtSysid.Items[removeItemIndex] as ComboBoxItem).Visibility = Visibility.Visible;
						(txtTopid.Items[removeItemIndex] as ComboBoxItem).Visibility = Visibility.Visible;
						(optColsPanel.Children[removeItemIndex] as Button).Visibility = Visibility.Visible;
					}
				}
				if(txtSysid.SelectedIndex!=-1
				 &&txtTopid.SelectedIndex!=-1
				 &&txtName.SelectedIndex!=-1)
				{
					SetEnabled(true, btnExport, btnCompute);
				}
			}
		}
		
	   	private void btnExport_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDlg = new SaveFileDialog();
		    saveFileDlg.Title = "选择导出数据文件存放的位置";
		    saveFileDlg.Filter = "tab分隔文件|*.tab";
		    saveFileDlg.FileName = "exp_" + txtTable.SelectedValue;
		    if (saveFileDlg.ShowDialog() == true)
		    {
		    	bool result = ExportData(saveFileDlg.FileName);
		    	if(result)
		    	{
		    		MessageBox.Show("导出数据完成！");
		    	}
		    }
		}
	   	
	   	private void btnCompute_Click(object sender, RoutedEventArgs e)
	   	{
	   		if(!Directory.Exists(MemData.MemDataTemp))
	   		{
	   			Directory.CreateDirectory(MemData.MemDataTemp);
	   		}
	   		string expName = MemData.MemDataTemp + "/exp_" + txtTable.SelectedValue + ".tab";
	    	if(ExportData(expName))
	    	{
	    		this.Close();
	    		MyTrees.OpenDBFile(expName, "\t", false);
	    		File.Delete(expName);
	    	}
	   	}
	   	
	   	private bool ExportData(string file)
	   	{
	   		bool result = false;
	   		
	   		SetEnabled(false, btnDisConnect, btnExport, btnCompute, txtDBName, txtTable, txtSysid, txtTopid, txtName);
        	prograss.Visibility = Visibility.Visible;
            labelMessage.Text = "开始导出数据......";
            DoEvents();
           
            StreamWriter mysw = new StreamWriter(file, false, Encoding.UTF8);
            
            //表头第一行
            List<string> headCols = new List<string>();
            headCols.Add((txtSysid.SelectedValue as ComboBoxItem).Content.ToString());
			headCols.Add((txtTopid.SelectedValue as ComboBoxItem).Content.ToString());
			headCols.Add((txtName.SelectedValue as ComboBoxItem).Content.ToString());
            foreach (UIElement element in optColsPanel.Children)
            {
            	Button btn = element as Button;
            	if(btn.Visibility==Visibility.Visible)
            	{
            		headCols.Add(btn.Content.ToString());
            	}
            }
            mysw.WriteLine(string.Join("\t", headCols));
                
            result = connDB.ExportData(mysw, headCols, txtTable.SelectedValue.ToString(), this);
 
            SetEnabled(true, btnDisConnect, btnExport, btnCompute, txtDBName, txtTable, txtSysid, txtTopid, txtName);
            return result;
	   	}
		
	    public void DoEvents()
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