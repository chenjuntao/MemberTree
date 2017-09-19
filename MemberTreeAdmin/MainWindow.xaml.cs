using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using MemberTree;

namespace MemberTreeAdmin
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
			Assembly ab = Assembly.LoadFrom("dll/MemberTreeA.dll");
            Type[] types = ab.GetTypes();
            foreach (Type t in types)
            {
               //如果某些类实现了预定义的IMsg.IMsgPlug接口，则认为该类适配与主程序(是主程序的插件)
                if (t.GetInterface("IPluginAdmin")!=null)
                {
                	IPluginAdmin pluginAdmin = (IPluginAdmin)ab.CreateInstance(t.FullName);
                	pluginAdmin.Load(this);
                }
            }
		}
    }
}
