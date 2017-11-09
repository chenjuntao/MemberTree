using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MemberTree
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAdmin : UserControl
    {
        internal static INotify notify = null;

        private List<MyTreeNode> findResultNodes = new List<MyTreeNode>();

        public WindowAdmin()
        {
            InitializeComponent();
            
            myStatusBar.SetShowHideView(progressView, null, new UIElement[]{mainTab});
            notify = myStatusBar;
            adminDataset.windowAdmin = this;

            adminDataset.datasetListView.RefreshDB(MyTrees.treeDB, "");
            if(MyTrees.treeDB is MyTreeDBAMysql)
            {
            	tabAdminUser.Visibility = Visibility.Visible;
            	adminUser.InitUserAdmin();
            }
		}
		
		//主TabControl进行Tab页面切换
		void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			notify.SetStatusMessage("进入" + (sender as TextBlock).Text);
		}
    }
}
