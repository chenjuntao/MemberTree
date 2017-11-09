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
using System.Windows.Threading;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// 连接到mysql数据库导出数据
	/// </summary>
	public partial class ConnMysql : Window
	{
		private MySqlConnection conn;
	    private MySqlCommand cmd;
	    
		public ConnMysql()
		{
			InitializeComponent();
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
				   txtPort.Text!="" &&
				   txtUserName.Text!="" &&
				   txtPwd.Password!="")
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
			SetEnabled(true, btnConnect, txtDBServer, txtPort, txtUserName, txtPwd);
			SetEnabled(false, btnDisConnect, txtDBName, txtTable, txtSysid, txtTopid, txtName, btnExport, btnCompute);
			ClearItems(txtDBName, txtTable, txtSysid, txtTopid, txtName, optColsPanel);
		}
		
		private bool Connect()
		{
			uint port  = 3306;
			if(!uint.TryParse(txtPort.Text, out port))
			{
				MessageBox.Show("端口号必须为大于0的数字！");
			}
				
			MySqlConnectionStringBuilder sqlStrBuilder = new MySqlConnectionStringBuilder();
			sqlStrBuilder.Server = txtDBServer.Text; //server ip
			sqlStrBuilder.Port = port; //端口号
			sqlStrBuilder.UserID = txtUserName.Text;  //用户名
			sqlStrBuilder.Password = txtPwd.Password;  //密码

            conn = new MySqlConnection(sqlStrBuilder.ConnectionString);
            try {
	        	conn.Open();
		        if(conn.Ping())
		        {
		        	cmd = new MySqlCommand();
		        	cmd.Connection = conn;
		        	conn.Close();
		        	return true;
		        }
		        conn.Close();
		        return false;
	        } catch (Exception ex) {
	        	MessageBox.Show("数据库连接错误:"+ex.Message);
	        	return false;
	        }
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			if(Connect())
			{
				SetEnabled(true, btnDisConnect, txtDBName);
				SetEnabled(false, btnConnect, txtDBServer, txtPort, txtUserName, txtPwd);
	            try
	            {
	            	//查出所有的数据库名
	            	conn.Open();
	            	cmd.CommandText = "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA";
	                MySqlDataReader reader = cmd.ExecuteReader();
	                while (reader.Read())
	                {
	                	txtDBName.Items.Add(reader.GetString(0));
	                }
	                reader.Close();
	            }
	            catch (Exception ex)
	            {
	                MessageBox.Show(ex.Message);
	            }
	            finally
	            {
	            	 conn.Close();
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
				try
	            {
	            	//查出所有的表名
	            	conn.Open();
	            	string db = e.AddedItems[0].ToString();
	            	conn.ChangeDatabase(db);
	            	cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '"
	            						+ db + "'";
	                MySqlDataReader reader = cmd.ExecuteReader();
	                while (reader.Read())
	                {
	                	txtTable.Items.Add(reader.GetString(0));
	                }
	                reader.Close();
	            }
	            catch (Exception ex)
	            {
	                MessageBox.Show(ex.Message);
	            }
	            finally
	            {
	            	 conn.Close();
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
	            try
	            {
	            	//查出所有的列名
	            	conn.Open();
	            	string db = txtDBName.Text;
	            	string tb = e.AddedItems[0].ToString();
	            	cmd.CommandText = "select column_name from information_schema.columns where table_schema = '"
	            						+ db + "' and table_name = '" + tb + "'";
	                MySqlDataReader reader = cmd.ExecuteReader();
	                while (reader.Read())
	                {
	                	ComboBoxItem item = new ComboBoxItem();
	                	item.Content = reader.GetString(0);
	                	txtSysid.Items.Add(item);
	                	ComboBoxItem item2 = new ComboBoxItem();
	                	item2.Content = reader.GetString(0);
	                	txtTopid.Items.Add(item2);
	                	ComboBoxItem item3 = new ComboBoxItem();
	                	item3.Content = reader.GetString(0);
	                	txtName.Items.Add(item3);
	                	Button btn = new Button();
						btn.Content = reader.GetString(0);
						btn.Margin = new Thickness(2);
						btn.Padding = new Thickness(2);
						btn.IsEnabled = true;
						optColsPanel.Children.Add(btn);
	                }
	                reader.Close();
	            }
	            catch (Exception ex)
	            {
	                MessageBox.Show(ex.Message);
	            }
	            finally
	            {
	            	 conn.Close();
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
	   		if(!Directory.Exists("temp"))
	   		{
	   			Directory.CreateDirectory("temp");
	   		}
	   		string expName = "temp/exp_" + txtTable.SelectedValue + ".tab";
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
	   		//保存到tab文件中，以tab键分割
           
            StreamWriter mysw = new StreamWriter(file, false, Encoding.UTF8);
            try
            {
            	SetEnabled(false, btnDisConnect, btnExport, btnCompute, txtDBName, txtTable, txtSysid, txtTopid, txtName);
            	prograss.Visibility = Visibility.Visible;
	            labelMessage.Text = "开始导出数据......";
	            DoEvents();

            	//先查出总数量
            	conn.Open();
            	cmd.CommandText = "select count(*) from " + txtTable.SelectedValue;
                MySqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                int allRow = sdr.GetInt32(0);
                sdr.Close();
                
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
                
                //再查询所有的数据
				cmd.CommandText = "select " + string.Join(",", headCols) + " from " + txtTable.SelectedValue;
	            sdr = cmd.ExecuteReader();

	            int row = 0;
	            int step = allRow > 100 ? allRow / 100 : 1;
	            object[] objs = new object[sdr.FieldCount];
	            
            	while (sdr.Read())
                {
                	sdr.GetValues(objs);
                	for (int i = 0; i < objs.Length; i++) {
                		string obj = objs[i].ToString();
                		if(obj.Contains("\t"))
                		{
                			objs[i] = obj.Replace("\t", " ");
                		}
                	}
					string line = string.Join("\t", objs);
                	mysw.WriteLine(line);
                	
		            row++;
		            if (row % step == 0)
		            {
		            	prograss.Value = (int)(100.0 * row / allRow);
		                labelMessage.Text = "正在导出第" + row + "（总共" + allRow + "）";
		                DoEvents();
		            }
                }
 
                sdr.Close();
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            	 conn.Close();
            	 mysw.Close();
            }
            SetEnabled(true, btnDisConnect, btnExport, btnCompute, txtDBName, txtTable, txtSysid, txtTopid, txtName);
            return result;
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