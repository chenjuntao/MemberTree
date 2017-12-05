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
using System.Data.SQLite;

namespace MemberTree
{
	/// <summary>
	/// Description of DBHelper.
	/// </summary>
	public class MyTreeDBSqlite : IMyTreeDB
	{
		protected SQLiteConnection conn;
		protected SQLiteCommand cmd;
		protected SQLiteTransaction trans;
		protected string datasetName;
		
		public string DatasetName
		{
			get{return datasetName;}
		}

		public string TableName
		{
			get{return "tree";}
		}
		
		public List<string> GetDatasetNames()
		{			
			//创建一个数据库文件
			if(!Directory.Exists("db"))
			{
				Directory.CreateDirectory("db");
			}
			DirectoryInfo dir = new DirectoryInfo("db");
			FileInfo[] files = dir.GetFiles("*.db");
			//按创建时间排序
			Array.Sort(files, (FileInfo x, FileInfo y) => x.CreationTime.CompareTo(y.CreationTime));
			List<string> result = new List<string>();
			foreach (FileInfo file in files) 
			{
				result.Add(file.Name.Replace(file.Extension, ""));
			}
			return result;
		}
		
		public List<DatasetInfo> GetDatasets()
		{
			List<string> datasetNames = GetDatasetNames();
			List<DatasetInfo> result = new List<DatasetInfo>();
			foreach (string dsName in datasetNames) 
			{
				ConnectDB(dsName);
				OpenDB();
				DatasetInfo dsInfo = new DatasetInfo();
				dsInfo.Name = dsName;
				dsInfo.RowCount = getCount("select v from tree_profile where k = 'AllNodeCount'");
				dsInfo.ColCount = 7 + getCount("select count(*) from tree_profile where k = 'TableOptCol'");
				
				#region 获取创建时间
				FileInfo file = new FileInfo("db/"+dsName+".db");
				if(file != null)
				{
					dsInfo.CreateData = file.CreationTime;
				}
				#endregion
				
				result.Add(dsInfo);
				CloseDB();
			}
			return result;
		}
		
		public bool ConnectDB(string dbName)
		{
			//连接数据库
			this.datasetName = dbName;
			string dbFile = "db/" + dbName + ".db";
			if(File.Exists(dbFile))
			{
	            SQLiteConnectionStringBuilder connstr =new SQLiteConnectionStringBuilder();
	            connstr.DataSource = "db/" + dbName + ".db";
	            connstr.Password = "passwd";
	            connstr.Version = 3;
	
				conn = new SQLiteConnection();
				conn.ConnectionString = connstr.ConnectionString;
	            
	            cmd = new SQLiteCommand();
		        cmd.Connection = conn;
		        
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
		
		public Dictionary<string, int> GetCounts()
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			OpenDB();
			result["AllNodeCount"] = getCount("select v from tree_profile where k = 'AllNodeCount'");
	        result["TreeNodeCount"] = getCount("select v from tree_profile where k = 'TreeNodeCount'");
	        result["TreeCount"] = getCount("select count(*) from tree_profile where k = 'Tree'");
	        result["LeafCount"] = getCount("select count(*) from tree_profile where k = 'Leaf'");
	        result["RingCount"] = getCount("select count(*) from tree_profile where k = 'Ring'");
	        result["ConflictCount"] = getCount("select count(*) from tree_profile where k = 'Conflict'");
			CloseDB();
			return result;
		}
		
		private int getCount(string sql)
		{
			int result = 0;
			cmd.CommandText = sql;
	        SQLiteDataReader reader = cmd.ExecuteReader();
	        if(reader.Read())
	        {
	        	result = int.Parse(reader.GetValue(0).ToString());
	        }
	        reader.Close();
	        return result;
		}
    }  
}
