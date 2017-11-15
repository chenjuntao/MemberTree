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
	public interface IMyTreeDBA : IMyTreeDB
	{
		bool CreateDB(string dbName);
		
		void DeleteDB(string dbName);
		
		void BeginInsert();
		
		void InsertNodes(MyTreeNode node, string[] optCols);
		
		void TransCommit(bool end);
		
		void CreateIndex(); 
		
		void CreateProfile(); 
    }  
}
