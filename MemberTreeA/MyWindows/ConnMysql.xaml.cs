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
using System.Windows.Threading;
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
	    private List<string> cols = new List<string>();
	    
		public ConnMysql()
		{
			InitializeComponent();
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
					btnConnect.IsEnabled = true;
				}
				else
				{
					btnConnect.IsEnabled = false;
					btnExport.IsEnabled = false;
					btnCompute.IsEnabled = false;
					txtDBName.Items.Clear();
					txtDBName.IsEnabled = false;
					txtTable.Items.Clear();
					txtTable.IsEnabled = false;
					txtSysid.Items.Clear();
					txtSysid.IsEnabled = false;
					txtTopid.Items.Clear();
					txtTopid.IsEnabled = false;
					txtName.Items.Clear();
					txtName.IsEnabled = false;
					optColsPanel.Children.Clear();
				}
			}
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
			txtDBName.IsEnabled = true;
			txtDBName.Items.Clear();
			txtTable.Items.Clear();
			txtSysid.Items.Clear();
			txtName.Items.Clear();
			txtTopid.Items.Clear();
			optColsPanel.Children.Clear();
			btnExport.IsEnabled = false;
			btnCompute.IsEnabled = false;
			if(Connect())
			{
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
	                txtDBName.Items.Clear();
	                txtDBName.IsEnabled = false;
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
				txtTable.Items.Clear();
				txtTable.IsEnabled = true;
				txtSysid.Items.Clear();
				txtName.Items.Clear();
				txtTopid.Items.Clear();
				optColsPanel.Children.Clear();
				btnExport.IsEnabled = false;
				btnCompute.IsEnabled = false;
				try
	            {
	            	//查出所有的表名
	            	conn.Open();
	            	string db = e.AddedItems[0].ToString();
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
	                txtTable.IsEnabled = false;
	                txtTable.Items.Clear();
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
				txtSysid.IsEnabled = true;
				txtName.IsEnabled = true;
				txtTopid.IsEnabled = true;
				txtSysid.Items.Clear();
				txtName.Items.Clear();
				txtTopid.Items.Clear();
				optColsPanel.Children.Clear();
				btnExport.IsEnabled = false;
				btnCompute.IsEnabled = false;
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
	                	cols.Add(reader.GetString(0));
	                	txtSysid.Items.Add(reader.GetString(0));
	                	txtTopid.Items.Add(reader.GetString(0));
	                	txtName.Items.Add(reader.GetString(0));
	                }
	                reader.Close();
	            }
	            catch (Exception ex)
	            {
	                MessageBox.Show(ex.Message);
	                txtSysid.IsEnabled = false;
					txtName.IsEnabled = false;
					txtTopid.IsEnabled = false;
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
				string addItem = e.AddedItems[0].ToString();
				string removeItem = null;
				if(e.RemovedItems.Count > 0)
				{
					removeItem = e.RemovedItems[0].ToString();
				}
				if(sender == txtSysid)
				{
					txtTopid.Items.Remove(addItem);
					txtName.Items.Remove(addItem);
					if(removeItem!=null)
					{
						txtTopid.Items.Add(removeItem);
						txtName.Items.Add(removeItem);
					}
				}
				else if(sender == txtTopid)
				{
					txtSysid.Items.Remove(addItem);
					txtName.Items.Remove(addItem);
					if(removeItem!=null)
					{
						txtSysid.Items.Add(removeItem);
						txtName.Items.Add(removeItem);
					}
				}
				else if(sender == txtName)
				{
					txtSysid.Items.Remove(addItem);
					txtTopid.Items.Remove(addItem);
					if(removeItem!=null)
					{
						txtSysid.Items.Add(removeItem);
						txtTopid.Items.Add(removeItem);
					}
				}
				if(txtSysid.SelectedIndex != -1
				   && txtTopid.SelectedIndex != -1
				   && txtName.SelectedIndex != -1)
				{
					btnExport.IsEnabled = true;
					btnCompute.IsEnabled = true;
					optColsPanel.Children.Clear();
					foreach (string col in cols) 
					{
						if(col != txtSysid.SelectedValue.ToString()
						  && col != txtTopid.SelectedValue.ToString()
						  && col != txtName.SelectedValue.ToString())
						{
							Button btn = new Button();
							btn.Content = col;
							btn.Margin = new Thickness(2);
							optColsPanel.Children.Add(btn);
						}
					}
				}
				else
				{
					btnExport.IsEnabled = false;
					btnCompute.IsEnabled = false;
				}
			}
		}
		
	    void btnExport_Click(object sender, RoutedEventArgs e)
		{
            try
            { 
            	//先查出总数量
            	cmd.CommandText = "select count(*) from [" + txtTable.SelectedValue + "]";
                MySqlDataReader  sdr = cmd.ExecuteReader();
                sdr.Read();
                int allRow = sdr.GetInt32(0);
                sdr.Close();
                
                //再查下所有的数据
                StringBuilder strBld = new StringBuilder();
                strBld.Append("select [" + txtSysid.SelectedValue + "], [" );
                if(txtName.SelectedIndex != -1)
                {
                	strBld.Append(txtName.SelectedValue + "], [");
                }
                strBld .Append(txtTopid.SelectedValue);
//                if(txtLayer.SelectedIndex != -1)
//                {
//                	strBld.Append("], [" + txtLayer.SelectedValue);
//                }
                strBld.Append("] from [" + txtTable.SelectedValue + "]");
                cmd.CommandText = strBld.ToString();
	            sdr = cmd.ExecuteReader();
	            
	            prograss.Visibility = Visibility.Visible;
	            labelMessage.Text = "开始导出数据到CSV文件......";
	            DoEvents();
	
	            StringBuilder allLines = new StringBuilder("sysid,realname,topid,layer");
	            int row = 0;
	            int step = allRow > 100 ? allRow / 100 : 1;
                while (sdr.Read())
                {   
                	allLines.Append("\r\n");
                	allLines.Append(sdr[0].ToString().Replace(',',' '));
		            allLines.Append(",");
		            if(txtName.SelectedIndex != -1)
	                {
			            allLines.Append(sdr[1].ToString().Replace(',',' '));
			            allLines.Append(",");
			            allLines.Append(sdr[2].ToString().Replace(',',' '));
		            }
		            else
		            {
		            	allLines.Append(",");
		            	allLines.Append(sdr[1].ToString().Replace(',',' '));
		            }
		            allLines.Append(",");
//		            if(txtLayer.SelectedIndex != -1)
//	                {
//			           
//			            allLines.Append(sdr[3].ToString().Replace(',',' '));
//		            }
		            row++;
		            if (row % step == 0)
		            {
		            	prograss.Value = (int)(100.0 * row / allRow);
		                labelMessage.Text = "正在导出第" + row + "个节点（总共" + allRow + "个节点）";
		                DoEvents();
		            }
                }
                sdr.Close();
                
                //保存到csv文件中
                string csvName = "user" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";
                using(StreamWriter mysw = new StreamWriter(csvName, false, Encoding.UTF8))
                {
	            	mysw.Write(allLines);
                }
                MessageBox.Show("数据已经保存完成！");
	            this.Close();
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