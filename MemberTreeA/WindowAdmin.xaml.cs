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
			forestView.SetCallBack(new InvokeStringDelegate(SelectDB));
			forestView.RefreshDB(MyTrees.treeDB);
		}
		
		private void SelectDB(string selectDB)
		{
			MyTrees.SetDBName(selectDB);
			commonView.Init(MyTrees.treeDB);
		}

        #region 左侧按钮事件
        //检查csv文件合法性
        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
        	CsvErrCheck csvErrCheck = new CsvErrCheck();
        	csvErrCheck.ShowDialog();
        }
        
        //打开文件
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "打开要作为会员树数据源的文件";
            openfileDlg.Filter = "CSV文件|*.csv";
            if (openfileDlg.ShowDialog() == true)
            {
            	progressView.SetCsvFile(openfileDlg.FileName);
            	
                int upperLower = comboToLower.SelectedIndex;
                int DBCSBC = comboToHalf.SelectedIndex;
                int trim = comboTrim.SelectedIndex;
                MyTrees.OpenCSVFile(openfileDlg.FileName, upperLower, DBCSBC, trim);
                
                forestView.RefreshDB(MyTrees.treeDB);

                //显示统计信息
//                commonView.SetSummary(openfileDlg.FileName, 
//                                      MyTrees.ForestNodeCount,
//                                      MyTrees.IdConflictNodes.Count,
//                                      MyTrees.RingNodes.Count,
//                                      MyTrees.IdNullNodes.Count,
//                                      MyTrees.AllNodeCount);
//                tabNoParent.Header = "森林（一共" + MyTrees.NoParentNodes.Count + "棵树）";
//                tabIdConflict.Header = "Id有重复（" + MyTrees.IdConflictNodes.Count + "个）";
//                tabIdNull.Header = "信息不完整（" + MyTrees.IdNullNodes.Count + "个）";
//                tabRingErr.Header = "形成闭环（" + MyTrees.RingNodes.Count + "个）";
            }
        }
        
        //sqlserver导出csv文件
        private void ButtonSqlServer2CSV_Click(object sender, RoutedEventArgs e)
        {
        	ConnDBWindow connDB = new ConnDBWindow();
        	connDB.ShowDialog();
        }
        
        //直连Mysql
        private void ButtonConnectMysql_Click(object sender, RoutedEventArgs e)
        {
        	MessageBox.Show("功能正在开发中！");
        }
        
        #endregion
        
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
		{
			WindowVerLog verLog = new WindowVerLog(true, SysInfo.I.PRODUCT + "管理工具");
			verLog.ShowDialog();
		}
    }

}
