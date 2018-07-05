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
using System.Data.SQLite;
using System.IO;
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
		private SQLiteConnection conn;
		private SQLiteCommand cmd;
		private bool hasError = false;
		
		public CalcSystemB(string sysA)
		{
			InitializeComponent();
			txtSysA.Text = sysA;
			
			bgworker.WorkerReportsProgress = true;
            bgworker.WorkerSupportsCancellation = true;
            bgworker.DoWork += readSysBcsv;
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
                bgworker.RunWorkerAsync(txtSysB.Text);
                btnStart.IsEnabled = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        	if(bgworker.IsBusy)
        	{
            	bgworker.CancelAsync();
        	}
        	else
        	{
        		this.Close();
        	}
        }
       
        private void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        	if(step == 1)
        	{
        		prgStep1.Text = "已读取" + e.ProgressPercentage + "条数据:" + e.UserState;
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
        	
        	if(e.UserState != null)
        	{
        		txtMsg.Text = e.UserState.ToString();
        	}
        }

        private void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
            	MessageBox.Show("已取消计算！");
            }
            else if(!hasError)
            {
                if(step==1)
                {
                	step=2;
                	bgworker.DoWork -= readSysBcsv;
                	bgworker.DoWork += readSysADb;
                	bgworker.RunWorkerAsync(txtSysA.Text);
                }
                else if(step==2)
                {
                	step=3;
                	bgworker.DoWork -= readSysADb;
                	bgworker.DoWork += CalcA_B;
                	bgworker.RunWorkerAsync();
                }
                else if(step==3)
                {
                	step=4;
                	bgworker.DoWork -= CalcA_B;
                	bgworker.DoWork += saveToDB;
                	FileInfo finfo = new FileInfo(txtSysB.Text);
                	string sysBName = finfo.Name.Replace(finfo.Extension, "");
                	bgworker.RunWorkerAsync(new string[]{txtSysA.Text, sysBName});
                }
                else if(step==4)
                {
                	step=1;
                	bgworker.DoWork -= saveToDB;
                	bgworker.DoWork += readSysBcsv;
                	
                	MessageBox.Show("计算完成！");
                }
            }
        }
        
        private void openDB(string dbName)
        {
        	SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = MemData.MemDataSqlite + "\\" + dbName + ".db";
            connstr.Password = "passwd";
            connstr.Version = 3;
			conn = new SQLiteConnection(connstr.ConnectionString);
            cmd = new SQLiteCommand(conn);
            conn.Open();
        }
        
        private void exeSql(string sql, object param)
        {
        	cmd.CommandText = sql;
        	cmd.Parameters.Clear();
        	cmd.Parameters.Add(new SQLiteParameter("@1", param));
        	cmd.ExecuteNonQuery();
        }
        
        #region 计算相关
        
        //第一步，读取csv文件获取B系统的Id信息--------------------------------------------------------------
        private List<string> sysBids = new List<string>();
		private void readSysBcsv(object sender, DoWorkEventArgs e)
        {
			sysBids.Clear();
			string csvPath = e.Argument.ToString();
        	Encoding encoding = TextUtil.GetFileEncodeType(csvPath);
            StreamReader mysr = new StreamReader(csvPath, encoding);
            int count = 1;
            try
            {
            	string line;
                while(!mysr.EndOfStream)
                {
                	if (bgworker.CancellationPending)
		            {
		                e.Cancel = true;
                	}
                	else
                	{
	                	line = mysr.ReadLine();
	                	if(line != "")
			        	{
	                		sysBids.Add(line);
	                	}
	                	bgworker.ReportProgress(count++);
                	}
                }
                bgworker.ReportProgress(count, "第一步读取csv数据完成！");
            }
            catch (Exception ex)
            {
            	bgworker.ReportProgress(count, "第一步出错:" + ex.Message + ex.StackTrace);
            	hasError = true;
            }
            finally
            {
            	mysr.Close();
            }
        }
        
		//第二步，读取数据库获取系统A节点信息--------------------------------------------------------------
		private static Dictionary<string, SysABNode> sysAnodes = new Dictionary<string, SysABNode>(); 
		private void readSysADb(object sender, DoWorkEventArgs e)
		{
			sysAnodes.Clear();
			try
			{
				string sysADbName = e.Argument.ToString();
				openDB(sysADbName);
		        
		        //查询总数量
		        int allcount = 0;
		        cmd.CommandText = "select count(sysid) from tree_calc";
				SQLiteDataReader reader = cmd.ExecuteReader();
	            while (reader.Read())
	            {
	            	allcount = reader.GetInt32(0);
	            }
	            reader.Close();

	            //开始查询所有数据
	            cmd.CommandText = "select sysid, topid from tree_calc";
				SQLiteDataReader reader1 = cmd.ExecuteReader();
				int count = 0, step = 0;
	            while (reader1.Read())
	            {
	            	SysABNode node = new SysABNode()
		        	{
		        		SysId = reader1.GetString(0), 
		        		TopId = reader1.GetString(1)
		        	};
	            	sysAnodes.Add(node.SysId, node);
	            	if(100*count/allcount > step)
	            	{
	            		step = 100*count/allcount;
	            		bgworker.ReportProgress(step);
	            	}
	            	count++;
	            }
	            bgworker.ReportProgress(100, "第二步读取数据库数据完成！");
	            reader1.Close();
	            
            }
            catch(Exception ex)
            {
            	bgworker.ReportProgress(0, "第二步出错：" + ex.Message + ex.StackTrace);
            	hasError = true;
            }
            finally
            {
            	conn.Close();
            }
		}
		
		//第三步，计算A与B系统的关系--------------------------------------------------------------
		private void CalcA_B(object sender, DoWorkEventArgs e)
		{
			int step = 0;
			for (int i = 0; i < sysBids.Count; i++) 
			{
				if(sysAnodes.ContainsKey(sysBids[i]))
				{
					SysABNode anode = sysAnodes[sysBids[i]];
					anode.sysBId = 1;
					anode.sysBCount = 1;
					sysABCountInc(anode.TopId);
				}
				if(100*i/sysBids.Count > step)
            	{
            		step = 100*i/sysBids.Count;
            		bgworker.ReportProgress(step);
            	}
			}
			bgworker.ReportProgress(100, "第三步AB关系计算分析完成！");
		}
		//父节点与B系统有关的数量递归+1
		
		private void sysABCountInc(string id)
		{
			if(sysAnodes.ContainsKey(id))
			{
				SysABNode anode = sysAnodes[id];
				anode.sysBCount += 1;
				sysABCountInc(anode.TopId);
			}
		}
		
		//第四步，计算结果保存到数据库--------------------------------------------------------------
		private void saveToDB(object sender, DoWorkEventArgs e)
		{
			string[] args = e.Argument as string[];
			string sysAName = args[0];
			string sysBName = args[1];
			string colId = sysBName + "是否存在";
			string colCount = sysBName + "子节点数量";
			try
			{
				openDB(sysAName);
		        
		        //判断是否已存在列名
		        cmd.CommandText = "select count(*) from tree_profile where k='TableOptCol' and v=@1";
		        cmd.Parameters.Clear();
		        cmd.Parameters.Add(new SQLiteParameter("@1", colId));
		        SQLiteDataReader reader = cmd.ExecuteReader();
		        int num = 0;
	            while (reader.Read())
	            {
	            	num = reader.GetInt32(0);
	            }
	            reader.Close();
	            bgworker.ReportProgress(1);
	            
	            //如果不存在，则创建列
	            if(num == 0)
	            {
	            	string sqlStr2 = "insert into tree_profile values ('TableOptCol', @1)";
	            	exeSql(sqlStr2, colId);
	            	bgworker.ReportProgress(2);
	            	exeSql(sqlStr2, colCount);
	            	bgworker.ReportProgress(3);
	            	
                	cmd.CommandText = "alter table tree_calc add column " + colId + " varchar(1)";
	            	cmd.ExecuteNonQuery();
	            	bgworker.ReportProgress(4);
	            	cmd.CommandText = "alter table tree_calc add column " + colCount + " varchar(20)";
	            	cmd.ExecuteNonQuery();
	            	bgworker.ReportProgress(5);
	            }
	            
	            //设置初始默认值
               	cmd.CommandText = "update tree_calc set " + colId + "=0";
	            cmd.ExecuteNonQuery();
	            bgworker.ReportProgress(7);
	            cmd.CommandText = "update tree_calc set " + colCount + "=0";
	            cmd.ExecuteNonQuery();
	            bgworker.ReportProgress(10);
               	
               	//开始写入数据库
               	int step =0, count = 0;
               	foreach (SysABNode node in sysAnodes.Values)
               	{
               		if(node.sysBCount>0)
               		{
               			cmd.CommandText = "update tree_calc set " + colId + "=@1," + colCount + "=@2 where sysid=@3";
			        	cmd.Parameters.Clear();
			        	cmd.Parameters.Add(new SQLiteParameter("@1", node.sysBId.ToString()));
			        	cmd.Parameters.Add(new SQLiteParameter("@2", node.sysBCount.ToString()));
			        	cmd.Parameters.Add(new SQLiteParameter("@3", node.SysId));
			        	cmd.ExecuteNonQuery();
               		}
               		if(90*count/sysAnodes.Count > step)
		            {
		            	step = 90*count/sysAnodes.Count;
		            	bgworker.ReportProgress(step+10);
		            }
               		count++;
               	}
               	bgworker.ReportProgress(100, "第四步写入数据库完成！");
            }
            catch(Exception ex)
            {
            	bgworker.ReportProgress(0, "第四步出错：" + ex.Message + ex.StackTrace);
            	hasError = true;
            }
            finally
            {
            	conn.Close();
            }
		}
		
        #endregion
	}
	
	public class SysABNode
    {    	
    	public string SysId;
    	public string TopId;
    	public int sysBId;  //是否在系统B中
    	public int sysBCount;  //子节点（含自身）在B系统中的数量
	}
}