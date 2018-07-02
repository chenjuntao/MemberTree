/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 12/07/2017
 * 时间: 17:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;

namespace MemberTree
{
	/// <summary>
	/// Description of ConnSqlserver.
	/// </summary>
	public class ConnSqlserver : IConnDB
	{
		private SqlConnection conn;
		private SqlCommand cmd;
		public ConnSqlserver()
		{
		}
		
		#region IConnDB implementation
	
		public bool Connect(string IP, string usr, string pwd, string port)
		{
			 SqlConnectionStringBuilder sqlStrBuilder = new SqlConnectionStringBuilder();
            sqlStrBuilder.DataSource = IP; //server ip
//            sqlStrBuilder.InitialCatalog = txtDBName.Text; //数据库名
            sqlStrBuilder.UserID = usr;  //用户名
            sqlStrBuilder.Password = pwd;  //密码
            sqlStrBuilder.IntegratedSecurity = false; //false:用户名密码验证；true：windows身份验证
            conn = new SqlConnection(sqlStrBuilder.ConnectionString);
            try
            {
                cmd = conn.CreateCommand();
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接错误:" + ex.Message);
                return false;
            }
		}
	
		public List<string> GetAllDB()
		{
			List<string> result = new List<string>();
			try
            {
            	//查出所有的数据库名
            	conn.Open();
            	cmd.CommandText = "SELECT Name FROM Master..SysDatabases ORDER BY Name";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                	result.Add(reader.GetString(0));
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
			return result;
		}
	
		public List<string> GetAllTab(string db)
		{
			List<string> result = new List<string>();
			try
            {
            	//查出所有的表名
            	conn.Open();
            	conn.ChangeDatabase(db);
            	cmd.CommandText = string.Format("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                	result.Add(reader.GetString(0));
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
			return result;
		}
	
		public List<string> GetAllCol(string tb)
		{
			List<string> result = new List<string>();
			try
			{
				//查出所有的列名
				conn.Open();
				cmd.CommandText = string.Format("select name from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='{0}')", tb);
			    SqlDataReader reader = cmd.ExecuteReader();
			    while (reader.Read())
			    {
			    	result.Add(reader.GetString(0));
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
			return result;
		}
	
		public bool ExportData(StreamWriter mysw, List<string> headCols, string tab, WindowConnDB win)
		{
			bool result = false;
			int allRow = 0;
			int row = 0;
	   		//保存到tab文件中，以tab键分割
            try
            {
            	//先查出总数量
            	conn.Open();
            	cmd.CommandText = "select count(*) from " + tab;
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                allRow = sdr.GetInt32(0);
                sdr.Close();

                //再查询所有的数据
				cmd.CommandText = "select " + string.Join(",", headCols) + " from " + tab;
	            sdr = cmd.ExecuteReader();

	           
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
		            	win.prograss.Value = (int)(100.0 * row / allRow);
		                win.labelMessage.Text = "正在导出第" + row + "（总共" + allRow + "）";
		                win.DoEvents();
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
            	 win.prograss.Visibility = Visibility.Hidden;
		         win.labelMessage.Text = "导出完成！一共导出" + row + "（总共" + allRow + "）";
            	 conn.Close();
            	 mysw.Close();
            }
            return result;
		}
	
		#endregion
	}
}
