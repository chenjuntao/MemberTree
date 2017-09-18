/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 08/06/2017
 * 时间: 11:20
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
	/// Interaction logic for MyNodeInfo.xaml
	/// </summary>
	public partial class MyNodeInfo : UserControl
	{
		private List<TextBlock> nodeInfoCols = new List<TextBlock>();
		public MyNodeInfo()
		{
			InitializeComponent();
		}
		
		internal void InitCols()
		{
			addCol("会员ID：");
			addCol("父级ID：");
			addCol("会员姓名：");
			addCol("所在等级：");
			addCol("下线层级数：");
			addCol("直接下级数：");
			addCol("下级总数：");
			
			List<string> optCols = MyTrees.GetTableOptCols();
			for (int i = 0; i < optCols.Count; i++) 
			{
				addCol(optCols[i] + "：");
			}
		}
		
		private void addCol(string colName)
		{
			TextBlock header = new TextBlock();
			header.Text = colName;
			leftPanel.Children.Add(header);
			
			TextBlock val = new TextBlock();
			rightPanel.Children.Add(val);
			
			nodeInfoCols.Add(val);
		}
		
		internal void SetNode(string node)
		{
			string[] nodes = node.Split(new String[]{","}, StringSplitOptions.None);

			for (int i = 0; i < nodes.Length; i++) 
			{
				nodeInfoCols[i].Text = nodes[i];
			}
		}
	}
}