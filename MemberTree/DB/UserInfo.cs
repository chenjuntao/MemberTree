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
		public string ID{get;set;}			//编号
		public string Name{get;set;}		//姓名
		public string Pwd{get;set;}			//密码
		public string Remark{get;set;}		//备注
		public bool Enable{get;set;}		//状态：启用，停用
		public DateTime CreateDate;			//创建时间
		public DateTime ModifyDate;			//修改时间
		public DateTime LastLoginDate;		//上次登陆时间
		public int LoginTimes{get;set;}		//登陆次数
		public int OnlineTime{get;set;}		//登陆时长（分钟为单位）
		public DateTime DueDate{get;set;}	//限制使用到期日期
		public int DueTime{get;set;}		//限制使用最大时长（分钟数）
		
		public string CreateDateStr
		{
			get{return CreateDate.ToString("yyyy-MM-dd hh:mm:ss"); }
		}
		
		public string ModifyDateStr
		{
			get{return ModifyDate.ToString("yyyy-MM-dd hh:mm:ss"); }
		}
		
		public string LastLoginDateStr
		{
			get{return LastLoginDate.ToString("yyyy-MM-dd hh:mm:ss"); }
		}
		
		public UserInfo(string id, string name, string pwd, string remark)
		{
			this.ID = id;
			this.Name = name;
			this.Pwd = pwd;
			this.Remark = remark;
			this.Enable = true;
			this.CreateDate = DateTime.Now;
			this.ModifyDate = DateTime.Now;
			this.DueDate = DateTime.MaxValue;
			this.DueTime = int.MaxValue;
		}
		
		public string ToShortString()
		{
			return string.Format("{0}({1})", Name, ID);
		}
		
		public string ToLongString()
		{
			return string.Format("用户 ID：{0}, 姓名：{1}, 备注信息：{2}", ID, Name, Remark);
		}
	}
}
