/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/01/2017
 * 时间: 21:39
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for AdminDataSet.xaml
	/// </summary>
	public partial class AdminDataSet : UserControl
	{
		internal WindowAdmin windowAdmin;
		public AdminDataSet()
		{
			InitializeComponent();
			datasetListView.SetCallBack(new InvokeStringDelegate(SelectDB));
		}
		
		private void SelectDB(string selectDB)
		{
			MyTrees.SetDBName(selectDB);
			datasetInfoView.Init(MyTrees.treeDB);
		}
		
        //检查csv文件合法性
        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
        	CsvErrCheck csvErrCheck = new CsvErrCheck();
        	csvErrCheck.ShowDialog();
        }
        
        //打开文件
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
        	bool csv_or_tab = (sender == btnImportCsv);
            OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "打开要作为会员树数据源的文件";
            openfileDlg.Filter = csv_or_tab ? "CSV逗号分隔文件|*.csv" : "TAB键分割文件|*.tab";
            if (openfileDlg.ShowDialog() == true)
            {
            	windowAdmin.progressView.SetCsvFile(openfileDlg.FileName);
            	TextUtil.enUpperLower = (EnumUpperLower)comboToLower.SelectedIndex;
                TextUtil.enDBCSBC = (EnumDBCSBC)comboToHalf.SelectedIndex;
                TextUtil.enTrim = (EnumTrim)comboTrim.SelectedIndex;
                string separator = csv_or_tab ? "," : "\t";
                MyTrees.OpenDBFile(openfileDlg.FileName, separator, true);
                datasetListView.RefreshDB(MyTrees.treeDB, "");
            }
        }
        
        //直连sqlserver
        private void ButtonConnSqlserver_Click(object sender, RoutedEventArgs e)
        {
        	WindowConnDB connDB = new WindowConnDB("sqlserver");
        	connDB.ShowDialog();
        	datasetListView.RefreshDB(MyTrees.treeDB, "");
        }
        
        //直连Mysql
        private void ButtonConnectMysql_Click(object sender, RoutedEventArgs e)
        {
        	WindowConnDB connDB = new WindowConnDB("mysql");
        	connDB.ShowDialog();
        	datasetListView.RefreshDB(MyTrees.treeDB, "");
        }
        
        //删除数据集
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
        	string selectDataset = datasetListView.GetSelectDataset();
        	if(selectDataset != null)
        	{
        		MessageBoxResult msgResult = MessageBox.Show("您确定要删除数据集"+selectDataset+"吗？", 
        		                                             "删除确认", MessageBoxButton.OKCancel);
        		if(msgResult == MessageBoxResult.OK)
        		{
        			MyTrees.treeDB.DeleteDB(selectDataset);
        			datasetListView.DeleteBtn(selectDataset);
        		}
        	}
        }
        
         //计算案中案管理关系
        private void ButtonA_B_Click(object sender, RoutedEventArgs e)
        {
        	string selectDataset = datasetListView.GetSelectDataset();
        	if(selectDataset != null)
        	{
    			CalcSystemB calcSysB = new CalcSystemB(selectDataset);
    			calcSysB.ShowDialog();
        	}
        	else
        	{
        		MessageBox.Show("请先在右侧数据集中选中一个要作为系统A的数据集！");
        	}
        }
        
        //关于软件功能
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
		{
        	WindowAbout winAbout = new WindowAbout(true);
			winAbout.ShowDialog();
		}
        //历史版本记录
        private void BtnVerLog_Click(object sender, RoutedEventArgs e)
		{
			WindowVerLog verLog = new WindowVerLog(true);
			verLog.ShowDialog();
		}
	}
}