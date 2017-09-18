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
using System.IO;
using System.Text;

namespace MemberTree
{
	/// <summary>
	/// Description of IMyTreeDB.
	/// </summary>
	public interface IMyTreeDB
	{
		string DBName {get;}
		string TableName{get;}
		List<string> GetDBs();
		bool ConnectDB(string dbName);
		void OpenDB();
		void CloseDB();
		Dictionary<string,int> GetCounts();
    }
}
