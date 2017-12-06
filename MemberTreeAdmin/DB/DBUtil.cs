/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 09/11/2017
 * 时间: 16:47
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MemberTree
{
	/// <summary>
	/// Description of DBUtil.
	/// </summary>
	public class DBUtil
	{
		//id列长度和名称列长度
		public static int idLen = 1, nameLen = 1;
		//其他可选列的名称
		public static List<string> OptCols = new List<string>();
		//其他可选列的长度
		public static List<int> OptColsLen = new List<int>();
		
		//检查判断表头是否合法
		public static bool CheckHead(string head, string separator, bool confirm)
		{
			idLen = 1;
			nameLen = 1;
			OptCols.Clear();
			OptColsLen.Clear();
			
			string[] heads = head.Split(new String[] { separator }, StringSplitOptions.None);
			for (int i = 3; i < heads.Length; i++) 
			{
				if(heads[i] == "sysid")
				{
					heads[i] = heads[i] + "1";
				}
				else if(heads[i] == "topid")
				{
					heads[i] = heads[i] + "1";
				}
				else if(heads[i] == "name")
				{
					heads[i] = heads[i] + "1";
				}
				else if(heads[i] == "level")
				{
					heads[i] = heads[i] + "1";
				}
				else if(heads[i] == "sublevel")
				{
					heads[i] = heads[i] + "1";
				}
				else if(heads[i] == "subcount")
				{
					heads[i] = heads[i] + "1";
				}
				else if(heads[i] == "subcountall")
				{
					heads[i] = heads[i] + "1";
				}
				while(OptCols.Contains(heads[i]))
				{
					heads[i] = heads[i] + "1";
				}
				OptCols.Add(heads[i]);
				OptColsLen.Add(1);
			}
			if(confirm)
			{
				WindowColsCheck window = new WindowColsCheck(heads);
				if(!(bool)window.ShowDialog())
				{
					return false;
				}
			}
			return true;
		}
		
		//判断各列数据是否合法，并判断各列最大长度
		public static bool CheckLen(string[] row)
		{
			if(row.Length == OptCols.Count + 3)
			{
				if(row[0].Length > idLen)
				{
					idLen = row[0].Length;
				}
				if(row[2].Length > nameLen)
				{
					nameLen = row[2].Length;
				}
				
				for (int i = 0; i < OptColsLen.Count; i++) 
				{
					if(row[i+3].Length > OptColsLen[i])
					{
						OptColsLen[i] = row[i+3].Length;
					}
				}
				return true;
			}
			return false;
		}
		
		//将包含可选列在内的各数据列及其长度构造成sql建表语句字符串
		public static string GetCreateTableSql()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format(RegConfig.config["TABLE_CALC_COLS"], idLen, idLen, nameLen));
			for (int i = 0; i < OptCols.Count; i++) 
			{	
				sb.Append(",");
				sb.Append(OptCols[i]);
				sb.Append(" varchar(");
				sb.Append(OptColsLen[i]);
				sb.Append(")");
			}
			sb.Append(")");
			
			return sb.ToString();
		}
	}
}
