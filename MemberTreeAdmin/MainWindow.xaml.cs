using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MemberTree;

namespace MemberTreeAdmin
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
    	private ConnDBView connectView;
        public MainWindow()
        {
            InitializeComponent();
            
            MemData.InitMemData();
            
            if(SoftReg.hasReged())
            {
            	this.Title = SysInfo.I.PRODUCT + " - " + SysInfo.I.VERSION;
				loginView.InitSelectDB(true, new InvokeBoolDelegate(ConnectDB));
            }
            else
            {
            	this.Title = "网络传销会员层级分析系统——软件授权注册向导";
            	mainGrid.Children.Remove(loginView);
            	mainGrid.Children.Add(new SoftRegView());
            }
        }
        
        private void ConnectDB(bool isSqlite)
		{
			MyTrees.InitTreeDB(isSqlite);
			
       		if(isSqlite)
       		{

       			StartUp();
       		}
       		else
       		{
       			connectView = new ConnDBView(new InvokeDelegate(StartUp), MyTrees.treeDB);
				mainGrid.Children.Remove(loginView);
				mainGrid.Children.Add(connectView);
       		}
		}
        
        private void StartUp()
		{
			if(mainGrid.Children.Contains(loginView))
			{
				mainGrid.Children.Remove(loginView);
			}
			else if(mainGrid.Children.Contains(connectView))
			{
				mainGrid.Children.Remove(connectView);
			}
			
			WindowAdmin windowAdmin = new WindowAdmin();
			mainGrid.Children.Add(windowAdmin);
			
			this.WindowState = WindowState.Maximized;
			this.ResizeMode = ResizeMode.CanResize;
			this.Width = 1000;
			this.Height = 700;
		}
        
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
    }
}
