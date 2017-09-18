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
	public class MyTreeDBASqlite : MyTreeDBSqlite, IMyTreeDBA
	{
		public bool CreateDB(string dbName)
		{
			string dbFile = "db/" + dbName + ".db";

			SQLiteConnection.CreateFile(dbFile);
			ConnectDB(dbName);
            OpenDB();
            
            //创建profile表
            cmd.CommandText = "create table tree_profile(" 
            	+ "k varchar(16),"
            	+ "v varchar(32))";
            cmd.ExecuteNonQuery();
            
            //创建calc表
            cmd.CommandText = "create table tree_calc" 
            	+ DBUtil.GetCreateTableSql();
            cmd.ExecuteNonQuery();
            
            CloseDB();
            
            return true;
		}
		
		public void DeleteDB(string dbName)
		{
			string dbFile = "db/" + dbName + ".db";
			File.Delete(dbFile);
		}

		//插入数据
		public void BeginInsert()
		{
			OpenDB();
			trans = conn.BeginTransaction();
			
			//创建插入sql语句
			StringBuilder sb = new StringBuilder();
			sb.Append("insert into tree_calc values(");
			//字段总个数=可选字段+必选字段(7个)
			int colCount = DBUtil.OptCols.Count + 7;
			sb.Append("@0");
			for (int j = 1; j < colCount; j++) 
			{
				sb.Append(",@");
				sb.Append(j);
			}
			sb.Append(")");
			cmd.CommandText = sb.ToString();
		}

		public void InsertNodes(MyTreeNode node, string[] optCols)
		{
            cmd.Parameters.AddRange(new[] {
            new SQLiteParameter("@0", node.SysId),
            new SQLiteParameter("@1", node.TopId),
            new SQLiteParameter("@2", node.Name),
            new SQLiteParameter("@3", node.Level),
            new SQLiteParameter("@4", node.ChildrenLevels),
            new SQLiteParameter("@5", node.ChildrenNodes.Count),
            new SQLiteParameter("@6", node.ChildrenCount)});
			
			for (int i = 3; i < optCols.Length; i++) 
			{
				cmd.Parameters.Add(new SQLiteParameter("@"+(i+4), optCols[i]));
			}
			
        	cmd.ExecuteNonQuery();
		}
		
		public void TransCommit(bool end)
		{
			if(end)
			{
				trans.Commit();
				CloseDB();
			}
		}
		
		public void CreateIndex()
		{
			OpenDB();
			cmd.CommandText = "create index idxsysid on tree_calc(sysid)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "create index idxtopid on tree_calc(topid)";
			cmd.ExecuteNonQuery();
	        CloseDB();
		}
		
		public void CreateProfile()
		{
			OpenDB();
			trans = conn.BeginTransaction();
			cmd.CommandText = "insert into tree_profile values(@k,@v)";
			//全部节点总数量
			cmd.Parameters.AddWithValue("@k","AllNodeCount");
			cmd.Parameters.AddWithValue("@v", MyTrees.allNodeCount);
			cmd.ExecuteNonQuery();
			//构成树的节点数量
			cmd.Parameters.AddWithValue("@k","TreeNodeCount");
			cmd.Parameters.AddWithValue("@v", MyTrees.TreeNodeCount);
			cmd.ExecuteNonQuery();
			//表格可选列标题
			foreach (string head in DBUtil.OptCols) {
				cmd.Parameters.AddWithValue("@k","TableOptCol");
				cmd.Parameters.AddWithValue("@v", head);
				cmd.ExecuteNonQuery();
			}
			//形成树的根节点
			foreach (MyTreeNode node in MyTrees.TreeRootNodes) {
				cmd.Parameters.AddWithValue("@k","Tree");
				cmd.Parameters.AddWithValue("@v", node.SysId);
				cmd.ExecuteNonQuery();
			}
			//单个叶子节点
			foreach (MyTreeNode node in MyTrees.NoParentNodes) {
				cmd.Parameters.AddWithValue("@k","Leaf");
				cmd.Parameters.AddWithValue("@v", node.SysId);
				cmd.ExecuteNonQuery();
			}
			//ID有冲突的节点
			foreach (MyTreeNode node in MyTrees.IdConflictNodes) {
				cmd.Parameters.AddWithValue("@k","Conflict");
				cmd.Parameters.AddWithValue("@v", node.SysId);
				cmd.ExecuteNonQuery();
			}
			//形成闭环的节点
			foreach (string node in MyTrees.RingNodes.Keys) {
				cmd.Parameters.AddWithValue("@k","Ring");
				cmd.Parameters.AddWithValue("@v", node);
				cmd.ExecuteNonQuery();
			}
			trans.Commit();
			CloseDB();
		}
    }  
}
