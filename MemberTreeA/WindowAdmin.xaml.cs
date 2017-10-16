using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MemberTree
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAdmin : UserControl, IPluginAdmin
    {
        internal static INotify notify = null;
        private WelcomeView welcomeView;
        private ConnDBView connectView;
        private Window mainWindow;

        private List<MyTreeNode> findResultNodes = new List<MyTreeNode>();

        public WindowAdmin()
        {
            InitializeComponent();
            myStatusBar.SetShowHideView(progressView, null, new UIElement[]{mainTab});
            notify = myStatusBar;
            adminDataset.windowAdmin = this;
        }
        
		public void Load(IPluginHost host)
		{
			mainWindow = host as Window;
			mainWindow.Title = SysInfo.I.PRODUCT + " - " + SysInfo.I.VERSION;
			
			welcomeView = new WelcomeView();
			welcomeView.InitSelectDB(true, SysInfo.I.PRODUCT + "管理工具", new InvokeBoolDelegate(ConnectDB));
			(mainWindow.Content as Grid).Children.Add(welcomeView);
		}
		
		private void ConnectDB(bool isSqlite)
		{
			MyTrees.InitTreeDB(isSqlite);
       		if(isSqlite)
       		{
       			tabAdminUser.Visibility = Visibility.Hidden;
       			StartUp();
       		}
       		else
       		{
       			connectView = new ConnDBView(new InvokeDelegate(StartUp), true, MyTrees.treeDB);
				(mainWindow.Content as Grid).Children.Remove(welcomeView);
				(mainWindow.Content as Grid).Children.Add(connectView);
       		}
		}
		
		private void StartUp()
		{
			Grid main_Grid = mainWindow.Content as Grid;
			if(main_Grid.Children.Contains(welcomeView))
			{
				main_Grid.Children.Remove(welcomeView);
			}
			else if(main_Grid.Children.Contains(connectView))
			{
				main_Grid.Children.Remove(connectView);
			}
			(mainWindow.Content as Grid).Children.Add(this);
			mainWindow.ResizeMode = ResizeMode.CanResize;
			mainWindow.WindowState = WindowState.Maximized;
			adminDataset.datasetListView.RefreshDB(MyTrees.treeDB);
			UserAdmin.I = new UserAdmin();
		}
    }
}
