/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/8/30
 * 时间: 10:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Reflection;

namespace MemberTree
{
	/// <summary>
	/// Description of SysInfo.
	/// </summary>
	public class SysInfo
	{
		#region singleton
		private static SysInfo instance;
		public static SysInfo I
		{
			get
			{
				if(instance == null)
				{
					instance = new SysInfo();
				}
				return instance;
			}
		}
		#endregion
		
		public string VERSION = "v" + Assembly.GetExecutingAssembly().GetName().Version;
		public string PRODUCT = (Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute).Product;
		public string COMPANY = (Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute).Company;
		public string COPYRIGHT = (Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute).Copyright;
		public string DESCRIPTION = (Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute).Description;
		
	}
}
