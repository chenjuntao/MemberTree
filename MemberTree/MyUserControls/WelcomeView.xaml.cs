using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// HelpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeView : UserControl
    {
    	private InvokeBoolDelegate startupDelegate;
    	private Dictionary<string, string> verLogs = new Dictionary<string, string>();
  	
    	public WelcomeView()
        {
            InitializeComponent();
    	}
    	
    	private void Init(bool isAdmin, string title)
		{
           if(isAdmin)
            {
            	imgAdmin.Visibility = Visibility.Visible;
            	isAdmin = true;
            }
            else
            {
            	imgView.Visibility = Visibility.Visible;
            	isAdmin = true;
            }
            txtHead.Text = title;
           	txtVer.Text = "版本：v" + SysInfo.I.VERSION + "    by " + SysInfo.I.COMPANY;
            txtCpy.Text = SysInfo.I.COPYRIGHT;
		}
    	
    	//显示修改数据库窗体
        public void InitSelectDB(bool isAdmin, string title, InvokeBoolDelegate startupDelegate)
        {
        	Init(isAdmin, title);
            this.startupDelegate = startupDelegate;
        }
        
		void BtnSqlite_Click(object sender, RoutedEventArgs e)
		{
			startupDelegate.Invoke(true);
		}
		
		void BtnMysql_Click(object sender, RoutedEventArgs e)
		{
			startupDelegate.Invoke(false);
		}
		
		//显示版本修改历史窗体
		public void InitVerLog(bool isAdmin, string title)
        { 
			Init(isAdmin, title);
			mainGrid.Visibility = Visibility.Collapsed;
			mainTab.Visibility = Visibility.Visible;
			
			ReadVerLog();
			foreach (string ver in verLogs.Keys) 
			{
				TabItem tab = new TabItem();
				tab.Header = ver;
				tab.Content = verLogs[ver];
				mainTab.Items.Add(tab);
			}
		}
		
		//读取日志历史文件
		private void ReadVerLog()
		{
			string logFile = "dll/Ver/verlog.dll";
			if(File.Exists(logFile))
			{
				using(StreamReader mysr = new StreamReader(logFile, Encoding.UTF8))
				{
					string verHead = "";
					string verContent = "";
					while(!mysr.EndOfStream)
	                {
						string line = mysr.ReadLine();
						if(line.StartsWith("ver"))
						{
							if(verContent != "")
							{
								verLogs.Add(verHead, verContent);
								verContent = "";
							}
							verHead = line;
						}
						else
						{
							verContent += Environment.NewLine + line;
						}
					}
				}
			}
		}
    }
}
