/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/10/17
 * 时间: 10:17
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace MemberTree
{
	/// <summary>
	/// Description of UserInfo.
	/// </summary>
	public class UserInfo
	{
		public string ID;			//编号
		public string Name;			//姓名
		public string Remark;		//备注
		public bool Enable;			//是否启用
		public DateTime CreateDate;		//创建日期
		public DateTime LastLoginDate;	//上次登陆日期
		public int LoginTime;	//登陆次数
		public int OnlineTime;	//登陆时长（分钟为单位）
		public DateTime DueDate;	//限制使用到期日期
		public int DueTime;		//限制使用最大时长（分钟数）
		
		public UserInfo()
		{
		}
	}
}
