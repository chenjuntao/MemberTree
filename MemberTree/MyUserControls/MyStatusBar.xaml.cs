/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 09/16/2017
 * 时间: 17:27
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
using System.Windows.Threading;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for MyStatusBar.xaml
	/// </summary>
	public partial class MyStatusBar : UserControl, INotify
	{
		public MyStatusBar()
		{
			InitializeComponent();
		}
		
		private ProgressView progressView;
		private UIElement[] disableViews;
		private UIElement[] hideViews;
		public void SetShowHideView(ProgressView showView, UIElement[] disableViews, UIElement[] hideViews)
		{
			this.progressView = showView;
			if(disableViews == null){
				this.disableViews = new UIElement[]{};
			}else{
				this.disableViews = disableViews;
			}
			if(hideViews == null){
				this.hideViews = new UIElement[]{};
			}else{
				this.hideViews = hideViews;
			}
		}
		
		 #region INotify接口
        
        //设置进度条是否显示
        private InvokeBoolDelegate processBarVisibleDelegate = null;
        public void SetProcessBarVisible(bool visible)
        {
//            if (processBarVisibleDelegate == null)
//            {
//                processBarVisibleDelegate = new InvokeBoolDelegate(SetProcessBarVisibleImp);
//            }
//            this.Dispatcher.Invoke(processBarVisibleDelegate, visible);
SetProcessBarVisibleImp(visible);
            DoEvents();
        }
        private void SetProcessBarVisibleImp(bool visible)
        {
        	int a = Environment.TickCount;
            progressBar.Value = 0;
            progressBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if(progressView !=null)
            {
            	progressView.ReSetProgressValue();
            	progressView.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
            foreach (UIElement element in disableViews) 
            {
            	element.IsEnabled = !visible;
            }
            foreach (UIElement element in hideViews) 
            {
            	element.Visibility = visible ? Visibility.Collapsed : Visibility.Visible;
            }
            Cursor = visible ? Cursors.Wait : Cursors.Arrow;
        }

        //设置进度条进度(0-100)
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
            if(progressView != null)
            {
            	progressView.SetProgressValue(step);
            }
        }

        //设置状态栏提示文本
        private InvokeStringDelegate showTextDelegate = null;
        public void SetStatusMessage(string message)
        {
            if (showTextDelegate == null)
            {
                showTextDelegate = new InvokeStringDelegate(SetStatusMessageImp);
            }
            Dispatcher.Invoke(showTextDelegate, message);
            DoEvents();
        }
        private void SetStatusMessageImp(string message)
        {
            statusMessage.Text = message;
            if(progressView != null)
            {
            	progressView.SetWaitting();
            }
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