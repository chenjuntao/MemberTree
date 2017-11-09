using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MemberTree;

namespace MemberTreeView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
    	private ConnDBView connectView;
        private DatasetListView datasetListView;
    	
        public MainWindow()
        {
            InitializeComponent();
            
            this.Title = SysInfo.I.PRODUCT + " - " + SysInfo.I.VERSION;
			welcomeView.InitSelectDB(false, SysInfo.I.PRODUCT + "查看工具", new InvokeBoolDelegate(ConnectDB));
        }
        
        private void ConnectDB(bool isSqlite)
		{
			MyTrees.InitTreeDB(isSqlite);
       		if(isSqlite)
       		{
       			SelectDB();
       		}
       		else
       		{
       			UserAdmin.InitDB(MyTrees.treeDB);
       			connectView = new ConnDBView(new InvokeDelegate(SelectDB), MyTrees.treeDB);
				mainGrid.Children.Remove(welcomeView);
				mainGrid.Children.Add(connectView);
       		}
		}
        
       	private void SelectDB()
		{
       		datasetListView = new DatasetListView();
       		datasetListView.RefreshDB(MyTrees.treeDB, WindowView.UserID);
       		datasetListView.SetCallBack(new InvokeStringDelegate(StartUp));
       		
			if(mainGrid.Children.Contains(welcomeView))
			{
				mainGrid.Children.Remove(welcomeView);
			}
			else if(mainGrid.Children.Contains(connectView))
			{
				mainGrid.Children.Remove(connectView);
			}
			mainGrid.Children.Add(datasetListView);
		}

		private void StartUp(string selectDB)
		{
			MyTrees.SetDBName(selectDB);
			
			WindowView windowView = new WindowView();
			mainGrid.Children.Remove(datasetListView);
			mainGrid.Children.Add(windowView);
				
			this.WindowState = WindowState.Maximized;
			this.ResizeMode = ResizeMode.CanResize;
			this.Width = 900;
			this.Height = 600;
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
    }
}
