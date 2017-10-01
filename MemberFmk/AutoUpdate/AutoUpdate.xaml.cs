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
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for AutoUpdate.xaml
	/// </summary>
	public partial class AutoUpdate : UserControl
	{
		public AutoUpdate()
		{
			InitializeComponent();
		}
		
		public void Update()
		{
			AutoUpdateVer oldVer = AutoUpdateVer.ReadVer();
			lblVer.Content = "当前版本：" + oldVer.Version;
			lblVerInfo.Content = oldVer.VerInfo;
			SetStatusMessage("程序正在检查是否有新版本...");
			if(HttpDownload(oldVer.Url + "ver.dll", "dll/ver.dll"))
			{
				AutoUpdateVer newVer = AutoUpdateVer.ReadVer();
				lblVer.Content = "当前版本：" + oldVer.Version + "——>新版本：" + newVer.Version;
				lblVerInfo.Content = newVer.VerInfo;
				SetStatusMessage("发现新版本，正在更新...");
				File.Delete("dll/ver.dll");
				int allCount = newVer.DLLs.Count;
				int count = 1;
				foreach (string dll in newVer.DLLs.Keys) 
				{
					SetStatusMessage("正在更新第"+count+"个("+dll+")/总共"+allCount+"个...");
					if(!oldVer.DLLs.ContainsKey(dll)&&
						newVer.DLLs[dll] != oldVer.DLLs[dll]) //更新文件
					{
						HttpDownload(newVer.Url + dll, "dll/" + dll);
					}
					count++;
				}
				HttpDownload(oldVer.Url + "ver.dll", "dll/ver.dll");
			}
			else
			{
				SetStatusMessage("连接更新主机出错，更新中断！");
			}
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