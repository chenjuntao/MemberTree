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
    	public WelcomeView()
        {
            InitializeComponent();
    	}
    	
    	public void Init(bool isAdmin)
		{
    		txtHead.Text = SysInfo.I.PRODUCT;
           	txtVer.Text = "版本：" + SysInfo.I.VERSION;
            txtCpy.Text = SysInfo.I.COPYRIGHT;
            txtTel.Text = SysInfo.I.DESCRIPTION;
            txtReg.Text = string.Format("授权给（公司/单位：{0}，用户：{1}）", SoftReg.Com, SoftReg.Usr);
    		
           if(isAdmin)
            {
           		txtSubHead.Text = "管理端-数据处理工具";
            	imgAdmin.Visibility = Visibility.Visible;
            }
            else
            {
            	txtSubHead.Text = "客户端-数据查看工具";
            	imgView.Visibility = Visibility.Visible;
            } 
		}
    }
}
