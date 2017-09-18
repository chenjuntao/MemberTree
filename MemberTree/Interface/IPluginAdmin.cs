/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/8/29
 * 时间: 11:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Windows.Controls;

namespace MemberTree
{
	/// <summary>
	/// Description of IPluginAdmin.
	/// </summary>
	public interface IPluginAdmin
	{
		void Load(IPluginHost host);
	}
}
