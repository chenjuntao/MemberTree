/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/8/29
 * 时间: 12:03
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for AutoUpdate.xaml
	/// </summary>
	public partial class AutoUpdate : UserControl
	{
		private string url = "https://github.com/chenjuntao/MemberTree/raw/master/DLL/";
//		private string url = "https://coding.net/u/cjtlp2006/p/TemberTreeDLL/git/raw/master/";
		private string conf = "version.dll";
		public AutoUpdate()
		{
			InitializeComponent();
		}
		
		public void Update()
		{
			SetStatusMessage("程序正在检查更新...");
			List<string> oldConf = ReadConfig();
			if(HttpDownload(url + conf, "dll/" + conf))
			{
				List<string> newConf = ReadConfig();
				if(newConf[0] == oldConf[0])
				{
					SetStatusMessage("目前已经是最新版本，无需更新！");
				}
				else
				{
					for (int i = 1; i < newConf.Count; i++) 
					{
						SetStatusMessage("正在更新第"+i+"个("+newConf[i]+")/总共"+newConf.Count+"个...");
						HttpDownload(url + newConf[i], "dll/" + newConf[i]);
					}
				}
				HttpDownload(url + conf, "dll/" + conf);
			}
			else
			{
				SetStatusMessage("连接更新主机出错，更新中断！");
			}
		}
		
		private List<string> ReadConfig()
		{
			if(!Directory.Exists("dll"))
			{
				Directory.CreateDirectory("dll");
			}
			string confFile = "dll/" + conf;
			List<string> result = new List<string>();
			if(File.Exists(confFile))
			{
				using(StreamReader mysr = new StreamReader(confFile, Encoding.UTF8))
				{
					while(!mysr.EndOfStream)
	                {
						result.Add(mysr.ReadLine());
					}
				}
			}
			else
			{
				result.Add("v0.0");
			}
			File.Delete(confFile);
			
			return result;
		}
		
		/// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        private bool HttpDownload(string url, string path)
        {
        	try {
	            // 设置参数
	            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
	 
	            //发送请求并获取相应回应数据
	            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
	            //直到request.GetResponse()程序才开始向目标网页发送Post请求
	            Stream responseStream = response.GetResponseStream();
	            //创建本地文件写入流
	            Stream stream = new FileStream(path, FileMode.Create);
	            byte[] bArr = new byte[10240];
	            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
	            long totalBytes = response.ContentLength;
	            long totalDownloadedByte = 0;
	            while (size > 0)
	            {
	            	
	                stream.Write(bArr, 0, size);
	                size = responseStream.Read(bArr, 0, (int)bArr.Length);
	                totalDownloadedByte = size + totalDownloadedByte;
	                double percent = (double)totalDownloadedByte / (double)totalBytes * 100;
	                SetProcessBarValue(percent);
	            }
	            stream.Close();
	            responseStream.Close();
	            return true;
            	
        	} catch (Exception ex) {
        		
        	}
        	finally
        	{
        		
        	}
        	return false;
        }
		
		#region 提示文本实时更新
		//设置状态栏提示文本
		private delegate void InvokeStringDelegate(string parm);
        private InvokeStringDelegate showTextDelegate = null;
        public void SetStatusMessage(string message)
        {
            if (showTextDelegate == null)
            {
                showTextDelegate = new InvokeStringDelegate(SetStatusMessageImp);
            }
            this.Dispatcher.BeginInvoke(showTextDelegate, message);
            DoEvents();
        }
        private void SetStatusMessageImp(string message)
        {
            this.lblMsg.Content = message;
        }
  		//设置进度条进度(0-100)
  		private delegate void InvokeDoubleDelegate(double parm);
        private InvokeDoubleDelegate processBarValueDelegate = null;
        public void SetProcessBarValue(double step)
        {
            if (processBarValueDelegate == null)
            {
                processBarValueDelegate = new InvokeDoubleDelegate(SetProcessBarValueImp);
            }
            this.Dispatcher.Invoke(processBarValueDelegate, step);
            DoEvents();
        }
        private void SetProcessBarValueImp(double step)
        {
            this.progressBar.Value = step;
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