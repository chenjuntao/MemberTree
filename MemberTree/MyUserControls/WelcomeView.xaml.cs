using System;
using System.Collections.Generic;
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
    		
        public WelcomeView(bool isAdmin, string title, InvokeBoolDelegate startupDelegate)
        {
            InitializeComponent();
            this.startupDelegate = startupDelegate;
            
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
        
		void BtnSqlite_Click(object sender, RoutedEventArgs e)
		{
			startupDelegate.Invoke(true);
		}
		
		void BtnMysql_Click(object sender, RoutedEventArgs e)
		{
			startupDelegate.Invoke(false);
		}
    }
}
