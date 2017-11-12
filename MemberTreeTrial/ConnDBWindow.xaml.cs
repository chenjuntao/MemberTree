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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Threading;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for ConnDBWindow.xaml
	/// </summary>
	public partial class ConnDBWindow : Window
	{
		public ConnDBWindow()
		{
			InitializeComponent();
		}
		void rbLocalHost_Checked(object sender, RoutedEventArgs e)
		{
			txtDBServer.Text= ".";
			txtDBServer.IsEnabled = false;
			txtDBServer.Background = Brushes.Gray;
		}
		
		void rbOtherHost_Checked(object sender, RoutedEventArgs e)
		{
			txtDBServer.Text= "";
			txtDBServer.IsEnabled = true;
			txtDBServer.Background = Brushes.White;
		}
	
		private bool CheckTxtBox(TextBox txtBox, string name)
		{
			if(txtBox.Text=="")
			{
				txtBox.BorderBrush = Brushes.Red;
				MessageBox.Show(name +"不能为空！");
				return false;
			}
			return true;
		}
		
		private bool CheckComboBox(ComboBox txtBox, string name)
		{
			if(txtBox.SelectedIndex == -1)
			{
				txtBox.BorderBrush = Brushes.Red;
				MessageBox.Show(name +"不能为空！");
				return false;
			}
			return true;
		}
		
		private SqlConnection Connect()
		{
			if(!CheckTxtBox(txtDBServer,"服务器名称"))
		    {
		   		return null;
		    }
			if(!CheckTxtBox(txtDBName,"数据库名称"))
			{
		   		return null;
		    }
			if(!CheckTxtBox(txtUserName,"用户名"))
			{
		   		return null;
		    }
			
			// 使用一个IntPtr类型值来存储加密字符串的起始点  
            IntPtr p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(txtPwd.SecurePassword); 
            // 使用.NET内部算法把IntPtr指向处的字符集合转换成字符串  
            string password = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
			if(password=="")
			{
				txtPwd.BorderBrush = Brushes.Red;
				MessageBox.Show("密码不能为空！");
			}
		
			string connStr = "server="+ txtDBServer.Text
				+ ";database=" + txtDBName.Text
				+ ";uid=" + txtUserName.Text
				+ ";pwd=" + password;
            SqlConnection sConn = new SqlConnection(connStr);
            try
            {
                sConn.Open();
                return sConn;
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接错误:" + ex.Message);
                txtTableName.IsEnabled = false;
                txtSysid.IsEnabled = false;
                txtRealname.IsEnabled = false;
                txtTopid.IsEnabled = false;
                txtLayer.IsEnabled = false;
            }
            return null;
		}

		void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			txtTableName.IsEnabled = true;
			txtTableName.Items.Clear();
			SqlConnection sConn = Connect();
			if(sConn != null)
			{
	            try
	            {
	            	//先查出所有的表名
	            	string sqlStr = "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name";
	            	SqlCommand sCmd = new SqlCommand(sqlStr, sConn);
	                SqlDataReader  sdr = sCmd.ExecuteReader();
	                while (sdr.Read())
	                {  
	                	txtTableName.Items.Add(sdr.GetString(0));
	                }
	                sdr.Close();
	            }
	            catch (Exception ex)
	            {
	                MessageBox.Show(ex.Message);
	                txtTableName.IsEnabled = false;
	            }
	            finally
	            {
	            	 sConn.Close();
	            }
			}
		}
		
		void btnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		
		//表名变化时，更新列名下拉框
		void txtTableName_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			txtSysid.IsEnabled = true;
			txtRealname.IsEnabled = true;
			txtTopid.IsEnabled = true;
			txtLayer.IsEnabled = true;
			txtSysid.Items.Clear();
			txtRealname.Items.Clear();
			txtTopid.Items.Clear();
			txtLayer.Items.Clear();
			SqlConnection sConn = Connect();
            try
            {
            	//先查出所有的表名
            	string sqlStr = "select name from syscolumns where id=(object_id('" + txtTableName.SelectedValue + "'))";
            	SqlCommand sCmd = new SqlCommand(sqlStr, sConn);
                SqlDataReader  sdr = sCmd.ExecuteReader();
                while (sdr.Read())
                {
                	txtSysid.Items.Add(sdr.GetString(0));
                	txtRealname.Items.Add(sdr.GetString(0));
                	txtTopid.Items.Add(sdr.GetString(0));
                	txtLayer.Items.Add(sdr.GetString(0));
                }
                sdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtSysid.IsEnabled = false;
				txtRealname.IsEnabled = false;
				txtTopid.IsEnabled = false;
				txtLayer.IsEnabled = false;
            }
            finally
            {
            	 sConn.Close();
            }
		} 
		
	    void btnExport_Click(object sender, RoutedEventArgs e)
		{
	    	if(!CheckComboBox(txtTableName,"数据表名"))
	    	{
	    		return;
	    	}
	    	if(!CheckComboBox(txtSysid,"sysid列"))
	    	{
	    		return;
	    	}
	    	if(!CheckComboBox(txtTopid,"topid列"))
	    	{
	    		return;
	    	}
	    	SqlConnection sConn = Connect();
            try
            { 
            	//先查出总数量
            	SqlCommand sCmd = new SqlCommand("select count(*) from [" + txtTableName.SelectedValue + "]", sConn);
                SqlDataReader  sdr = sCmd.ExecuteReader();
                sdr.Read();
                int allRow = sdr.GetInt32(0);
                sdr.Close();
                
                //再查下所有的数据
                StringBuilder strBld = new StringBuilder();
                strBld.Append("select [" + txtSysid.SelectedValue + "], [" );
                if(txtRealname.SelectedIndex != -1)
                {
                	strBld.Append(txtRealname.SelectedValue + "], [");
                }
                strBld .Append(txtTopid.SelectedValue);
                if(txtLayer.SelectedIndex != -1)
                {
                	strBld.Append("], [" + txtLayer.SelectedValue);
                }
                strBld.Append("] from [" + txtTableName.SelectedValue + "]");
                sCmd.CommandText = strBld.ToString();
	            sdr = sCmd.ExecuteReader();
	            
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
		            if(txtRealname.SelectedIndex != -1)
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
		            if(txtLayer.SelectedIndex != -1)
	                {
			           
			            allLines.Append(sdr[3].ToString().Replace(',',' '));
		            }
		            row++;
		            if (row % step == 0)
		            {
		            	prograss.Value = (int)(100.0 * row / allRow);
		                labelMessage.Text = "正在导出第" + row + "个节点（总共" + allRow + "个节点）";
		                DoEvents();
		            }
                }
                sdr.Close();
                sConn.Close();
                
                //保存到csv文件中
                string csvName = "user" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";
                using(StreamWriter mysw = new StreamWriter(csvName, false, Encoding.UTF8))
                {
	            	mysw.Write(allLines);
                }
                MessageBox.Show("数据已经保存到" + System.Windows.Forms.Application.StartupPath + "目录下！");
	            this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            	 sConn.Close();
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