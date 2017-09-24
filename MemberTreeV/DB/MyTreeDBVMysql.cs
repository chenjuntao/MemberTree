﻿/*
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
	public class MyTreeDBVMysql : MyTreeDBMysql, IMyTreeDBV
	{
		public List<MyTreeNode> SearchNode(string sql)
		{
			cmd.CommandText = sql;
            MySqlDataReader reader = cmd.ExecuteReader();
            List<MyTreeNode> result = new List<MyTreeNode>();
            while (reader.Read())
            {
            	MyTreeNode node = new MyTreeNode()
	        	{
	        		SysId = reader.GetString(0), 
	        		TopId = reader.GetString(1),
	        		Name = reader.GetString(2),
	        		Level = reader.GetInt32(3),
	        		ChildrenLevels = reader.GetInt32(4),
	        		ChildrenCount = reader.GetInt32(5),
	        		ChildrenCountAll = reader.GetInt32(6)
	        	};
            	result.Add(node);
            }
            reader.Close();
            return result;
		}
		
		public List<MyTreeNode> SearchNode(string sql, List<string> param)
		{
			for (int i = 0; i < param.Count; i++) 
			{
				cmd.Parameters.AddWithValue(i.ToString(), param[i]);
			}
			cmd.Parameters.Clear();
			List<MyTreeNode> result = SearchNode(sql);
			return result;
		}
		
		public List<string> SearchString(string sql)
		{
			cmd.CommandText = sql;
			MySqlDataReader reader = cmd.ExecuteReader();
			List<string> result = new List<string>();
            object[] oj = new object[reader.FieldCount];
            while (reader.Read())
            {
				reader.GetValues(oj);
				string node = string.Join(",", oj);
            	result.Add(node);
            }
            reader.Close();
            return result;
		}
    }  
}
