/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/8/8
 * 时间: 13:38
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for ErrLinesConfirm.xaml
	/// </summary>
	public partial class CsvErrCheck : Window
	{
		public CsvErrCheck()
		{
			InitializeComponent();
		}
		
		int allCount, errCount;
		string filepath;
		
		//检测
		private void btnCheck_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "打开要作为会员树数据源的文件";
            openfileDlg.Filter = "CSV文件|*.csv";
            if (openfileDlg.ShowDialog() == true)
            {
            	
            	allCount = 0;
            	errCount = 0;
            	txtErrLines.Text = "";
				filepath = openfileDlg.FileName;
	            Encoding encoding = TextUtilTool.GetFileEncodeType(filepath);
	            StreamReader mysr = new StreamReader(filepath, encoding);
	            filepath = filepath.Replace(".csv","_1.csv");
	            StreamWriter mysw = new StreamWriter(filepath, false, Encoding.UTF8);
	            mysw.WriteLine(TextUtilTool.CSVInHeader);
	            
	            try
	            {
	                SetStatusMessage("开始读取文件......");
	
	                string firstLine = mysr.ReadLine(); //第一行是表头，读取之后不处理，直接跳过
	                if (firstLine.ToLower() != TextUtilTool.CSVInHeader)
	                {
	                    MessageBox.Show("文件格式不正确，第一行必须由标题头("+TextUtilTool.CSVInHeader+")组成！");
	                    return;
	                }
	                
	                while(!mysr.EndOfStream)
	                {
	                	string line = mysr.ReadLine();
	                	Line2TreeNode(line, mysw);
	                }
	                
	                if(txtErrLines.Text == "")
	                {
	                	btnSave.IsEnabled = false;
	                	SetStatusMessage("检查完成，正确数据行数：" + allCount + "，出错数据行数："+ errCount);
	                	MessageBox.Show("恭喜，该文件没有错误，可以直接导入计算！");
	                }
	              	else
	              	{
	              		btnSave.IsEnabled = true;
	              		SetStatusMessage("检查完成，正确数据行数：" + allCount + "，出错数据行数："+ errCount +"，错误数据都存储在下面的文本框中。数据格式为："+TextUtilTool.CSVInHeader);
	              		MessageBox.Show("该文件出现了多处错误，错误数据都存储在下面的文本框中，请将这些错误人工校准之后重新填回文本框中，点击“修改保存到文件”按钮");
	              	}

	            }
	            catch (Exception ex)
	            {
	            	SetStatusMessage("文件读取出错！+\n" + ex.Message);
	                return;
	            }
	            finally
	            {
	                mysr.Close();
	                mysw.Close();
	            }
            }
		}
		
		private void Line2TreeNode(string line, StreamWriter mysw)
        {
	        string[] aryline = line.Split(new char[] { ',' });
        	if(aryline.Length<10){
	        	if(line != "")
	        	{
					AddErrLine(line);
	        	}
				return;
        	}
	        if(MyTreeNodeDB.Check(aryline))
	        {
	        	mysw.WriteLine(line);
	        }
	        else
	        {
	        	AddErrLine(line);
	        	return;
	        }
	        
//            if(myNode.SysId == "") //信息不完整（ID为空）的节点
//            {
//            	IdNullNodes.Add(myNode);
//            }
//           	else if(allNodes.ContainsKey(myNode.SysId)) //ID有重复的节点
//            {
//                IdConflictNodes.Add(myNode);
//            }
//            else
//            {
//                allNodes.Add(myNode.SysId, myNode);
//            }

            allCount++;
            if (allCount % 1000 == 0)
            {
                SetStatusMessage("正在检查数据，正确数据行数：" + allCount + "，出错数据行数："+ errCount);
            }
        }
		
		//保存
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			StreamWriter mysw = new StreamWriter(filepath, true, Encoding.UTF8);
			string[] lines = txtErrLines.Text.Split(new String[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
			txtErrLines.Text = "";
			errCount = 0;
			foreach (string line in lines) 
			{
				Line2TreeNode(line, mysw);
			}
			mysw.Close();
			
			if(txtErrLines.Text == "")
            {
				btnSave.IsEnabled = false;
            	SetStatusMessage("保存完成，最终合适数据行数：" + allCount);
            	MessageBox.Show("恭喜，保存完成！");
            }
          	else
          	{
          		SetStatusMessage("合并保存完成，正确数据行数：" + allCount + "，出错数据行数："+ errCount +"，错误数据都存储在下面的文本框中。数据格式为："+TextUtilTool.CSVInHeader);
          		MessageBox.Show("您处理过的数据仍然有多处错误，错误数据都存储在下面的文本框中，请将这些错误人工校准之后重新填回文本框中，点击“修改保存到文件”按钮");
          	}
		}
		
		#region
		
		//设置状态栏提示文本
        private delegate void ShowTextDelegate(string message);
        private ShowTextDelegate setStatusDelegate = null;
        private ShowTextDelegate addErrDelegate = null;
        public void SetStatusMessage(string message)
        {
            if (setStatusDelegate == null)
            {
                setStatusDelegate = new ShowTextDelegate(SetStatusMessageImp);
            }
            this.Dispatcher.Invoke(setStatusDelegate, message);
            DoEvents();
        }
        private void SetStatusMessageImp(string message)
        {
            this.txtHeader.Text = message;
        }
        
        public void AddErrLine(string line)
        {
            if (addErrDelegate == null)
            {
                addErrDelegate = new ShowTextDelegate(AddErrLineImp);
            }
            this.Dispatcher.Invoke(addErrDelegate, line);
            DoEvents();
        }
        private void AddErrLineImp(string line)
        {
        	this.txtErrLines.AppendText(line);
        	this.txtErrLines.AppendText(Environment.NewLine);
        	errCount++;
        }

        private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate(object f)
                {
                    (f as DispatcherFrame).Continue = false;

                    return null;
                }
            ), frame);
            Dispatcher.PushFrame(frame);
        }
		
		#endregion
	}
}