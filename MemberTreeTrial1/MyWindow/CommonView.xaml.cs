/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 12/24/2016
 * 时间: 18:41
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
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class CommonView : UserControl
	{
		public MainWindow mainWindow;
		
		public CommonView()
		{
			InitializeComponent();
		}
		
		public void SetSummary(string csvFile, int forest, int idConflict, int ring, int idNull, int allNodeCount)
		{
			this.txtCsvFile.Text = csvFile;
			
			this.txtForest.Text = "构成森林的节点：" + forest;
			this.txtIdConflict.Text = "ID重复的节点：" + idConflict;
			this.txtRing.Text = "构成闭环的节点：" + ring;
			this.txtIdNull.Text = "信息不完整的节点：" + idNull;
			
			this.allNodeCount.Text = "所有节点总数" + allNodeCount 
				+ " = 构成森林的节点数" + forest
				+ " + 形成闭环的节点数" + ring 
				+ " + 信息不完整的节点数" +idNull
				+"\n其中构成森林的节点数，已包含ID重复的节点数" + idConflict;
		}
		
		private void BtnForest_Click(object sender, RoutedEventArgs e)
		{
			mainWindow.tabView.SelectedItem = mainWindow.tabTree;
		}
		
		private void BtnRing_Click(object sender, RoutedEventArgs e)
		{
			mainWindow.tabView.SelectedItem = mainWindow.tabRingErr;
		}
		
		private void BtnIdConflict_Click(object sender, RoutedEventArgs e)
		{
			mainWindow.tabView.SelectedItem = mainWindow.tabIdConflict;
		}
		
		private void BtnIdNull_Click(object sender, RoutedEventArgs e)
		{
			mainWindow.tabView.SelectedItem = mainWindow.tabIdNull;
		}
	}
}