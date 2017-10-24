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
    	
    	private void Init(bool isAdmin)
		{
    		txtHead.Text = SysInfo.I.PRODUCT;
           	txtVer.Text = "版本：v" + SysInfo.I.VERSION;
            txtCpy.Text = SysInfo.I.COPYRIGHT;
    		
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
		}
    	
    	//显示修改数据库窗体
        public void InitSelectDB(bool isAdmin, string title, InvokeBoolDelegate startupDelegate)
        {
        	Init(isAdmin);
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
		public void InitVerLog(bool isAdmin)
        { 
			Init(isAdmin);
			selectDBGrid.Visibility = Visibility.Collapsed;
			verGrid.Visibility = Visibility.Visible;
			
			ReadVerLog();
			foreach (string ver in verLogs.Keys) 
			{
				TabItem item = new TabItem();
				item.Header = ver;
				verTab.Items.Add(item);
			}
		}
		
		private void VerTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabItem selectItem = verTab.SelectedItem as TabItem;
			string selectVer = selectItem.Header.ToString();
			if(verLogs.ContainsKey(selectVer))
			{
				verContent.Text = verLogs[selectVer];
			}
		}
		
		//读取日志历史文件
		private void ReadVerLog()
		{
			string logFile = "dll/verlog.dll";
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
							if(verContent != "" && !verLogs.ContainsKey(verHead))
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
