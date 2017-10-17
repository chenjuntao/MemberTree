/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/16/2017
 * 时间: 21:55
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// 用户管理访问数据库的类
	/// </summary>
	public class UserAdmin
	{
		private MyTreeDBAMysql db;	
		
		internal static UserAdmin I;
		internal static void Init()
		{
			I = new UserAdmin();
		}
		private UserAdmin()
		{
			db = MyTrees.treeDB as MyTreeDBAMysql;
		}
		
		//是否启用用户权限管理
		internal bool UserAdminEnabled
		{
			get
			{
				db.OpenDB();
				db.cmd.CommandText = "select table_name from tree_userprivilege where user_id = 'UserAdminEnabled'";
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
				db.cmd.CommandText = "update tree_userprivilege set table_name='" 
					+ value + "' where user_id = 'UserAdminEnabled'";
				db.cmd.ExecuteNonQuery();
				db.CloseDB();
			}
		}
	}
}
