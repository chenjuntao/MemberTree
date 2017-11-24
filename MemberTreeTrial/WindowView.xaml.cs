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
    public partial class WindowView : UserControl
    {
    	internal static INotify notify = null;
    	
        private List<MyTreeNode> findResultNodes = new List<MyTreeNode>();

        public WindowView()
        {
            InitializeComponent();
            myStatusBar.SetShowHideView(new UIElement[]{leftMenu, mainGrid}, null);
            notify = myStatusBar;
            
            datasetInfoView.Init();
			datasetInfoView.SetCallBack(SwitchTabView);
			mySearchFilter.InitCols();
			listNodes.InitCols();
			myTreeView.myNodeInfo.InitCols();
			myTreeView.btnAllNode.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        
        #region 左侧按钮事件

        //查找
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
			List<string> searchParams = mySearchFilter.GetSearchParams();
			if(searchParams != null)
			{
				TimingUtil.StartTiming();
				WindowView.notify.SetStatusMessage("正在查询，请稍后。。。");
            	WindowView.notify.SetProcessBarVisible(true);
				searchResults.NodeListView.ItemsSource = MyTrees.FindBySql(searchParams);
				WindowView.notify.SetStatusMessage("查询完成！");
				WindowView.notify.SetProcessBarVisible(false);
	        	WindowView.notify.SetStatusMessage(TimingUtil.EndTiming());
			}
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
        }
        
        //用户双击选中进入查看某个查找结果
        private void NodeList_DbClick(object sender, MouseButtonEventArgs e)
        {
            ListView currentList = (sender as SearchResult).NodeListView;
            MyTreeNode selectedNode = currentList.SelectedItem as MyTreeNode;
            if (selectedNode != null)
            {
            	if(MyTrees.LeafAloneNodes.ContainsKey(selectedNode.SysId))
            	{
            		listNodes.Visibility = Visibility.Visible;
               	 	myTreeView.Visibility = Visibility.Collapsed;
            		listNodes.grpHeader.Text = "孤立的叶子节点";
            		listNodes.nodeList.ItemsSource = new List<MyTreeNode>{selectedNode};
            		datasetInfoView.SelectTab("leaf");
            	}
            	else if(MyTrees.RingNodes.ContainsKey(selectedNode.SysId))
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
        }
		private void BtnAboutApp_Click(object sender, RoutedEventArgs e)
		{

		}
		private void BtnAboutUser_Click(object sender, RoutedEventArgs e)
		{

		}
        #endregion
    }
}
