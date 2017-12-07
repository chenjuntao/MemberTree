/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/03/2017
 * 时间: 18:41
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MemberTree
{
	/// <summary>
	/// 数据库会话管理
	/// </summary>
	public class DBSession
	{
		public string SessionName{get;set;}
		public string SessionRemark{get;set;}
		public string ServerIP{get;set;}
		public string UserID{get;set;}
		public string Password{get;set;}
		public string Port{get;set;}

		public DBSession()
		{
			#region 保证sessionName不重名
			int i = 1;
			List<string> sessions = GetSessionNames();
			SessionName = "server_" + i;
			while(sessions.Contains(SessionName))
			{
				i++;
				SessionName = "server_" + i;
			}
			#endregion
			
			ServerIP = "localhost";
			Port = "3306";
		}
		
		private static string GetSessionFileName(string sessionName)
		{
			return MemData.MemDataMysqlKey + "/" + sessionName + ".key";
		}
		
		public static List<string> GetSessionNames()
		{
			DirectoryInfo dir = new DirectoryInfo(MemData.MemDataMysqlKey);
			FileInfo[] files = dir.GetFiles("*.key");
			List<string> sessions = new List<string>();
			foreach (FileInfo file in files) {
				sessions.Add(file.Name.Replace(file.Extension, ""));
			}
			return sessions;
		}
		
		public static DBSession GetSession(string sessionName)
		{
			if(File.Exists(GetSessionFileName(sessionName)))
			{
				string sessionStr = EncryptHelper.FileDecrypt(GetSessionFileName(sessionName));
			    DBSession dbSession = JsonConvert.DeserializeObject<DBSession>(sessionStr);
			    return dbSession;
			}
			return null;
		}
		
		public static void SaveSession(DBSession dbSession)
		{
			string fileName = GetSessionFileName(dbSession.SessionName);
			string secretStr = JsonConvert.SerializeObject(dbSession);
			EncryptHelper.FileEncrypt(fileName, secretStr);
		}
		
		public static void DeleteSession(string sessionName)
		{
			if(File.Exists(GetSessionFileName(sessionName)))
			{
				File.Delete(GetSessionFileName(sessionName));
			}
		}
	}
}
