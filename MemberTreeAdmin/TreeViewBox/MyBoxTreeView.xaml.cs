﻿using System;
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

namespace MemberTree
{
    /// <summary>
    /// MyGraphView.xaml 的交互逻辑
    /// </summary>
    public partial class MyBoxTreeView : UserControl
    {
        public MyBoxTreeView()
        {
            InitializeComponent();
            InitMyTree();
        }

        private void InitMyTree()
        {
            //加载所有根节点
            foreach (MyTreeNode node in MyTreeNode.ParentErrNodes)
            {
                TreeViewItem rootItem = NewTreeViewItem(node);
                rootItem.Expanded += item_Expanded;
                rootItem.Collapsed += item_Collapsed;

                memberTreeView.Items.Add(rootItem);
                List<MyTreeNode> childrenNodes = node.ChildrenNodes;
                foreach (MyTreeNode subNode in childrenNodes)
                {
                    TreeViewItem subItem = NewTreeViewItem(subNode);
                    rootItem.Items.Add(subItem);
                }
            }
        }

        private TreeViewItem NewTreeViewItem(MyTreeNode subNode)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = subNode.RealName + "，级别：" + subNode.Level + "，下线人数：" + subNode.DescendantCount;
            item.Tag = subNode;
            item.MouseEnter += item_MouseEnter;
            //item.MouseLeave += item_MouseLeave;
            item.MouseMove += item_MouseMove;
            return item;
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            foreach (TreeViewItem subItem in item.Items)
            {
                MyTreeNode node = subItem.Tag as MyTreeNode;
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

        void item_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            foreach (TreeViewItem subItem in item.Items)
            {
                subItem.Items.Clear();
            }
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isAutoExpand.IsChecked == true)
            {
                TreeViewItem item = sender as TreeViewItem;
                //Console.WriteLine("===============鼠标进入了" + item.Header);
                item.IsExpanded = true;
                e.Handled = true;
            }
        }

        bool isCurrentLeave = false;
        void item_MouseMove(object sender, MouseEventArgs e)
        {
            if (isAutoExpand.IsChecked == true)
            {
                TreeViewItem item = sender as TreeViewItem;
                //Console.WriteLine("$$$$$$$$$$鼠标移动了"+item.Header);
                MyTreeNode node = item.Tag as MyTreeNode;
                if (node != null)
                {
                    statusText.Text = "Id：" + node.SysId + "，TopId：" + node.TopId;
                }
                isCurrentLeave = true;
                e.Handled = true;
            }
        }

        //void item_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (isAutoExpand.IsChecked == true && isCurrentLeave)
        //    {
        //        TreeViewItem item = sender as TreeViewItem;
        //        //Console.WriteLine("........................鼠标退出了" + item.Header);
        //        item.IsExpanded = false;
        //        isCurrentLeave = false;
        //    }
        //}
    }
}