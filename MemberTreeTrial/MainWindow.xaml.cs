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
using MemberTree;

namespace MemberTreeView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {   	
        public MainWindow()
        {
            InitializeComponent();
            
            this.Title = "会员关系树分析工具 - v5.13 (试用版)";
            welcomeView.SetCallBack(new InvokeStringDelegate(StartUp));
        }

		private void StartUp(string selectDB)
		{
			MyTrees.OpenSampleData(selectDB);
			MyTrees.SetDBName(selectDB);
			
			WindowView windowView = new WindowView();
			mainGrid.Children.Remove(welcomeView);
			mainGrid.Children.Add(windowView);
				
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
