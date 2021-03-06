﻿/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/16/2017
 * 时间: 21:55
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// 用户管理访问数据库的类
	/// </summary>
	public class UserAdmin
	{
		private static MyTreeDBMysql db;	
		
		public static void InitDB(IMyTreeDB treeDB)
		{
			db = treeDB as MyTreeDBMysql;
		}
		
		//是否启用用户权限管理
		public static bool UserAdminEnabled
		{
			get
			{
				db.OpenDB();
				db.cmd.CommandText = "select data_name from tree_userprivilege where user_id = 'UserAdminEnabled'";
				MySqlDataReader reader = db.cmd.ExecuteReader();
				bool result = false;
				if(reader.Read())
				{
					result = (reader.GetString(0) == "True");
					reader.Close();
				}
				else //如果不存在该记录，则插入一条默认记录
				{
					reader.Close();
					db.cmd.CommandText = "insert into tree_userprivilege values('UserAdminEnabled', 'False')";
					db.cmd.ExecuteNonQuery();
				}
				db.CloseDB();
				return result;
			}
			set
			{
				db.OpenDB();
				db.cmd.CommandText = "update tree_userprivilege set data_name='" 
					+ value + "' where user_id = 'UserAdminEnabled'";
				db.cmd.ExecuteNonQuery();
				db.CloseDB();
			}
		}
		
		//获取单个用户信息
		public static UserInfo GetUserInfo(string ID)
		{
			UserInfo userInfo = null;
			db.OpenDB();
			db.cmd.CommandText = "select * from tree_userinfo where ID = '" + ID + "'";
			MySqlDataReader reader = db.cmd.ExecuteReader();
			if(reader.Read())
			{
				userInfo = GetUserInfoByReader(reader);
			}
			reader.Close();
			db.CloseDB();
			return userInfo;
		}
		
		//获取用户信息列表
		public static List<UserInfo> GetUserInfoList()
		{
			List<UserInfo> result = new List<UserInfo>();
			db.OpenDB();
			db.cmd.CommandText = "select * from tree_userinfo";
			MySqlDataReader reader = db.cmd.ExecuteReader();
			while(reader.Read())
			{
				result.Add(GetUserInfoByReader(reader));
			}
			reader.Close();
			db.CloseDB();
			return result;
		}
		
		private static UserInfo GetUserInfoByReader(MySqlDataReader reader)
		{
			string id = reader.GetString("id");
			string name = reader.GetString("name");
			string pwd = reader.GetString("pwd");
			string remark = reader.GetString("remark");
			UserInfo userInfo = new UserInfo(id, name, pwd, remark);
			userInfo.Enable = reader.GetBoolean("enable");
			userInfo.CreateDate = reader.GetDateTime("create_date");
			userInfo.ModifyDate = reader.GetDateTime("modify_date");
			userInfo.LastLoginDate = reader.GetDateTime("last_login_date");
			userInfo.LoginTimes = reader.GetInt32("login_times");
			userInfo.OnlineTime = reader.GetInt32("online_time");
			return userInfo;
		}
		
		//添加用户信息
		public static void AddUserInfo(UserInfo userInfo)
		{
			db.OpenDB();
			db.cmd.CommandText = "insert into tree_userinfo values(@id, @name, @pwd,@remark,@enable," +
				"@create_date,@modify_date,@last_login_date,@login_times,@online_time,@due_date,@due_time)";
			db.cmd.Parameters.AddWithValue("@id", userInfo.ID);
			db.cmd.Parameters.AddWithValue("@name", userInfo.Name);
			db.cmd.Parameters.AddWithValue("@pwd", userInfo.Pwd);
			db.cmd.Parameters.AddWithValue("@remark", userInfo.Remark);
			db.cmd.Parameters.AddWithValue("@enable", userInfo.Enable);
			db.cmd.Parameters.AddWithValue("@create_date", userInfo.CreateDate);
			db.cmd.Parameters.AddWithValue("@modify_date", userInfo.ModifyDate);
			db.cmd.Parameters.AddWithValue("@last_login_date", userInfo.LastLoginDate);
			db.cmd.Parameters.AddWithValue("@login_times", userInfo.LoginTimes);
			db.cmd.Parameters.AddWithValue("@online_time", userInfo.OnlineTime);
			db.cmd.Parameters.AddWithValue("@due_date", userInfo.DueDate);
			db.cmd.Parameters.AddWithValue("@due_time", userInfo.DueTime);
			db.cmd.ExecuteNonQuery();
			db.cmd.Parameters.Clear();
			db.CloseDB();
		}
		
		//修改用户信息
		public static void UpdateUserInfo(string id, string name, string remark)
		{
			db.OpenDB();
			db.cmd.CommandText = "update tree_userinfo set name=@name, remark=@remark, modify_date=@modify_date where id=@id";
			db.cmd.Parameters.AddWithValue("@id", id);
			db.cmd.Parameters.AddWithValue("@name", name);
			db.cmd.Parameters.AddWithValue("@remark", remark);
			db.cmd.Parameters.AddWithValue("@modify_date", DateTime.Now);
			db.cmd.ExecuteNonQuery();
			db.cmd.Parameters.Clear();
			db.CloseDB();
		}
		
		//修改用户密码
		public static void UpdateUserPwd(string id, string pwd)
		{
			db.OpenDB();
			db.cmd.CommandText = "update tree_userinfo set pwd=@pwd, modify_date=@modify_date where id=@id";
			db.cmd.Parameters.AddWithValue("@id", id);
			db.cmd.Parameters.AddWithValue("@pwd", pwd);
			db.cmd.Parameters.AddWithValue("@modify_date", DateTime.Now);
			db.cmd.ExecuteNonQuery();
			db.cmd.Parameters.Clear();
			db.CloseDB();
		}
		
		//修改用户启用状态
		public static void UpdateUserEnabled(string id, bool enable)
		{
			db.OpenDB();
			db.cmd.CommandText = "update tree_userinfo set enable=@enable, modify_date=@modify_date where id=@id";
			db.cmd.Parameters.AddWithValue("@id", id);
			db.cmd.Parameters.AddWithValue("@enable", enable);
			db.cmd.Parameters.AddWithValue("@modify_date", DateTime.Now);
			db.cmd.ExecuteNonQuery();
			db.cmd.Parameters.Clear();
			db.CloseDB();
		}
		
		//修改用户登陆次数+1，更新最近登陆时间
		public static void UpdateUserLogin(string id)
		{
			db.OpenDB();
			db.cmd.CommandText = "update tree_userinfo set login_times=login_times+1, last_login_date=@last_login_date where id=@id";
			db.cmd.Parameters.AddWithValue("@id", id);
			db.cmd.Parameters.AddWithValue("@last_login_date", DateTime.Now);
			db.cmd.ExecuteNonQuery();
			db.cmd.Parameters.Clear();
			db.CloseDB();
		}
		
		//修改用户累计登陆在线时长(分钟)
		public static void UpdateUserOnline(string id, int addminutes)
		{
			db.OpenDB();
			db.cmd.CommandText = "update tree_userinfo set online_time=online_time+@add_minutes where id=@id";
			db.cmd.Parameters.AddWithValue("@id", id);
			db.cmd.Parameters.AddWithValue("@add_minutes", addminutes);
			db.cmd.ExecuteNonQuery();
			db.cmd.Parameters.Clear();
			db.CloseDB();
		}
		
		//删除用户信息
		public static void DeleteUserInfo(string id)
		{
			db.OpenDB();
			db.cmd.CommandText = "delete from tree_userinfo where id = '"+ id +"'";
			db.cmd.ExecuteNonQuery();
			db.cmd.CommandText = "delete from tree_userprivilege where user_id = '"+ id +"'";
			db.cmd.ExecuteNonQuery();
			db.CloseDB();
		}
		
		//根据用户ID获取该用户允许访问的数据集列表
		public static List<string> GetAllowDataByUser(string userId)
		{
			List<string> result = new List<string>();
			db.OpenDB();
			db.cmd.CommandText = "select data_name from tree_userprivilege where user_id = '" + userId + "'";
			MySqlDataReader reader = db.cmd.ExecuteReader();
			while(reader.Read())
			{
				result.Add(reader.GetString(0));
			}
			reader.Close();
			db.CloseDB();
			return result;
		}
		
		//根据数据集名称获取有访问权限的用户ID列表
		public static List<string> GetAllowUserByData(string dataName)
		{
			List<string> result = new List<string>();
			db.OpenDB();
			db.cmd.CommandText = "select user_id from tree_userprivilege where data_name = '" + dataName + "'";
			MySqlDataReader reader = db.cmd.ExecuteReader();
			while(reader.Read())
			{
				result.Add(reader.GetString(0));
			}
			reader.Close();
			db.CloseDB();
			return result;
		}
		
		//添加用户权限
		public static void AddUserPrivilege(string userId, string dataName)
		{
			db.OpenDB();
			//先判断是否已经存在
			db.cmd.CommandText = "select * from tree_userprivilege where user_id='" + userId + "' and data_name='" + dataName + "'";
			MySqlDataReader reader = db.cmd.ExecuteReader();
			bool hasExist = reader.Read();
			reader.Close();
			if(!hasExist)
			{
				db.cmd.CommandText = "insert into tree_userprivilege values('" + userId + "','" + dataName + "')";
				db.cmd.ExecuteNonQuery();
			}
			db.CloseDB();
		}
		
		//删除用权限
		public static void DeleteUserPrivilege(string userId, string dataName)
		{
			db.OpenDB();
			db.cmd.CommandText = "delete from tree_userprivilege where user_id='" + userId + "' and data_name='" + dataName + "'";
			db.cmd.ExecuteNonQuery();
			db.CloseDB();
		}
	}
}
