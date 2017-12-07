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
            
            if(SoftReg.hasReged())
            {
            	this.Title = SysInfo.I.PRODUCT + " - " + SysInfo.I.VERSION;
				loginView.InitSelectDB(false, new InvokeBoolDelegate(ConnectDB));
            }
            else
            {
            	this.Title = "网络传销会员层级分析系统——软件授权注册向导";
            	mainGrid.Children.Remove(loginView);
            	mainGrid.Children.Add(new SoftRegView());
            }
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
				mainGrid.Children.Remove(loginView);
				mainGrid.Children.Add(connectView);
       		}
		}
        
       	private void SelectDB()
		{
       		datasetListView = new DatasetListView();
       		datasetListView.RefreshDB(MyTrees.treeDB, WindowView.UserID);
       		datasetListView.SetCallBack(new InvokeStringDelegate(StartUp));
       		
			if(mainGrid.Children.Contains(loginView))
			{
				mainGrid.Children.Remove(loginView);
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
			this.Width = 1000;
			this.Height = 700;
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
    }
}
