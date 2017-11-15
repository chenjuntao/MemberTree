using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

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

        private static List<string> ringNodeIds = new List<string>();
        //判断闭环是否关闭
        private static bool isRingClose(string id)
        {
            if (MyTrees.RingNodes.ContainsKey(id))
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
            ExpandNode(memberTreeView.Items[0] as TreeViewItem, maxLevel);
        }
        public void ExpandNode(TreeViewItem item, int maxLevel)
        {
            if (levels < maxLevel)
            {
                levels++;
                foreach (TreeViewItem subItem in item.Items)
                {
                    item.IsExpanded = true;
                    ExpandNode(subItem, maxLevel);
                }
                levels--;
            }
        }

        private TreeViewItem NewTreeViewItem(MyTreeNode node)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = node.SysId;
            item.ToolTip = node.ToString();
            item.Tag = node;
            item.MouseEnter += item_MouseEnter;
            return item;
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            foreach (TreeViewItem subItem in item.Items)
            {
                MyTreeNode node = subItem.Tag as MyTreeNode;
                if (isRingClose(node.SysId))
                {
                    return;
                }
                if (!subItem.IsExpanded)
                {
                    if (node != null)
                    {
                    	List<MyTreeNode> childrenNodes = node.ChildrenNodes;
                        foreach (MyTreeNode subNode in childrenNodes)
                        {
                            TreeViewItem grandson = NewTreeViewItem(subNode);
                            subItem.Items.Add(grandson);
                        }
                    }
                }
            }
        }

        void item_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            foreach (TreeViewItem subItem in item.Items)
            {
                if (!subItem.IsExpanded)
                {
                    subItem.Items.Clear();
                }
            }
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isAutoExpand.IsChecked == true)
            {
                TreeViewItem item = sender as TreeViewItem;
                item.IsExpanded = true;
                e.Handled = true;
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
        			MyTreeNodeDB nodeDB = MyTrees.treeDB[node.SysId];
	        		if(nodeDB != null)
	        		{
	        			myNodeInfo.DataContext = nodeDB;
	        		}
        		}
        	}
        }
        
        //显示到顶级根节点
        private void btnUpRootNode_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem oldRootItem = memberTreeView.Items[0] as TreeViewItem;
            MyTreeNode oldRootNode = oldRootItem.Tag as MyTreeNode;
            MyTreeNode newRootNode = MyTrees.FindParentNode(oldRootNode.TopId); 
            if (isRingClose(newRootNode.SysId))
            {
                return;
            }

	        List<MyTreeNode> parentNodes = MyTrees.FindToRootNodeList(oldRootNode.TopId);
	        if(parentNodes.Count > 0)
	        {
	        	//先移除旧的根节点
	        	memberTreeView.Items.Remove(oldRootItem);
	        	
		        for (int i = 0; i < parentNodes.Count; i++) {
		        	MyTreeNode node = parentNodes[i];
		        	//添加新的根节点
            		TreeViewItem newRootItem = NewTreeViewItem(parentNodes[i]);
	            	newRootItem.IsExpanded = true;
	            	newRootItem.Expanded += item_Expanded;
	            	newRootItem.Collapsed += item_Collapsed;
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
            TreeViewItem oldRootItem = memberTreeView.Items[0] as TreeViewItem;
            MyTreeNode oldRootNode = oldRootItem.Tag as MyTreeNode;
            MyTreeNode newRootNode = MyTrees.FindParentNode(oldRootNode.TopId);
            if (isRingClose(newRootNode.SysId))
            {
                return;
            }

            //先移除旧的根节点
            oldRootItem.Expanded -= item_Expanded;
            oldRootItem.Collapsed -= item_Collapsed;
            memberTreeView.Items.Remove(oldRootItem);

            //添加新的根节点
            TreeViewItem newRootItem = NewTreeViewItem(newRootNode);
            newRootItem.IsExpanded = true;
            newRootItem.Expanded += item_Expanded;
            newRootItem.Collapsed += item_Collapsed;
            memberTreeView.Items.Add(newRootItem);

            //新的根节点添加子节点
            List<MyTreeNode> childrenNodes = newRootNode.ChildrenNodes;
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

                List<MyTreeNode> grandNodes = subNode.ChildrenNodes;
                foreach (MyTreeNode grandNode in grandNodes)
                {
                    TreeViewItem grandItem = NewTreeViewItem(grandNode);
                    subItem.Items.Add(grandItem);
                }
            }

            //判断当前根节点是否存在父节点
			if (MyTrees.FindParentNode(newRootNode.TopId) != null)
            {
                btnUpLevelNode.IsEnabled = true;
                btnUpRootNode.IsEnabled = true;
            }
            else
            {
                btnUpLevelNode.IsEnabled = false;
                btnUpRootNode.IsEnabled = false;
            }
        }

        public void SetRootNode(MyTreeNode rootNode)
        {
            memberTreeView.Items.Clear();
            ringNodeIds.Clear();

            if (rootNode != null)
            {
                TreeViewItem rootItem = NewTreeViewItem(rootNode);
                rootItem.Expanded += item_Expanded;
                rootItem.Collapsed += item_Collapsed;

                memberTreeView.Items.Add(rootItem);
                List<MyTreeNode> childrenNodes = rootNode.ChildrenNodes;
                foreach (MyTreeNode subNode in childrenNodes)
                {
                    TreeViewItem subItem = NewTreeViewItem(subNode);
                    rootItem.Items.Add(subItem);
                }

                //判断当前根节点是否存在父节点
                if(MyTrees.FindParentNode(rootNode.TopId)!=null)
                {
                    btnUpLevelNode.IsEnabled = true;
                    btnUpRootNode.IsEnabled = true;
                }
                else
                {
                    btnUpLevelNode.IsEnabled = false;
                    btnUpRootNode.IsEnabled = false;
                }
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
            SetRootNode(MyTrees.RootNode);

            if (MyTrees.NoParentNodes.Count > 0)
            {
                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = "森林（共" + MyTrees.NoParentNodes.Count + "棵树)";
                treeItem.ToolTip = "森林（共" + MyTrees.NoParentNodes.Count + "棵树)";
                treeItem.Expanded += item_Expanded;
                treeItem.Collapsed += item_Collapsed;
                memberTreeView.Items.Add(treeItem);
                foreach (MyTreeNode node in MyTrees.NoParentNodes)
                {
                    TreeViewItem subItem = NewTreeViewItem(node);
                    treeItem.Items.Add(subItem);
                }
            }

            if (MyTrees.RingNodes.Count > 0)
            {
                TreeViewItem ringErrItem = new TreeViewItem();
                ringErrItem.Header = "形成闭环的节点（共" + MyTrees.RingNodes.Count() + "个)";
                ringErrItem.ToolTip = "形成闭环的节点（共" + MyTrees.RingNodes.Count() + "个)";
                //nodeInfoErrItem.Expanded += item_Expanded;
                //nodeInfoErrItem.Collapsed += item_Collapsed;
                memberTreeView.Items.Add(ringErrItem);
                foreach (MyTreeNode node in MyTrees.RingNodes.Values)
                {
                    TreeViewItem subItem = NewTreeViewItem(node);
                    ringErrItem.Items.Add(subItem);
                }
            }

            btnUpLevelNode.IsEnabled = false;
            btnUpRootNode.IsEnabled = false;
        }

        //展开选中项的子项
        private void item_Expand(object sender, RoutedEventArgs e)
        {
        	TreeViewItem treeItem = memberTreeView.SelectedItem as TreeViewItem;
    	    if (treeItem != null)
            {
    	    	MenuItem menu = sender as MenuItem;
            	int expLevel = int.Parse(menu.Tag.ToString());
            	if(expLevel > 10)
            	{
            		string warnTxt = "你确定要一次性展开"+expLevel+"层子项吗？\n展开层级过大可能会由于数据量太大而造成程序卡死。";
            		MessageBoxResult result = MessageBox.Show(warnTxt,"警告",MessageBoxButton.YesNo);
            		if(result == MessageBoxResult.No)
            		{
            			return;
            		}
            	}
            	
            	levels = 0;
        		ExpandNode(treeItem, expLevel);
            }
        }
    }
}


