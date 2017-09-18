/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2016/12/15
 * 时间: 23:08
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// Description of DBHelper.
	/// </summary>
	public class MyTreeDBMysql : IMyTreeDB
	{
	    private string server = "114.55.33.130";
	    private string database = "test";
	    private string userid = "root";
	    private string password = "passwd";
	    protected MySqlConnection conn;
	    protected MySqlCommand cmd;
	    protected string dbName;
	    protected string dbNameProfile;
	    protected string dbNameCalc;
			
		public string DBName
		{
			get{return dbName;}
		}
		
		public string TableName
		{
			get{return dbName;}
		}
	    
		public List<string> GetDBs()
		{
			if(conn == null)
			{
				ConnectDB("");
			}
			OpenDB();
			cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '"+database+"'";
			MySqlDataReader reader = cmd.ExecuteReader();
			List<string> tables = new List<string>();
			while(reader.Read())
			{
				tables.Add(reader.GetString(0));
			}
			List<string> result = new List<string>();
			foreach (string table in tables) 
			{
				if(table.EndsWith("_calc"))
				{
					if(tables.Contains(table.Replace("_calc", "_profile")))
					{
						result.Add(table.Replace("_calc", ""));
					}
				}
			}
			CloseDB();
			return result;
		}
		
		public bool ConnectDB(string dbName)
		{
			if(conn == null)
			{
				//连接数据库
		        MySqlConnectionStringBuilder connstr = new MySqlConnectionStringBuilder();
		        connstr.Server = server;
		        connstr.Database = database;
		        connstr.UserID = userid;
		        connstr.Password = password;
		        connstr.AllowUserVariables = true;
	
		        conn = new MySqlConnection();
		        conn.ConnectionString = connstr.ConnectionString;
		        
		        cmd = new MySqlCommand();
		        cmd.Connection = conn;
		        
		        if(conn.Ping())
		        {
		        	return true;
		        }
			}
			else
			{
				this.dbName = dbName;
				this.dbNameProfile = dbName + "_profile";
				this.dbNameCalc = dbName + "_calc";
				return true;
			}
			return false;
		}
		
		public void OpenDB()
		{
			if(conn.State == ConnectionState.Closed)
			{
				conn.Open();
			}
		}
		
		public void CloseDB()
		{
			if(conn.State == ConnectionState.Open)
			{
				conn.Close();
			}
		}
		
		public int Count
		{
			get
			{
				OpenDB();
				cmd.CommandText = "select AllNodeCount from " + dbNameProfile;
	            MySqlDataReader reader = cmd.ExecuteReader();
	            int result = 0;
	            while (reader.Read())
	            {
	            	result = int.Parse(reader.GetString(0));
	            	break;
	            }
	            CloseDB();
	            return result;
			}
		}
		
		public Dictionary<string, int> GetCounts()
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			OpenDB();
			result["AllNodeCount"] = getCount("select v from " + dbNameProfile + " where k = 'AllNodeCount'");
	        result["TreeNodeCount"] = getCount("select v from " + dbNameProfile + " where k = 'TreeNodeCount'");
	        result["TreeCount"] = getCount("select count(*) from " + dbNameProfile + " where k = 'Tree'");
	        result["LeafCount"] = getCount("select count(*) from " + dbNameProfile + " where k = 'Leaf'");
	        result["RingCount"] = getCount("select count(*) from " + dbNameProfile + " where k = 'Ring'");
	        result["ConflictCount"] = getCount("select count(*) from " + dbNameProfile + " where k = 'Conflict'");
			CloseDB();
			return result;
		}
		
		private int getCount(string sql)
		{
			int result = 0;
			cmd.CommandText = sql;
	        MySqlDataReader reader = cmd.ExecuteReader();
	        if(reader.Read())
	        {
	        	result = int.Parse(reader.GetValue(0).ToString());
	        }
	        reader.Close();
	        return result;
		}
    }  
}
