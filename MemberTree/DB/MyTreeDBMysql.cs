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
using System.Windows;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// Description of DBHelper.
	/// </summary>
	public class MyTreeDBMysql : IMyTreeDB
	{
	    private string server = "114.55.33.130";
	    private string userid = "user1";
	    private string password = "123456";
	    private uint port = 3306;
	    protected MySqlConnection conn;
	    public MySqlCommand cmd;
	    protected string dbName;
	    protected string dbNameProfile;
	    protected string dbNameCalc;
	    
	    public bool TestPing(DBSession dbSession)
	    {
	    	this.server = dbSession.ServerIP;
	    	this.userid = dbSession.UserID;
	    	this.password = dbSession.Password;
	    	this.port = uint.Parse(dbSession.Port);
	    	
	    	//连接数据库
	        MySqlConnectionStringBuilder connstr = new MySqlConnectionStringBuilder();
	        connstr.Server = server;
	        connstr.UserID = userid;
	        connstr.Password = password;
	        connstr.Port = port;
	        connstr.AllowUserVariables = true;

	        MySqlConnection testConn = new MySqlConnection();
	        testConn.ConnectionString = connstr.ConnectionString;
	        try {
	        	testConn.Open();
		        if(testConn.Ping())
		        {
		        	testConn.Close();
		        	return true;
		        }
		        testConn.Close();
		        return false;
	        } catch (Exception ex) {
	        	MessageBox.Show(ex.Message);
	        	return false;
	        }
	    }
			
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
			cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'tree'";
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
//		        connstr.Database = database;
		        connstr.UserID = userid;
		        connstr.Password = password;
		        connstr.Port = port;
		        connstr.AllowUserVariables = true;
	
		        conn = new MySqlConnection();
		        conn.ConnectionString = connstr.ConnectionString;
		        
		        cmd = new MySqlCommand();
		        cmd.Connection = conn;
		        
		        conn.Open();
		        if(conn.Ping())
		        {
		        	//如果不存在tree数据库，则创建一个
		        	cmd.CommandText = "CREATE DATABASE if not exists tree DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_bin";
		        	cmd.ExecuteNonQuery();
		        	conn.ChangeDatabase("tree");
		        	//如果不存在用户信息表，则创建一个(Id,姓名,备注,是否启用,创建日期,最近一次登陆日期,登陆次数,在线时长分钟数)
		        	cmd.CommandText = "create table if not exists tree_userinfo(id varchar(16), name varchar(32), remark varchar(100)," +
		        	"enable char(1), create_date varchar(8), last_login_date varchar(8), login_time int, online_time int)";
		        	cmd.ExecuteNonQuery();
		        	//如果不存在用户权限表，则创建一个
		        	cmd.CommandText = "create table if not exists tree_userprivilege(user_id varchar(16), table_name varchar(64))";
		        	cmd.ExecuteNonQuery();
		        	conn.Close();
		        	return true;
		        }
		        conn.Close();
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
