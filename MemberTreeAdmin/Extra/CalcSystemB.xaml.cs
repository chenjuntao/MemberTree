/*
 * 由SharpDevelop创建。
 * 用户： tomchen
 * 日期: 2018/7/2
 * 时间: 16:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for CalcSystemB.xaml
	/// </summary>
	public partial class CalcSystemB : Window
	{
		private int step = 1;
		private BackgroundWorker bgworker = new BackgroundWorker();
		public CalcSystemB(string sysA)
		{
			InitializeComponent();
			txtSysA.Text = sysA;
			
			bgworker.WorkerReportsProgress = true;
            bgworker.WorkerSupportsCancellation = true;
            bgworker.DoWork += bgworker_DoWork;
            bgworker.ProgressChanged += bgworker_ProgressChanged;
            bgworker.RunWorkerCompleted += bgworker_RunWorkerCompleted;
		}
		
		private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
			OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "打开系统B的数据文件";
            openfileDlg.Filter = "CSV逗号分隔文件|*.csv";
            if (openfileDlg.ShowDialog() == true)
            {
            	txtSysB.Text = openfileDlg.FileName;
            	btnStart.IsEnabled = true;
            }
        }
		
		private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!bgworker.IsBusy)
            {
                bgworker.RunWorkerAsync();
                btnStart.IsEnabled = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        	if(bgworker.IsBusy)
        	{
            	bgworker.CancelAsync();
            	btnStart.IsEnabled = true;
        	}
        	else
        	{
        		this.Close();
        	}
        }
		
		private void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 1; i <= 100; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    worker.ReportProgress(i);
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
       
        private void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        	if(step == 1)
        	{
        		prgStep1.Value = e.ProgressPercentage;
        	}
        	else if(step == 2)
        	{
        		prgStep2.Value = e.ProgressPercentage;
        	}
        	else if(step == 3)
        	{
        		prgStep3.Value = e.ProgressPercentage;
        	}
        	else if(step == 4)
        	{
        		prgStep4.Value = e.ProgressPercentage;
        	}
        }

        private void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("该次计算任务已经取消！", "取消");
            }
            else
            {
                if(step<4)
                {
                	step++;
                	bgworker.RunWorkerAsync();
                }
                else{
                	MessageBox.Show("该次计算任务已经完成！", "完成");
                	step = 1;
                }
            }
        }

      
	}
}