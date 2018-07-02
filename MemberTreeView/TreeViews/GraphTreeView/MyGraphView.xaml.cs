using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeContainer;

namespace MemberTree
{
    /// <summary>
    /// MyGraphView.xaml 的交互逻辑
    /// </summary>
    public partial class MyGraphView : UserControl
    {
        public MyGraphView()
        {
            InitializeComponent();
            InitMyTree();
        }

        private void InitMyTree()
        {
            //加载正确的节点
            List<MyTreeNode> roots = new List<MyTreeNode>();
//            if (MyTreeNode.AllNodes.ContainsKey(1))
//            {
//                roots.AddRange(MyTreeNode.AllNodes[1].Values);
//            }
            Button memBtn = new Button();
            memBtn.Content = "会员列表";
            TreeNode memNode = memberTreeView.AddRoot(memBtn);
            foreach (MyTreeNode node in roots)
            {
                Button rootItem = NewTreeViewItem(node);
                TreeNode rootNode = memberTreeView.AddNode(rootItem, memNode);
                rootNode.Tag = node;
                rootNode.Collapsed = true;

                //List<MyTreeNode> childrenNodes = node.ChildrenNodes;
                //foreach (MyTreeNode subNode in childrenNodes)
                //{
                //    Button subItem = NewTreeViewItem(subNode);
                //    memberTreeView.AddNode(subItem, rootNode).Tag = subNode;
                //}
            }

            //将错误节点也加上
            //foreach (string item in MyTreeNode.ErrNodes.Keys)
            //{
            //    TreeViewItem errRootItem = new TreeViewItem();
            //    errRootItem.Header = item;
            //    errRootItem.ToolTip = "下线人数：" + MyTreeNode.ErrNodes[item].Count;
            //    memberTreeView.Items.Add(errRootItem);
            //    foreach (MyTreeNode errNode in MyTreeNode.ErrNodes[item])
            //    {
            //        TreeViewItem errItem = new TreeViewItem();
            //        errItem.Header = errNode.RealName;
            //        errItem.ToolTip = "Id：" + errNode.SysId + "，级别：" + errNode.Layer + "，出错行数：" + errNode.DescendantCount;
            //        errRootItem.Items.Add(errItem);
            //    }
            //}
        }

        private Button NewTreeViewItem(MyTreeNode subNode)
        {
            Button btn = new Button();
            btn.Content = subNode.Name;
            btn.ToolTip = "级别：" + subNode.Level + "，下线人数：" + subNode.ChildrenCount;
            btn.Click += new RoutedEventHandler(btn_Click);
            btn.MouseEnter += item_MouseEnter;
            btn.MouseMove += item_MouseMove;
            return btn;
        }

        void item_Expanded(TreeNode tn)
        {
            MyTreeNode node = tn.Tag as MyTreeNode;
            if (node != null)
            {
                List<MyTreeNode> childrenNodes = MyTrees.GetNodesByTopId(node.SysId);;
                foreach (MyTreeNode subNode in childrenNodes)
                {
                    Button grandson = NewTreeViewItem(subNode);
                    TreeNode newNode = memberTreeView.AddNode(grandson, tn);
                    newNode.Tag = subNode;
                    newNode.Collapsed = true;
                }
            }
        }

        void item_Collapsed(TreeNode tn)
        {
            memberTreeView.ClearNodeChildren(tn);
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isAutoExpand.IsChecked == true)
            {
                TreeViewItem item = sender as TreeViewItem;
                //Console.WriteLine("===============鼠标进入了" + item.Header);
                //item.IsExpanded = true;
                e.Handled = true;
            }
        }

//        bool isCurrentLeave = false;
        void item_MouseMove(object sender, MouseEventArgs e)
        {
            if (isAutoExpand.IsChecked == true)
            {
                Button item = sender as Button;
                TreeNode tn = (TreeNode)(item.Parent);
                MyTreeNode node = tn.Tag as MyTreeNode;
                if (node != null)
                {
                    statusText.Text = "Id：" + node.SysId + "，TopId：" + node.TopId;
                }
//                isCurrentLeave = true;
                e.Handled = true;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            if (btn != null)
            {
                TreeNode tn = (TreeNode)(btn.Parent);
               
                //if (tn.TreeChildren.Count > 0)
                //{
                    if (tn.Collapsed)
                    {
                        btn.Background = Brushes.White;
                        item_Expanded(tn);
                        tn.Collapsed =false;
                    }
                    else
                    {
                        btn.Background = Brushes.Red;
                        item_Collapsed(tn);
                        tn.Collapsed=true;
                    }
                //}
            }
        }
    }
}
