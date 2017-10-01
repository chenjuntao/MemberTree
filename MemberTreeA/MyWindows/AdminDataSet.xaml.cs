﻿/*
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
			forestView.SetCallBack(new InvokeStringDelegate(SelectDB));
		}
		
		private void SelectDB(string selectDB)
		{
			MyTrees.SetDBName(selectDB);
			commonView.Init(MyTrees.treeDB);
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
            OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "打开要作为会员树数据源的文件";
            openfileDlg.Filter = "CSV文件|*.csv";
            if (openfileDlg.ShowDialog() == true)
            {
            	windowAdmin.progressView.SetCsvFile(openfileDlg.FileName);
            	
                int upperLower = comboToLower.SelectedIndex;
                int DBCSBC = comboToHalf.SelectedIndex;
                int trim = comboTrim.SelectedIndex;
                MyTrees.OpenCSVFile(openfileDlg.FileName, upperLower, DBCSBC, trim);
                
                forestView.RefreshDB(MyTrees.treeDB);
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
        
        //删除数据集
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
        	string selectDataset = forestView.GetSelectDataset();
        	if(selectDataset != null)
        	{
        		MessageBoxResult msgResult = MessageBox.Show("您确定要删除数据集"+selectDataset+"吗？", 
        		                                             "删除确认", MessageBoxButton.OKCancel);
        		if(msgResult == MessageBoxResult.OK)
        		{
        			MyTrees.treeDB.DeleteDB(selectDataset);
        			forestView.DeleteBtn(selectDataset);
        		}
        	}
        }
        
        //关于
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
		{
			WindowVerLog verLog = new WindowVerLog(true, SysInfo.I.PRODUCT + "管理工具");
			verLog.ShowDialog();
		}
	}
}