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
using System.IO;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// Description of ConnMysql.
	/// </summary>
	public class ConnMysql : IConnDB
	{
		private MySqlConnection conn;
	    private MySqlCommand cmd;
	    
		public ConnMysql()
		{
		}

		#region IConnDB implementation

		public bool Connect(string IP, string usr, string pwd, string portstr)
		{
			uint port  = 3306;
			if(!uint.TryParse(portstr, out port))
			{
				MessageBox.Show("端口号必须为大于0的数字！");
			}	
			
			MySqlConnectionStringBuilder sqlStrBuilder = new MySqlConnectionStringBuilder();
			sqlStrBuilder.Server = IP; //server ip
			sqlStrBuilder.Port = port; //端口号
			sqlStrBuilder.UserID = usr;  //用户名
			sqlStrBuilder.Password = pwd;  //密码

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

		public List<string> GetAllDB()
		{
			List<string> result = new List<string>();
			 try
            {
            	//查出所有的数据库名
            	conn.Open();
            	cmd.CommandText = "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA";
                MySqlDataReader reader = cmd.ExecuteReader();
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
            	cmd.CommandText = string.Format("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}'", db);
                MySqlDataReader reader = cmd.ExecuteReader();
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
            	cmd.CommandText = string.Format("select column_name from information_schema.columns where table_name = '{0}'", tb);
                MySqlDataReader reader = cmd.ExecuteReader();
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
	   		//保存到tab文件中，以tab键分割
            try
            {
            	//先查出总数量
            	conn.Open();
            	cmd.CommandText = "select count(*) from " + tab;
                MySqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                int allRow = sdr.GetInt32(0);
                sdr.Close();

                //再查询所有的数据
				cmd.CommandText = "select " + string.Join(",", headCols) + " from " + tab;
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
            	 conn.Close();
            	 mysw.Close();
            }
            return result;
		}

		#endregion
	}
}
