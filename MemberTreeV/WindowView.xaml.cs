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
using Microsoft.Win32;

namespace MemberTree
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowView : UserControl, IPluginView
    {
        internal static INotify notify = null;
        private WelcomeView welcomeView;
        private ForestsView forestView;
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
			
			welcomeView = new WelcomeView(false, SysInfo.I.PRODUCT + "查看工具", new InvokeBoolDelegate(SelectDB));
			(mainWindow.Content as Grid).Children.Add(welcomeView);
		}
        
       	private void SelectDB(bool isSqlite)
		{
       		MyTrees.InitTreeDB(isSqlite);
       		forestView = new ForestsView();
       		forestView.RefreshDB(MyTrees.treeDB);
       		forestView.SetCallBack(new InvokeStringDelegate(StartUp));
			(mainWindow.Content as Grid).Children.Remove(welcomeView);
			(mainWindow.Content as Grid).Children.Add(forestView);
		}
       	
		private void StartUp(string selectDB)
		{
			(mainWindow.Content as Grid).Children.Remove(forestView);
			(mainWindow.Content as Grid).Children.Add(this);
			mainWindow.ResizeMode = ResizeMode.CanResize;
			mainWindow.WindowState = WindowState.Maximized;
			MyTrees.SetDBName(selectDB);
			commonView.Init(MyTrees.treeDB);
			commonView.SetCallBack(SwitchTabView);
			mySearchFilter.InitCols();
			listNodes.InitCols();
			myTreeView.myNodeInfo.InitCols();
			myTreeView.btnAllNode.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
		}

        #region 左侧按钮事件

        //查找
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
			string searchsql = mySearchFilter.SearchSql();
			searchResults.NodeListView.ItemsSource = MyTrees.FindBySql(searchsql);
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
                if (tab == "conflict")
	            {
	        		listNodes.grpHeader.Text = "ID重复的节点";
	            	listNodes.nodeList.ItemsSource = MyTrees.GetIdConflictNodes();
	            }
	            else if (tab == "leaf")
	            {
	            	listNodes.grpHeader.Text = "孤立的叶子节点";
	            	listNodes.nodeList.ItemsSource = MyTrees.GetLeafAloneNodes().Values;
	            }
	            else if (tab == "ring")
	            {
	            	listNodes.grpHeader.Text = "构成闭环的节点";
	            	listNodes.nodeList.ItemsSource = MyTrees.GetRingNodes().Values;
	            }
            }
        }
        
        //用户双击选中进入查看某个查找结果
        private void NodeList_DbClick(object sender, MouseButtonEventArgs e)
        {
            ListView currentList = (sender as SearchResult).NodeListView;
            MyTreeNode selectedNode = currentList.SelectedItem as MyTreeNode;
            if (selectedNode != null)
            {
            	if(MyTrees.GetLeafAloneNodes().ContainsKey(selectedNode.SysId))
            	{
            		listNodes.Visibility = Visibility.Visible;
               	 	myTreeView.Visibility = Visibility.Collapsed;
            		listNodes.grpHeader.Text = "孤立的叶子节点";
            		listNodes.nodeList.ItemsSource = new List<MyTreeNode>{selectedNode};
            		commonView.SelectTab("leaf");
            	}
            	else if(MyTrees.GetRingNodes().ContainsKey(selectedNode.SysId))
            	{
            		listNodes.Visibility = Visibility.Visible;
                	myTreeView.Visibility = Visibility.Collapsed;
            		listNodes.grpHeader.Text = "构成闭环的节点";
            		listNodes.nodeList.ItemsSource = new List<MyTreeNode>{selectedNode};
            		commonView.SelectTab("ring");
            	}
            	else
            	{
	                myTreeView.SetRootNode(selectedNode);
	                SwitchTabView("tree");
	                commonView.SelectTab("tree");
	                myTreeView.ExpandRootNode(1);//打开一级子节点
            	}
            }
        }
        #endregion
    }
}
