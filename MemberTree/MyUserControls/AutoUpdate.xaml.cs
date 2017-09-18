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
		public AutoUpdate()
		{
			InitializeComponent();
		}
		
		public void Update(bool isAdmin)
		{
			SetStatusMessage("程序正在进行更新...");
			string dir = @"D:\Work\tree\bin\DLLs";
			DirectoryInfo dirInfo = new DirectoryInfo(dir);
			if(dirInfo.Exists)
			{
				if(Directory.Exists("dll")){
					string[] olddlls = Directory.GetFiles("dll");
					foreach (string olddll in olddlls) 
					{
						File.Delete(olddll);
					}
				}
				else
				{
					Directory.CreateDirectory("dll");
				}
			
				FileInfo[] newdlls = dirInfo.GetFiles();
				foreach (FileInfo newdll in newdlls) {
					File.Copy(newdll.FullName, "dll/" + newdll.Name);
				}
			}
		}
		
		#region 提示文本实时更新
		//设置状态栏提示文本

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