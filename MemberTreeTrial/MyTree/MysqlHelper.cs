/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/8/8
 * 时间: 10:40
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace MemberTree
{
	/// <summary>
	/// Description of MysqlHelper.
	/// </summary>
	public class MysqlHelper
	{
		public MysqlHelper()
		{
		}
		
		#region  建立MySql数据库连接
	    /// <summary>
	    /// 建立数据库连接.
	    /// </summary>
	    /// <returns>返回MySqlConnection对象</returns>
	    private MySqlConnection getmysqlcon()
	    {
	        string M_str_sqlcon = "server=114.55.33.130;user id=root;password=passwd;database=test"; //根据自己的设置
	        MySqlConnection myCon = new MySqlConnection(M_str_sqlcon);
	        return myCon;
	    }
	    #endregion
	
	    #region  执行MySqlCommand命令
	    /// <summary>
	    /// 执行MySqlCommand
	    /// </summary>
	    /// <param name="sqlstr">SQL语句</param>
	    public void ExecNoQuery(string sqlstr)
	    {
	        MySqlConnection mysqlcon = this.getmysqlcon();
	        mysqlcon.Open();
	        MySqlCommand mysqlcom = new MySqlCommand(sqlstr, mysqlcon);
	        mysqlcom.ExecuteNonQuery();
	        mysqlcom.Dispose();
	        mysqlcon.Close();
	        mysqlcon.Dispose();
	    }
	    #endregion
	
		#region  创建MySqlDataReader对象
	    /// <summary>
	    /// 创建一个MySqlDataReader对象
	    /// </summary>
	    /// <param name="M_str_sqlstr">SQL语句</param>
	    /// <returns>返回MySqlDataReader对象</returns>
	    public MySqlDataReader ExecReader(string sqlstr)
	    {
	        MySqlConnection mysqlcon = this.getmysqlcon();
	        MySqlCommand mysqlcom = new MySqlCommand(sqlstr, mysqlcon);
	        mysqlcon.Open();
	        MySqlDataReader mysqlread = mysqlcom.ExecuteReader(CommandBehavior.CloseConnection);
	        return mysqlread;
	    }
	    #endregion
	}
}
