/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 08/06/2017
 * 时间: 14:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for SearchFilter.xaml
	/// </summary>
	public partial class SearchFilter : UserControl
	{
		public SearchFilter()
		{
			InitializeComponent();
		}
		
		public string SearchSql()
		{
			StringBuilder sb = new StringBuilder();
			if(sysid.Text.Trim() != "")
			{
				sb.Append(" and sysid = '");
				sb.Append(sysid.Text.Trim());
				sb.Append("'");
			}
			if(topid.Text.Trim() != "")
			{
				sb.Append(" and topid = '");
				sb.Append(topid.Text.Trim());
				sb.Append("'");
			}
			if(name.Text.Trim() != "")
			{
				sb.Append(" and name = '");
				sb.Append(name.Text.Trim());
				sb.Append("'");
			}
			if(account.Text.Trim() != "")
			{
				sb.Append(" and account = '");
				sb.Append(sysid.Text.Trim());
				sb.Append("'");
			}
			if(idcard.Text.Trim() != "")
			{
				sb.Append(" and idcard = '");
				sb.Append(idcard.Text.Trim());
				sb.Append("'");
			}
			if(tel.Text.Trim() != "")
			{
				sb.Append(" and tel = '");
				sb.Append(tel.Text.Trim());
				sb.Append("'");
			}
			if(addr.Text.Trim() != "")
			{
				sb.Append(" and addr like '%");
				sb.Append(addr.Text.Trim());
				sb.Append("%'");
			}
			if(bank.Text.Trim() != "")
			{
				sb.Append(" and bank = '");
				sb.Append(bank.Text.Trim());
				sb.Append("'");
			}
			if(bankcard.Text.Trim() != "")
			{
				sb.Append(" and bankcard = '");
				sb.Append(bankcard.Text.Trim());
				sb.Append("'");
			}
			if(email.Text.Trim() != "")
			{
				sb.Append(" and email = '");
				sb.Append(email.Text.Trim());
				sb.Append("'");
			}

			if(sb.Length>0)
			{
				return "select sysid from treenodes where 1=1" + sb.ToString();
			}
			else
			{
				return "select sysid from treenodes where 1<>1";
			}
			
		}
	}
}