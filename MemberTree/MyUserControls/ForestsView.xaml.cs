﻿/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 08/30/2017
 * 时间: 16:13
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
	/// Interaction logic for ForestsView.xaml
	/// </summary>
	public partial class ForestsView : UserControl
	{
		private InvokeStringDelegate startupDelegate;
		
		public ForestsView()
		{
			InitializeComponent();
		}
		
		public void RefreshDB(IMyTreeDB treeDB)
		{
			mainPanel.Children.Clear();
			List<string> dbList = treeDB.GetDBs();
			if(dbList!=null && dbList.Count > 0)
			{
				foreach (string db in dbList)
				{
					Button btn = new Button();
					btn.Content = db;
					btn.ToolTip = db;
					btn.Click += Btn_Click;
					btn.Height = 50;
					btn.Width = 150;
					mainPanel.Children.Add(btn);
				}
			}
			else
			{
				Button btn = new Button();
				btn.Content = "没有发现可用的数据！";
				btn.Height = 50;
				btn.Width = 185;
				btn.Background = Brushes.Red;
				mainPanel.Children.Add(btn);
			}
		}
		
		public void SetCallBack(InvokeStringDelegate startupDelegate)
		{
			this.startupDelegate = startupDelegate;
		}
		
		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			if(startupDelegate != null)
			{
				Button btn = sender as Button;
				startupDelegate.Invoke(btn.Content.ToString());
			}
		}
	}
}