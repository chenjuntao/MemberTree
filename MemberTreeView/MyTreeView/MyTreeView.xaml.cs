﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MemberTree
{
    /// <summary>
    /// MyTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class MyTreeView : UserControl
    {
        public MyTreeView()
        {
            InitializeComponent();
            
            //让鼠标在TreeView滚动滚轮时ScrollViewer能够滚动
			memberTreeView.PreviewMouseWheel +=	(sender, e) => {
				var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
				eventArg.RoutedEvent = UIElement.MouseWheelEvent;
				eventArg.Source = sender;
				this.memberTreeView.RaiseEvent(eventArg);
			};
        }
        
        #region ExportImg
        internal void BeginExportImg()
        {
        	contentRow.Height = GridLength.Auto;
        	contentCol.Width = GridLength.Auto;
        }
        internal void EndExportImg()
        {
        	contentRow.Height = new GridLength(1, GridUnitType.Star);
        	contentCol.Width = new GridLength(1, GridUnitType.Star);
        }
        #endregion

        private static List<string> ringNodeIds = new List<string>();
        //判断闭环是否关闭
        private static bool isRingClose(string id)
        {
        	if (MyTrees.GetRingNodeIds().Contains(id))
            {
                if (ringNodeIds.Contains(id))
                {
                    return true;
                }
                else
                {
                    ringNodeIds.Add(id);
                }
            }
            return false;
        }

        private static int levels = 0;
        public void ExpandRootNode(int maxLevel)
        {
            levels = 0;
            MyTrees.OpenDB();
            ExpandNode(memberTreeView.Items[0] as TreeViewItem, maxLevel);
            MyTrees.CloseDB();
        }
        public void ExpandNode(TreeViewItem item, int maxLevel)
        {
            if (levels < maxLevel)
            {
                levels++;
                if(item.HasItems)
                {
	                item.IsExpanded = true;
	                WindowView.notify.SetStatusMessage("正在展开节点" + item.Header);
	                foreach (TreeViewItem subItem in item.Items)
	                {
	                	if(subItem.HasItems)
	                	{
	                    	ExpandNode(subItem, maxLevel);
	                	}
	                }
	                levels--;
                }
            }
        }

        private TreeViewItem NewTreeViewItem(MyTreeNode node)
        {
            TreeViewItem item = new TreeViewItem();
            if(node == null)
            {
            	item.Header = "none";
            	item.ToolTip = "none type node";
            }
            else if(node.SysId=="moreNode")
            {
            	item.Header = "双击加载更多节点";
            	item.ToolTip = string.Format("双击加载更多{0}({1})的子节点,已加载节点数:{2},未加载节点数{3}",
        	             node.Name, node.TopId, node.ChildrenCount, node.ChildrenCountAll-node.ChildrenCount);
            	item.Tag = node;
            	item.MouseDoubleClick += moreNode_Load;
            	item.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
	            item.Header = node.Name;
	            item.ToolTip = node.ToString();
	            item.Tag = node;
	            if(node.ChildrenCount>0)
	            {
		            item.Expanded += item_Expanded;
	            }
            }
            return item;
        }
        
        //节点太多，点击加载更多未显示的节点
        void moreNode_Load(object sender, RoutedEventArgs e)
        {
        	TreeViewItem moreItem = sender as TreeViewItem;
        	MyTreeNode node = moreItem.Tag as MyTreeNode;
        	TreeViewItem parentItem = moreItem.Parent as TreeViewItem;
        	if(parentItem!=null)
        	{
	        	parentItem.Items.Remove(moreItem);
	        	
	        	MyTrees.OpenDB();
				List<MyTreeNode> childrenNodes = MyTrees.GetNodesByTopIdPage(node.TopId, node.ChildrenCount);
				MyTrees.CloseDB();
				foreach (MyTreeNode subNode in childrenNodes)
				{
				    TreeViewItem subItem = NewTreeViewItem(subNode);
				    parentItem.Items.Add(subItem);
				    
				    //如果子节点还有孙子节点，则添加一个虚假的孙节点，使该子节点具有折叠的"+"
				    if(subNode.ChildrenCount>0)
				    {
				    	subItem.Items.Add(NewTreeViewItem(null));
				    }
				    
				    WindowView.notify.SetStatusMessage("正在展开节点" + subItem.Header);
				}
				
				//表示分页加载更多的节点
				node.ChildrenCount = node.ChildrenCount + MyTrees.getNodePageSize;
				if(node.ChildrenCount < node.ChildrenCountAll)
				{
					parentItem.Items.Add(NewTreeViewItem(node));
				}
        	}
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
        	TreeViewItem item = e.Source as TreeViewItem;
        	if(item.HasItems && (item.Items[0] as TreeViewItem).Header.ToString() == "none")
        	{
        		//先移除为使该节点具有折叠的"+"而添加的虚假的子节点
            	item.Items.Clear();
            	
	        	if((e.Source as TreeViewItem).IsSelected)
	        	{
	        		MyTrees.OpenDB();
	        	}
	        	
	            MyTreeNode node = item.Tag as MyTreeNode;
	            List<MyTreeNode> childrenNodes = MyTrees.GetNodesByTopIdPage(node.SysId, 0);
	            
	            foreach (MyTreeNode subNode in childrenNodes)
	            {
	                TreeViewItem subItem = NewTreeViewItem(subNode);
	                item.Items.Add(subItem);
	                
	                //如果子节点还有孙子节点，则添加一个虚假的孙节点，使该子节点具有折叠的"+"
	                if(subNode.ChildrenCount>0)
	                {
	                	subItem.Items.Add(NewTreeViewItem(null));
	                }
	                
	                WindowView.notify.SetStatusMessage("正在展开节点" + subItem.Header);
	            }
	            
	            //表示分页加载更多的节点
	            if(node.ChildrenCount>MyTrees.getNodePageSize)
	            {
	            	MyTreeNode moreNode = new MyTreeNode();
	            	moreNode.SysId = "moreNode";
	            	moreNode.Name = node.Name;
	            	moreNode.TopId = node.SysId;
	            	moreNode.ChildrenCount = MyTrees.getNodePageSize;
	            	moreNode.ChildrenCountAll = node.ChildrenCount;
	            	item.Items.Add(NewTreeViewItem(moreNode));
	            }
	            
				if((e.Source as TreeViewItem).IsSelected)
				{
					MyTrees.CloseDB();
				}
        	}
        }
        
        void item_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
        	TreeViewItem selectedItem = e.NewValue as TreeViewItem;
        	if(selectedItem != null)
        	{
        		MyTreeNode node = selectedItem.Tag as MyTreeNode;
        		if(node != null)
        		{
        			MyTrees.OpenDB();
        			string nodeDB = MyTrees.GetStringBySysId(node.SysId);
        			MyTrees.CloseDB();
	        		if(nodeDB != null)
	        		{
	        			myNodeInfo.SetNode(nodeDB);
	        		}
        		}
        	}
        	WindowView.Refresh_Online_time();
        }
        
        //显示到顶级根节点
        private void btnUpRootNode_Click(object sender, RoutedEventArgs e)
        {
        	MyTrees.OpenDB();
            TreeViewItem oldRootItem = memberTreeView.Items[0] as TreeViewItem;
            MyTreeNode oldRootNode = oldRootItem.Tag as MyTreeNode;
            MyTreeNode newRootNode = MyTrees.GetNodeBySysId(oldRootNode.TopId); 
            if (isRingClose(newRootNode.SysId))
            {
                return;
            }

	        List<MyTreeNode> parentNodes = MyTrees.FindToRootNodeList(oldRootNode.TopId);
	        MyTrees.OpenDB();
	        if(parentNodes.Count > 0)
	        {
	        	//先移除旧的根节点
	        	memberTreeView.Items.Remove(oldRootItem);
	        	
		        for (int i = 0; i < parentNodes.Count; i++) {
		        	MyTreeNode node = parentNodes[i];
		        	//添加新的根节点
            		TreeViewItem newRootItem = NewTreeViewItem(parentNodes[i]);
	            	newRootItem.IsExpanded = true;
	            	newRootItem.Items.Add(oldRootItem);
	            	
	            	oldRootItem = newRootItem;
		        }
	        }
            
	        memberTreeView.Items.Add(oldRootItem);
	        
            btnUpLevelNode.IsEnabled = false;
            btnUpRootNode.IsEnabled = false;
        }

        //显示上一级节点
        private void btnUpLevelNode_Click(object sender, RoutedEventArgs e)
        {
        	MyTrees.OpenDB();
            TreeViewItem oldRootItem = memberTreeView.Items[0] as TreeViewItem;
            MyTreeNode oldRootNode = oldRootItem.Tag as MyTreeNode;
            MyTreeNode newRootNode = MyTrees.GetNodeBySysId(oldRootNode.TopId);
            if (isRingClose(newRootNode.SysId))
            {
                return;
            }

            //先移除旧的根节点
            memberTreeView.Items.Remove(oldRootItem);

            //添加新的根节点
            TreeViewItem newRootItem = NewTreeViewItem(newRootNode);
            memberTreeView.Items.Add(newRootItem);
            newRootItem.IsExpanded = true;

            //新的根节点添加子节点
            List<MyTreeNode> childrenNodes = MyTrees.GetNodesByTopIdPage(newRootNode.SysId, 0);
            bool hasNotAdded = true;
            foreach (MyTreeNode subNode in childrenNodes)
            {
                if (hasNotAdded)
                {
                    if (oldRootNode.SysId == subNode.SysId)
                    {
                        newRootItem.Items.Add(oldRootItem);
                        hasNotAdded = false;
                        continue;
                    }
                }
                TreeViewItem subItem = NewTreeViewItem(subNode);
                newRootItem.Items.Add(subItem);

                //如果子节点还有孙子节点，则添加一个虚假的孙节点，使该子节点具有折叠的"+"
                if(subNode.ChildrenCount>0)
                {
                	subItem.Items.Add(NewTreeViewItem(null));
                }
            }
            
            //表示分页加载更多的节点
            if(newRootNode.ChildrenCount>MyTrees.getNodePageSize)
            {
            	MyTreeNode moreNode = new MyTreeNode();
            	moreNode.SysId = "moreNode";
            	moreNode.Name = newRootNode.Name;
            	moreNode.TopId = newRootNode.SysId;
            	moreNode.ChildrenCount = MyTrees.getNodePageSize;
            	moreNode.ChildrenCountAll = newRootNode.ChildrenCount;
            	newRootItem.Items.Add(NewTreeViewItem(moreNode));
            }

            //判断当前根节点是否存在父节点
			if (MyTrees.GetNodeBySysId(newRootNode.TopId) != null)
            {
                btnUpLevelNode.IsEnabled = true;
                btnUpRootNode.IsEnabled = true;
            }
            else
            {
                btnUpLevelNode.IsEnabled = false;
                btnUpRootNode.IsEnabled = false;
            }
            MyTrees.CloseDB();
        }

        public void SetRootNode(MyTreeNode rootNode)
        {
        	pager.Visibility = Visibility.Hidden;
            memberTreeView.Items.Clear();
            ringNodeIds.Clear();

            if (rootNode != null)
            {
                TreeViewItem rootItem = NewTreeViewItem(rootNode);

                memberTreeView.Items.Add(rootItem);
               
                //如果还有子节点，则添加一个节点，使该节点具有折叠的"+"
                if(rootNode.ChildrenCount>0)
                {
                	rootItem.Items.Add(NewTreeViewItem(null));
                }

				MyTrees.OpenDB();
                //判断当前根节点是否存在父节点
                if(MyTrees.GetNodeBySysId(rootNode.TopId)!=null)
                {
                    btnUpLevelNode.IsEnabled = true;
                    btnUpRootNode.IsEnabled = true;
                }
                else
                {
                    btnUpLevelNode.IsEnabled = false;
                    btnUpRootNode.IsEnabled = false;
                }
                MyTrees.CloseDB();
            }
        }

        //获取选中的节点
        public MyTreeNode GetSelectedNode()
        {
        	TreeViewItem selectedItem = memberTreeView.SelectedItem as TreeViewItem;
            if (selectedItem != null)
            {
                MyTreeNode node = (memberTreeView.SelectedItem as TreeViewItem).Tag as MyTreeNode;
                if (node != null)
                {
                	return node;
                }
            }

            return null;
        }

        private void btnAllNode_Click(object sender, RoutedEventArgs e)
        {
        	int treeRootCount = MyTrees.GetTreeRootNodesCount();
        	string rootHeader = "森林（共" + treeRootCount + "棵树）";
        	
        	SetRootNode(null);
        	TreeViewItem treeItem = new TreeViewItem();
            treeItem.Header = rootHeader;
            treeItem.ToolTip = rootHeader;
            memberTreeView.Items.Add(treeItem);
            treeItem.IsExpanded = true;
            
            btnUpLevelNode.IsEnabled = false;
            btnUpRootNode.IsEnabled = false;
            
            pager.Visibility = Visibility.Visible;
            pager.Init(treeRootCount);
            
        	#region 旧代码，一次性加载所有
//        	List<MyTreeNode> treeRootNodes = MyTrees.GetTreeRootNodes();
//        	string rootHeader = "森林（共" + treeRootNodes.Count + "棵树）";
//        	
//        	if(memberTreeView.HasItems)
//        	{
//        		TreeViewItem rootItem = memberTreeView.Items[0] as TreeViewItem;
//        		if(rootItem.Header.Equals(rootHeader))
//	            {
//	            	WindowView.notify.SetStatusMessage("当前已经是所有树视图！");
//	            	return;
//	            }
//        	}
//            
//        	TimingUtil.StartTiming();
//       		WindowView.notify.SetProcessBarVisible(true);
//            WindowView.notify.SetStatusMessage("正在查询所有构成树的节点。。。");
//            SetRootNode(null);
//            
//            int process = 0;
//            if (treeRootNodes.Count > 0)
//            {
//                TreeViewItem treeItem = new TreeViewItem();
//                treeItem.Header = rootHeader;
//                treeItem.ToolTip = rootHeader;
//                memberTreeView.Items.Add(treeItem);
//                treeItem.IsExpanded = true;
//                int loadCount = (treeRootNodes.Count > 10000) ? 10000 : treeRootNodes.Count;
//                for (int i = 0; i < loadCount; i++) {
//                	TreeViewItem subItem = NewTreeViewItem(treeRootNodes[i]);
//                    treeItem.Items.Add(subItem);
//                    //如果还有子节点，则添加一个节点，使该节点具有折叠的"+"
//	                subItem.Items.Add(NewTreeViewItem(null));
//	                
//	                int newProcess = i * 100 / loadCount;
//		            if (newProcess > process)
//		            {
//		            	WindowView.notify.SetStatusMessage("正在向树视图中添加构成树的节点。。。");
//		            	WindowView.notify.SetProcessBarValue(newProcess);
//		            	process = newProcess;
//		            }
//                }
//                
//                if(treeRootNodes.Count > loadCount)
//                {
//	                //太多而无法加载
//	                TreeViewItem moreItem = NewTreeViewItem(null);
//	                treeItem.Items.Add(moreItem);
//	                WindowView.notify.SetStatusMessage("由于树太多而只添加部分树，还有"+(treeRootNodes.Count-loadCount)+"棵树无法加载！");
//                }
//                else
//                {
//                	 WindowView.notify.SetStatusMessage("查询并添加所有树成功！");
//                }
//            }
//
//            btnUpLevelNode.IsEnabled = false;
//            btnUpRootNode.IsEnabled = false;
//            
//            TimingUtil.EndTiming();
//        	WindowView.notify.SetProcessBarVisible(false);
        	#endregion
        }
        
        /// <summary>  
		/// 分页控件回调函数  
		/// </summary>
		/// <param name="pageNo">页码，由分页控件传入</param>
		/// <param name="pageSize">每页记录数</param>
		/// <returns></returns>  
		private void LoadPageData(int pageNo, int pageSize) 
        {
         	List<MyTreeNode> treeRootNodes = MyTrees.GetTreeRootNodes(pageNo, pageSize);
            
            TreeViewItem rootItem = memberTreeView.Items[0] as TreeViewItem;
            rootItem.Items.Clear();
            if (rootItem!=null && treeRootNodes.Count > 0)
            {
            	
                for (int i = 0; i < treeRootNodes.Count; i++) {
                	TreeViewItem subItem = NewTreeViewItem(treeRootNodes[i]);
                	rootItem.Items.Add(subItem);
                    //如果还有子节点，则添加一个节点，使该节点具有折叠的"+"
	                subItem.Items.Add(NewTreeViewItem(null));
                }
            }
        }

        //展开选中项的子项
        private void item_Expand(object sender, RoutedEventArgs e)
        {
        	TreeViewItem treeItem = memberTreeView.SelectedItem as TreeViewItem;
    	    if (treeItem != null)
            {
    	    	MenuItem menu = sender as MenuItem;
            	int expLevel = int.Parse(menu.Tag.ToString());
            	MyTreeNode node = treeItem.Tag as MyTreeNode;
            	if(expLevel > 1000 && node.ChildrenCountAll > 10000)
            	{
            		string warnTxt = "你确定要一次性展开全部子节点吗？\n展开层级过大可能会由于数据量太大而造成程序卡死。";
            		MessageBoxResult result = MessageBox.Show(warnTxt,"警告",MessageBoxButton.YesNo);
            		if(result == MessageBoxResult.No)
            		{
            			return;
            		}
            	}
            	
            	levels = 0;
            	TimingUtil.StartTiming();
            	MyTrees.OpenDB();
            	WindowView.notify.SetProcessBarVisible(true);
        		ExpandNode(treeItem, expLevel);
        		WindowView.notify.SetProcessBarVisible(false);
        		MyTrees.CloseDB();
        		WindowView.notify.SetStatusMessage(TimingUtil.EndTiming());
            }
        }
        
        //切换树状视图的显示风格
        private void TreeStyle_Change(object sender, RoutedEventArgs e)
        {
        	MenuItem menu = sender as MenuItem;
        	if(!menu.IsChecked)
        	{
        		foreach (MenuItem item in menuTreeStyle.Items) 
        		{
        			item.IsChecked = false;
        		}
        		menu.IsChecked = true;
	        	switch (menu.Header.ToString())
	            {
	                case "最简风格树视图":
	                    memberTreeView.Resources.Clear();
	                    break;
	                case "带连接线树视图":
	                    memberTreeView.Resources.Source = new Uri("LineTreeStyle.xaml", UriKind.RelativeOrAbsolute);
	                    break;
	                case "水平组织结构树视图":
	                    memberTreeView.Resources.Source = new Uri("GraphTreeStyle.xaml", UriKind.RelativeOrAbsolute);
	                    break;
	                case "垂直组织结构树视图":
	                    memberTreeView.Resources.Source = new Uri("GraphTreeVStyle.xaml", UriKind.RelativeOrAbsolute);
	                    break;
	                case "盒状风格树视图":
	                    memberTreeView.Resources.Source = new Uri("BoxTreeStyle.xaml", UriKind.RelativeOrAbsolute);
	                    break;
	                default:
	                    break;
	            }
        	}
        }
        
         #region 导出
                
        //导出选中的树节点到csv表格
        private void ButtonExportSelectedNode_Click(object sender, RoutedEventArgs e)
        {
        	MyTreeNode node = GetSelectedNode();
        	ExportCSV.ExportNodes(node);
        	WindowView.Refresh_Online_time();
        }

        //导出图片
        private void btnExportImg_Click(object sender, RoutedEventArgs e)
        {
        	MyTreeNode node = GetSelectedNode();
        	ExportIMG.SaveNode2Image(this, node);
        	WindowView.Refresh_Online_time();
        }
        
        //导出PDF
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            MyTreeNode node = GetSelectedNode();
        	ExportPDF.Export2PDF(this, node);
        	WindowView.Refresh_Online_time();
        }

        //打印
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.UserPageRangeEnabled = true;

            if (printDlg.ShowDialog() == true)
            {
                 printDlg.PrintVisual(memberTreeView, "打印当前会员树视图");
            }
            WindowView.Refresh_Online_time();
        }
        #endregion
    }
}


