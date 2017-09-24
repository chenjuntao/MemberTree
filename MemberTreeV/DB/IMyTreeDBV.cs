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
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// Description of DBHelper.
	/// </summary>
	public interface IMyTreeDBV : IMyTreeDB
	{
		List<MyTreeNode> SearchNode(string sql);
		
		List<MyTreeNode> SearchNode(string sql, List<string> param);
		
		List<string> SearchString(string sql);
    }
}
