/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 12/16/2014
 * 时间: 17:33
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
	/// Interaction logic for MyForestList.xaml
	/// </summary>
	public partial class MyForest : UserControl
	{
		public MyForest()
		{
			InitializeComponent();
		}
		
		private List<MyTreeNode> forest;
		public void SetForest(List<MyTreeNode> nodeList)
		{
			if (forest != nodeList) {
				forest = nodeList;
				foreach (var tree in forest) {
            	
				}
			}
		}
	}
}