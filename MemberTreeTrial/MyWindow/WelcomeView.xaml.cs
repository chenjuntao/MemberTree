using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MemberTree
{
    /// <summary>
    /// HelpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeView : UserControl
    {
    	private InvokeStringDelegate startupDelegate;
    	private Dictionary<string, string> verLogs = new Dictionary<string, string>();
  		private DatasetBtn selectDatasetBtn = null;
    	public WelcomeView()
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
		
		public void RefreshDB()
		{
			mainPanel.Children.Clear();
			List<DatasetInfo> dbList = new List<DatasetInfo>();
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
