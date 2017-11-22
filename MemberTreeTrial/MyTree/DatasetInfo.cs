/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/10/17
 * 时间: 11:21
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace MemberTree
{
	/// <summary>
	/// Description of DatasetInfo.
	/// </summary>
	public class DatasetInfo
	{
		public static string CSVHeader = "会员Id,上级Id,会员姓名,所在层级,下级层级,直接下级数,总下级会员数";
    	public static void SetCSVHeader(string line)
    	{
    		string[] ary = line.Split(new char[] { ',' });
    		 for (int i = 3; i < ary.Length; i++) 
    		 {
        		CSVHeader+=(","+ary[i]);
        		MyTrees.TableOptCols.Add(ary[i]);
            }
    	}
		
		public string Name;		//数据集名字
		public int ColCount;	//列数
		public int RowCount;	//行数，节点数量
		public DateTime CreateData;	//创建日期
		
		public string GetOtherString()
		{
			string rowStr = RowCount.ToString();
			if(RowCount > 10000)
			{
				int big = RowCount / 10000;
				int small = RowCount % 10000;
				rowStr = string.Format("{0}万{1}", big, small);
			}
			return string.Format("{0}列,{1}条数据,创建日期:{2}", ColCount, rowStr, CreateData);
		}
		
		public int AllNodeCount;
		public int TreeNodeCount;
		public int TreeCount;
		public int LeafCount;
		public int RingCount;
		public int ConflictCount;
	}
}
