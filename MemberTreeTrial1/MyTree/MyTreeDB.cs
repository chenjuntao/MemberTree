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
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace MemberTree
{
	/// <summary>
	/// Description of DBHelper.
	/// </summary>
	public class MyTreeDB
	{
		private SQLiteCommand cmd = new SQLiteCommand();
		private SQLiteConnection conn =new SQLiteConnection();
		
		public bool CreateDB(string dbfile)
		{
			//创建一个数据库文件  
			if(!Directory.Exists("db"))
			{
				Directory.CreateDirectory("db");
			}
			FileInfo fi = new FileInfo(dbfile);
            string dbFile = "db\\" + fi.Name.Replace(".csv", ".db");
			if(File.Exists(dbFile))
			{
				OpenDB(dbFile);
				return false;
			}
			else
			{
				SQLiteConnection.CreateFile(dbFile);
	            OpenDB(dbFile);
	            
	            //创建表
	            StringBuilder sb = new StringBuilder();
	            cmd.CommandText = "create table treenodes(" +
	            	"sysid varchar(11)," +
	            	"topid varchar(11)," +
	            	"name varchar(20)," +
	            	"account varchar(20)," +
	            	"idcard varchar(18)," +
	            	"tel varchar(11)," +
	            	"email varchar(25)," +
	            	"bank varchar(60)," +
	            	"bankcard varchar(20)," +
	            	"addr varchar(100)," +
	            	
	            	"ywczyze varchar(33)," +
	            	"ywcszze varchar(33)," +
	            	"ywczycs varchar(21)," +
	            	"ywcszcs varchar(21)," +
	            	"yzcszz varchar(65)," +
	            	"yzcsxb varchar(65)," +
	            	"yzcsjb varchar(65)," +
	            	"cjqbyzc varchar(65)," +
	            	"glqbyzc varchar(65)," +
	            	"syszz varchar(11)," +
	            	"sysxb varchar(11)," +
	            	"sysjb varchar(11)," +
	            	"cjqbsy varchar(20)," +
	            	"glqbsy varchar(11))";
	            cmd.ExecuteNonQuery();
	            
	            return true;
			}
		}
		
		private void OpenDB(string dbFile)
		{
			//连接数据库  
            conn =new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr =new SQLiteConnectionStringBuilder();
            connstr.DataSource = dbFile;
            connstr.Version = 3;
//            connstr.Password = "admin";
            conn.ConnectionString = connstr.ToString();
//            conn.ConnectionString = "Data Source=:memory:;Version=3;New=True;";

            conn.Open();
            
            cmd = new SQLiteCommand();
	        cmd.Connection = conn;
		}
		
		//插入数据
		public SQLiteTransaction BeginInsert()
		{
			SQLiteTransaction trans = conn.BeginTransaction();
			cmd.CommandText = insertSQL;
			return trans;
		}
		
		private const string insertSQL = "insert into treenodes values(" +
			"@sysid,@topid,@name,@account,@idcard,@tel,@email,@bank,@bankcard,@addr,"+
			"@ywczyze,@ywcszze,@ywczycs,@ywcszcs,@yzcszz,@yzcsxb,@yzcsjb,@cjqbyzc,@glqbyzc,@syszz,@sysxb,@sysjb,@cjqbsy,@glqbsy";
		public void InsertNodes(MyTreeNodeDB node)
		{
            cmd.Parameters.AddRange(new[] {
            new SQLiteParameter("@sysid", node.SysId),
            new SQLiteParameter("@topid", node.TopId),
            new SQLiteParameter("@name", node.Name),  
            new SQLiteParameter("@account", node.Account),
            new SQLiteParameter("@idcard", node.IdCard),
            new SQLiteParameter("@tel", node.Tel),
            new SQLiteParameter("@email", node.Email),
            new SQLiteParameter("@bank", node.Bank),
            new SQLiteParameter("@bankcard", node.BankCard),
            new SQLiteParameter("@addr", node.Addr),
//			new SQLiteParameter("@level", node.Level),  
//          new SQLiteParameter("@childrencount", node.ChildrenCount),
//			new SQLiteParameter("@childrenlevels", node.ChildrenLevels)});
//          new SQLiteParameter("@linecount", node.LineCount),  
			new SQLiteParameter("@ywczyze", node.ywczyze),
			new SQLiteParameter("@ywcszze", node.ywcszze),
			new SQLiteParameter("@ywczycs", node.ywczycs),
			new SQLiteParameter("@ywcszcs", node.ywcszcs),
			new SQLiteParameter("@yzcszz", node.yzcszz),
			new SQLiteParameter("@yzcsxb", node.yzcsxb),
			new SQLiteParameter("@yzcsjb", node.yzcsjb),
			new SQLiteParameter("@cjqbyzc", node.cjqbyzc),
			new SQLiteParameter("@glqbyzc", node.glqbyzc),
			new SQLiteParameter("@syszz", node.syszz),
			new SQLiteParameter("@sysxb", node.sysxb),
			new SQLiteParameter("@sysjb", node.sysjb),
			new SQLiteParameter("@cjqbsy", node.cjqbsy),
			new SQLiteParameter("@glqbsy", node.glqbsy)});

        	cmd.ExecuteNonQuery();
		}
		
		public void TransCommit(SQLiteTransaction trans)
		{
			trans.Commit();
		}
		
		public int Count
		{
			get
			{
				cmd.CommandText = "select count(*) from treenodes";
	            SQLiteDataReader reader = cmd.ExecuteReader();
	            StringBuilder sb = new StringBuilder();
	            while (reader.Read())
	            {
	            	return reader.GetInt32(0);
	            }
	            return -1;
			}
		}
		
		public MyTreeNodeDB this[string sysId]
		{
			get
			{
				cmd.CommandText = "select * from treenodes where sysid='"+sysId+"'";
	            SQLiteDataReader reader = cmd.ExecuteReader();
	            while (reader.Read())
	            {
	            	MyTreeNodeDB node = new MyTreeNodeDB()
	            	{
	            		SysId = reader.GetString(0), 
	            		TopId = reader.GetString(1),
                        Name = reader.GetString(2), 
                        Account = reader.GetString(3), 
                        IdCard = reader.GetString(4), 
                        Tel = reader.GetString(5), 
                        Email = reader.GetString(6), 
                        Bank = reader.GetString(7), 
                        BankCard = reader.GetString(8), 
                        Addr = reader.GetString(9),
                        
                        ywczyze = reader.GetString(10),
                        ywcszze = reader.GetString(11),
                        ywczycs = reader.GetString(12),
                        ywcszcs = reader.GetString(13),
                        yzcszz = reader.GetString(14),
                        yzcsxb = reader.GetString(15),
                        yzcsjb = reader.GetString(16),
                        cjqbyzc = reader.GetString(17),
                        glqbyzc = reader.GetString(18),
                        syszz = reader.GetString(19),
                        sysxb = reader.GetString(20),
                        sysjb = reader.GetString(21),
                        cjqbsy = reader.GetString(22),
                        glqbsy = reader.GetString(23)
	            	};
	            	reader.Close();
	            	return node;
	            }
	            reader.Close();
	            return null;
			}
		}
		
		public List<string> Search(string sql)
		{
			List<string> ids = new List<string>();
			cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
            	ids.Add(reader.GetString(0));
            }
            reader.Close();
            return ids;
		}
		
		public void CreateIndex()
		{
			cmd.CommandText = "create index idxsysid on treenodes(sysid)";
	        cmd.ExecuteNonQuery();
		}
    }  
}
