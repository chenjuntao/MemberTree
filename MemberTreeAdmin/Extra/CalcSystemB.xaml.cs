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

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for CalcSystemB.xaml
	/// </summary>
	public partial class CalcSystemB : Window
	{
		BackgroundWorker bgworker = new BackgroundWorker();
		public CalcSystemB()
		{
			InitializeComponent();
			
			bgworker.WorkerReportsProgress = true;
            bgworker.WorkerSupportsCancellation = true;
            bgworker.DoWork += bgworker_DoWork;
            bgworker.ProgressChanged += bgworker_ProgressChanged;
            bgworker.RunWorkerCompleted += bgworker_RunWorkerCompleted;
		}
		
		void bgworker_DoWork(object sender, DoWorkEventArgs e)
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
       
        void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgStep1.Value = e.ProgressPercentage;
        }

        void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Background task has been canceled", "info");
            }
            else
            {
                MessageBox.Show("Background task finished", "info");
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!bgworker.IsBusy)
            {
                bgworker.RunWorkerAsync();
                btnStart.IsEnabled = false;
                btnCancel.IsEnabled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        	if(bgworker.IsBusy)
        	{
            	bgworker.CancelAsync();
            	btnStart.IsEnabled = true;
                btnCancel.IsEnabled = false;
        	}
        }
	}
}