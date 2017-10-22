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
using System.Timers;

namespace MemberTree
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowView : UserControl, IPluginView
    {
    	internal static INotify notify = null;
    	
    	//当前登录的用户ID
    	internal static string UserID = "";
    	
    	//用于记录用户在线时长
    	internal static int tickCount = 0;
        private WelcomeView welcomeView;
        private ConnDBView connectView;
        private DatasetListView datasetListView;
        private Window mainWindow;
        private List<MyTreeNode> findResultNodes = new List<MyTreeNode>();

        public WindowView()
        {
            InitializeComponent();
            myStatusBar.SetShowHideView(null, new UIElement[]{leftMenu, mainGrid}, null);
            notify = myStatusBar;
        }
      
        public void Load(IPluginHost host)
		{
			mainWindow = host as Window;
			mainWindow.Title = SysInfo.I.PRODUCT + " - " + SysInfo.I.VERSION;
			
			welcomeView = new WelcomeView();
			welcomeView.InitSelectDB(false, SysInfo.I.PRODUCT + "查看工具", new InvokeBoolDelegate(ConnectDB));
			(mainWindow.Content as Grid).Children.Add(welcomeView);
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
				(mainWindow.Content as Grid).Children.Remove(welcomeView);
				(mainWindow.Content as Grid).Children.Add(connectView);
       		}
		}
        
       	private void SelectDB()
		{
       		datasetListView = new DatasetListView();
       		datasetListView.RefreshDB(MyTrees.treeDB, UserID);
       		datasetListView.SetCallBack(new InvokeStringDelegate(StartUp));
       		
       		Grid main_Grid = mainWindow.Content as Grid;
			if(main_Grid.Children.Contains(welcomeView))
			{
				main_Grid.Children.Remove(welcomeView);
			}
			else if(main_Grid.Children.Contains(connectView))
			{
				main_Grid.Children.Remove(connectView);
			}
			main_Grid.Children.Add(datasetListView);
		}

		private void StartUp(string selectDB)
		{
			(mainWindow.Content as Grid).Children.Remove(datasetListView);
			(mainWindow.Content as Grid).Children.Add(this);
			mainWindow.ResizeMode = ResizeMode.CanResize;
			mainWindow.WindowState = WindowState.Maximized;
			MyTrees.SetDBName(selectDB);
			datasetInfoView.Init(MyTrees.treeDB);
			datasetInfoView.SetCallBack(SwitchTabView);
			mySearchFilter.InitCols();
			listNodes.InitCols();
			myTreeView.myNodeInfo.InitCols();
			myTreeView.btnAllNode.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			
			if(UserID!="")
			{
				//更新用户的最近登陆时间和登陆次数
				UserAdmin.UpdateUserLogin(UserID);
				tickCount = Environment.TickCount;
			}
		}
		
		internal static void Refresh_Online_time()
		{
			if(UserID!="")
			{
				int minutes = (Environment.TickCount-tickCount) / 60000;
				if(minutes > 0)
				{
					UserAdmin.UpdateUserOnline(UserID, minutes);
					tickCount = Environment.TickCount;
				}
			}
		}
        
        #region 左侧按钮事件

        //查找
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
			string searchsql = mySearchFilter.GetSearchSql();
			if(searchsql != null)
			{
				TimingUtil.StartTiming();
				WindowView.notify.SetStatusMessage("正在查询，请稍后。。。");
            	WindowView.notify.SetProcessBarVisible(true);
				List<string> searchParams = mySearchFilter.GetSearchParams();
				searchResults.NodeListView.ItemsSource = MyTrees.FindBySql(searchsql, searchParams);
				WindowView.notify.SetStatusMessage("查询完成！");
				WindowView.notify.SetProcessBarVisible(false);
	        	WindowView.notify.SetStatusMessage(TimingUtil.EndTiming());
			}
			Refresh_Online_time();
        }
 
        //切换视图
        private void SwitchTabView(string tab)
        {
            if (tab == "tree")
            {
                myTreeView.Visibility = Visibility.Visible;
                listNodes.Visibility = Visibility.Collapsed;
            }
            else
            {
                listNodes.Visibility = Visibility.Visible;
                myTreeView.Visibility = Visibility.Collapsed;
                listNodes.SetDataSource(tab);
            }
            Refresh_Online_time();
        }
        
        //用户双击选中进入查看某个查找结果
        private void NodeList_DbClick(object sender, MouseButtonEventArgs e)
        {
            ListView currentList = (sender as SearchResult).NodeListView;
            MyTreeNode selectedNode = currentList.SelectedItem as MyTreeNode;
            if (selectedNode != null)
            {
            	if(MyTrees.GetLeafAloneNodeIds().Contains(selectedNode.SysId))
            	{
            		listNodes.Visibility = Visibility.Visible;
               	 	myTreeView.Visibility = Visibility.Collapsed;
            		listNodes.grpHeader.Text = "孤立的叶子节点";
            		listNodes.nodeList.ItemsSource = new List<MyTreeNode>{selectedNode};
            		datasetInfoView.SelectTab("leaf");
            	}
            	else if(MyTrees.GetRingNodeIds().Contains(selectedNode.SysId))
            	{
            		listNodes.Visibility = Visibility.Visible;
                	myTreeView.Visibility = Visibility.Collapsed;
            		listNodes.grpHeader.Text = "构成闭环的节点";
            		listNodes.nodeList.ItemsSource = new List<MyTreeNode>{selectedNode};
            		datasetInfoView.SelectTab("ring");
            	}
            	else
            	{
	                myTreeView.SetRootNode(selectedNode);
	                SwitchTabView("tree");
	                datasetInfoView.SelectTab("tree");
	                myTreeView.ExpandRootNode(1);//打开一级子节点
            	}
            }
            Refresh_Online_time();
        }
		private void BtnAbout_Click(object sender, RoutedEventArgs e)
		{
			WindowVerLog verLog = new WindowVerLog(false, SysInfo.I.PRODUCT + "查看工具");
			verLog.ShowDialog();
		}
        #endregion
    }
}
