/*
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
	/// 所有数据集的集合列表
	/// </summary>
	public partial class DatasetListView : UserControl
	{
		private InvokeStringDelegate startupDelegate;
		private DatasetBtn selectDatasetBtn = null;
		public DatasetListView()
		{
			InitializeComponent();
		}
		
		public string GetSelectDataset()
		{
			if(selectDatasetBtn == null)
			{
				return null;
			}
			else
			{
				return selectDatasetBtn.DatasetName;
			}
		}
		
		public void RefreshDB(IMyTreeDB treeDB, string userId)
		{
			mainPanel.Children.Clear();
			List<DatasetInfo> dbList = treeDB.GetDatasets();
			if(userId != "") //如果用户权限启用，则进行用户数据集权限筛选
			{
				List<DatasetInfo> allowDbList = new List<DatasetInfo>();
				List<string> allowDbName = UserAdmin.GetAllowDataByUser(userId);
				foreach (DatasetInfo db in dbList) 
				{
					if(allowDbName.Contains(db.Name))
					{
						allowDbList.Add(db);
					}
				}
				dbList = allowDbList;
			}
			if(dbList.Count > 0)
			{
				foreach (DatasetInfo db in dbList)
				{
					DatasetBtn btn = new DatasetBtn(db);
					btn.MouseDown += Btn_Click;
					btn.Background = Brushes.Azure;
					mainPanel.Children.Add(btn);
				}
			}
			else
			{
				Button btn = new Button();
				btn.Content = "没有发现可用的数据！";
				btn.Height = 50;
				btn.Width = 200;
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
			DatasetBtn btn = sender as DatasetBtn;
			if(selectDatasetBtn != null)
			{
				if(selectDatasetBtn == btn)
				{
					return;
				}
				selectDatasetBtn.UnSelect();
			}
			btn.Select();
			selectDatasetBtn = btn;
			
			if(startupDelegate != null)
			{
				startupDelegate.Invoke(btn.DatasetName);
			}
		}
		
		public void DeleteBtn(string btnTxt)
		{
			if(selectDatasetBtn.DatasetName == btnTxt)
			{
				mainPanel.Children.Remove(selectDatasetBtn);
				selectDatasetBtn = null;
			}
			else
			{
				foreach (UIElement ele in mainPanel.Children)
				{
					DatasetBtn btn = ele as DatasetBtn;
					if(btn.DatasetName == btnTxt)
					{
						mainPanel.Children.Remove(btn);
						break;
					}
				}
			}
		}
	}
}