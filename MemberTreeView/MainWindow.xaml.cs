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
using Microsoft.Win32;

namespace MemberTreeView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IPluginHost
    {
    	private delegate void InvokeDelegate();
        public MainWindow()
        {
            InitializeComponent();
            
            this.Dispatcher.BeginInvoke(new InvokeDelegate(Init));
            this.Closing += Window_Closing;
        }
        
        public Window GetMainWindow()
		{
        	return this;
		}
		
		public void Init()
		{
			//自动更新
			autoUp.Update();
			mainGrid.Children.Remove(autoUp);
			
			//加载插件
			Assembly ab = Assembly.LoadFrom("dll/MemberTreeV.dll");
            Type[] types = ab.GetTypes();
            foreach (Type t in types)
            {
               //如果某些类实现了预定义的IMsg.IMsgPlug接口，则认为该类适配与主程序(是主程序的插件)
                if (t.GetInterface("IPluginView")!=null)
                {
                	IPluginView pluginView = (IPluginView)ab.CreateInstance(t.FullName);
                	pluginView.Load(this);
                }
            }
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
    }
}
