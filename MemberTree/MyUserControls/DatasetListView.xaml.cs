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
		private Button selectDatasetBtn = null;
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
				return selectDatasetBtn.Content.ToString();
			}
		}
		
		public void RefreshDB(IMyTreeDB treeDB)
		{
			mainPanel.Children.Clear();
			List<DatasetInfo> dbList = treeDB.GetDatasets();
			if(dbList!=null && dbList.Count > 0)
			{
				foreach (DatasetInfo db in dbList)
				{
					Button btn = new Button();
					btn.Content = db.Name;
					btn.ToolTip = db.Name + "\n共" + db.ColCount +"列,"+db.RowCount+"条数据";
					btn.Click += Btn_Click;
					btn.Height = 20 + calcBtnHeight(db.RowCount);
					btn.Width = 100 + db.ColCount;
					btn.Background = Brushes.Azure;
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
		
		private int calcBtnHeight(int row)
		{
			if(row<10000)
			{
				return row/200;
			}
			else if(row<100000)
			{
				return 50 + row/2000;
			}
			else if(row<1000000)
			{
				return 100 + row/20000;
			}
			else if(row<10000000)
			{
				return 150 + row/200000;
			}
			else
			{
				return 200 + row/1000000;
			}
		}
		
		public void SetCallBack(InvokeStringDelegate startupDelegate)
		{
			this.startupDelegate = startupDelegate;
		}
		
		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			if(selectDatasetBtn != null)
			{
				if(selectDatasetBtn == btn)
				{
					return;
				}
				selectDatasetBtn.Background = Brushes.Azure;
				selectDatasetBtn.FontWeight = FontWeights.Normal;
				selectDatasetBtn.FontSize = 12;
			}
			btn.Background = Brushes.LightSkyBlue;
			btn.FontWeight = FontWeights.Bold;
			btn.FontSize = 14;
			selectDatasetBtn = btn;
			
			if(startupDelegate != null)
			{
				startupDelegate.Invoke(btn.Content.ToString());
			}
		}
		
		public void DeleteBtn(string btnTxt)
		{
			if(selectDatasetBtn.Content.ToString() == btnTxt)
			{
				mainPanel.Children.Remove(selectDatasetBtn);
				selectDatasetBtn = null;
			}
			else
			{
				foreach (UIElement ele in mainPanel.Children)
				{
					Button btn = ele as Button;
					if(btn.Content.ToString() == btnTxt)
					{
						mainPanel.Children.Remove(btn);
						break;
					}
				}
			}
		}
	}
}