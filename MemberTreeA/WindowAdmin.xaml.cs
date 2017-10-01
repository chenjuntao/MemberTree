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
    public partial class WindowAdmin : UserControl, IPluginAdmin
    {
        internal static INotify notify = null;
        private WelcomeView welcomeView;
        private Window mainWindow;

        private List<MyTreeNode> findResultNodes = new List<MyTreeNode>();

        public WindowAdmin()
        {
            InitializeComponent();
            myStatusBar.SetShowHideView(progressView, null, new UIElement[]{grpboxTree, grpboxUser});
            notify = myStatusBar;
            adminDataset.windowAdmin = this;
        }
        
		public void Load(IPluginHost host)
		{
			mainWindow = host as Window;
			mainWindow.Title = SysInfo.I.PRODUCT + " - " + SysInfo.I.VERSION;
			
			welcomeView = new WelcomeView();
			welcomeView.InitSelectDB(true, SysInfo.I.PRODUCT + "管理工具", new InvokeBoolDelegate(StartUp));
			(mainWindow.Content as Grid).Children.Add(welcomeView);
		}
		
		private void StartUp(bool isSqlite)
		{
			MyTrees.InitTreeDB(isSqlite);
			(mainWindow.Content as Grid).Children.Remove(welcomeView);
			(mainWindow.Content as Grid).Children.Add(this);
			mainWindow.ResizeMode = ResizeMode.CanResize;
			mainWindow.WindowState = WindowState.Maximized;
			adminDataset.forestView.RefreshDB(MyTrees.treeDB);
		}
    }
}
