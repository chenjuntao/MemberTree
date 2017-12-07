/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 12/07/2017
 * 时间: 17:26
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace MemberTree
{
	/// <summary>
	/// Description of IConnDB.
	/// </summary>
	public interface IConnDB
	{
		bool Connect(string IP, string usr, string pwd, string port);
		List<string> GetAllDB();
		List<string> GetAllTab(string db);
		List<string> GetAllCol(string tb);
		bool ExportData(StreamWriter mysw, List<string> headCols, string tab, WindowConnDB win);
	}
}
