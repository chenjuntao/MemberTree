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
	public class MyTreeDBAMysql : MyTreeDBMysql, IMyTreeDBA
	{
		public bool CreateDB(string dbName)
		{
			dbNameProfile = dbName + "_profile";
			dbNameCalc = dbName + "_calc";
			OpenDB();
			
			//创建profile表
            cmd.CommandText = "create table " 
            	+ dbNameProfile 
            	+ "(k varchar(16)," 
            	+ "v varchar(32))engine=MyISAM";
            cmd.ExecuteNonQuery();
            
            //创建calc表
            cmd.CommandText = "create table " 
            	+ dbNameCalc 
            	+ DBUtil.GetCreateTableSql()
            	+ "engine=MyISAM";
            cmd.ExecuteNonQuery();
            
            CloseDB();
    
            return true;
		}
		
		public void DeleteDB(string dbName)
		{
			OpenDB();
			cmd.CommandText = "drop table if exists "+dbName+"_calc";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "drop table if exists "+dbName+"_profile";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "delete from tree_userprivilege where data_name = '"+ dbName +"'";
			cmd.ExecuteNonQuery();
			CloseDB();
		}
		
		//插入数据
		public void BeginInsert()
		{
			OpenDB();
			
			ConstructInsertSql(100);
		}
		
		//创建插入sql语句
		private void ConstructInsertSql(int count)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("insert into "+dbNameCalc+" values");
			
			//每次提交100个数据
			for (int i = 0; i < count; i++) 
			{
				//字段总个数=可选字段+必选字段(7个)
				int colCount = DBUtil.OptCols.Count + 7;
				sb.Append("(@0");
				sb.Append("_");
				sb.Append(i);
				for (int j = 1; j < colCount; j++) {
					sb.Append(",@");
					sb.Append(j);
					sb.Append("_");
					sb.Append(i);
				}
				sb.Append("),");
			}
			sb.Remove(sb.Length-1, 1);
			cmd.CommandText = sb.ToString();
		}

		int n = 0;
		public void InsertNodes(MyTreeNode node, string[] optCols)
		{
			cmd.Parameters.AddRange(new[] {
            new MySqlParameter("@0_"+n, node.SysId),
            new MySqlParameter("@1_"+n, node.TopId),
            new MySqlParameter("@2_"+n, optCols[2]),
            new MySqlParameter("@3_"+n, node.Level),
            new MySqlParameter("@4_"+n, node.ChildrenLevels),
            new MySqlParameter("@5_"+n, node.SubCount),
			new MySqlParameter("@6_"+n, node.ChildrenCount)});
			
			for (int i = 3; i < optCols.Length; i++) 
			{
				cmd.Parameters.Add(new MySqlParameter("@"+(i+4)+"_"+n, optCols[i]));
			}
			n++;
		}
		
		public void TransCommit(bool end)
		{
			if(end)
			{
				ConstructInsertSql(n);
			}
			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			n = 0;
			if(end)
			{
				CloseDB();
			}
		}
		
		public void CreateIndex()
		{
			OpenDB();
			cmd.CommandText = "create index idxsysid1 on "+dbNameCalc+"(sysid)";
			cmd.ExecuteNonQuery();
	        cmd.CommandText = "create index idxtopid1 on "+dbNameCalc+"(topid)";
			cmd.ExecuteNonQuery();
	        CloseDB();
		}
		
		public void CreateProfile()
		{
			StringBuilder sb = new StringBuilder();
			OpenDB();
			//全部节点总数量
			cmd.CommandText = "insert into "+dbNameProfile+" values(@k,@v)";
			cmd.Parameters.AddWithValue("@k","AllNodeCount");
			cmd.Parameters.AddWithValue("@v", MyTrees.allNodeCount);
			cmd.ExecuteNonQuery();
			//构成树的节点数量
			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@k","TreeNodeCount");
			cmd.Parameters.AddWithValue("@v", MyTrees.TreeNodeCount);
			cmd.ExecuteNonQuery();
			//表格可选列标题
			InsertProfile(sb, "TableOptCol", DBUtil.OptCols);
			//形成树的根节点
			InsertProfile(sb, "Tree", MyTrees.TreeRootNodes);
			//单个叶子节点
			InsertProfile(sb, "Leaf", MyTrees.NoParentNodes);
			//ID有冲突的节点
			InsertProfile(sb, "Conflict", MyTrees.IdConflictNodes);
			//形成闭环的节点
			List<MyTreeNode> nodeList = new List<MyTreeNode>(MyTrees.RingNodes.Values);
			InsertProfile(sb, "Ring", nodeList);
			CloseDB();
		}
		
		private void InsertProfile(StringBuilder sb, string nodeType, List<MyTreeNode> nodes)
		{
			List<string> nodestr = new List<string>();
			foreach (MyTreeNode node in nodes) {
				nodestr.Add(node.SysId);
			}
			InsertProfile(sb, nodeType, nodestr);
		}
		
		private void InsertProfile(StringBuilder sb, string nodeType, List<string> nodes)
		{
			if(nodes.Count > 0)
			{
				sb.Clear();
				cmd.Parameters.Clear();
				sb.Append("insert into ");
				sb.Append(dbNameProfile);
				sb.Append(" values(@k0,@v0)");
				cmd.Parameters.AddWithValue("@k0",nodeType);
				cmd.Parameters.AddWithValue("@v0", nodes[0]);
				for (int i = 1; i < nodes.Count; i++) {
					sb.Append(",(@k" + i);
					sb.Append(",@v" + i + ")");
					cmd.Parameters.AddWithValue("@k" + i,nodeType);
					cmd.Parameters.AddWithValue("@v" + i, nodes[i]);
				}
				cmd.CommandText = sb.ToString();
				cmd.ExecuteNonQuery();
			}
		}
	}
}
