/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 2017/12/5
 * 时间: 18:39
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace SoftRegister
{
	/// <summary>
	/// Description of RegConfig.
	/// </summary>
	public class RegConfig
	{
		internal static Dictionary<string, string> config = new Dictionary<string, string>();
		
		internal static void Init()
		{
			//MyTreeDBMysql
			config.Add("MYSQL_QUERY_ALL_TABLE_NAME", "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'tree' order by create_time");
			config.Add("MYSQL_CREATE_DATABASE", "CREATE DATABASE if not exists tree DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_bin");
			config.Add("MYSQL_CREATE_TABLE_USER_INFO", "create table if not exists tree_userinfo(id varchar(16), name varchar(32), pwd varchar(32), remark varchar(100), enable tinyint, create_date datetime, modify_date datetime, last_login_date datetime, login_times int, online_time int, due_date datetime, due_time int)");
			config.Add("MYSQL_CREATE_TABLE_USER_PRIVILEGE", "create table if not exists tree_userprivilege(user_id varchar(16), data_name varchar(64))");
			
			//DBUtil
			config.Add("TABLE_CALC_COLS", "(sysid varchar({0}), topid varchar({1}), name varchar({2}), level int, sublevel int, subcount int ,subcountall int");
		}
	}
}
