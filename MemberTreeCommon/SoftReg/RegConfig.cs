/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 2017/12/5
 * 时间: 18:46
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace MemberTree
{
	/// <summary>
	/// Description of RegConfig.
	/// </summary>
	public static class RegConfig
	{
		public static Dictionary<string, string> config = new Dictionary<string, string>();
		
		//MyTreeDBMysql
		public static string MYSQL_QUERY_ALL_TABLE_NAME = "";
		public static string MYSQL_CREATE_DATABASE = "";
		public static string MYSQL_CREATE_TABLE_USER_INFO = "";
		public static string MYSQL_CREATE_TABLE_USER_PRIVILEGE = "";
		
		//DBUtil
		public static string TABLE_CALC_COLS = "";
	}
}
