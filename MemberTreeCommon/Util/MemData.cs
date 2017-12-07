/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 2017/12/7
 * 时间: 23:32
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;

namespace MemberTree
{
	/// <summary>
	/// Description of MemData.
	/// </summary>
	public class MemData
	{
		private static string MemDataRoot = "Data";
		public static string MemDataSqlite = MemDataRoot + "/db";
		public static string MemDataMysqlKey = MemDataRoot + "/key";
		public static string MemDataTemp = MemDataRoot + "/temp";
		
		public static void InitMemData()
		{
			if(!Directory.Exists(MemDataRoot))
			{
				Directory.CreateDirectory(MemDataRoot);
			}
			if(!Directory.Exists(MemDataSqlite))
			{
				Directory.CreateDirectory(MemDataSqlite);
			}
			if(!Directory.Exists(MemDataMysqlKey))
			{
				Directory.CreateDirectory(MemDataMysqlKey);
			}
		}
	}
}
