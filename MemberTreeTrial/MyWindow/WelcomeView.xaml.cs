using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MemberTree
{
    /// <summary>
    /// HelpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeView : UserControl
    {
    	private InvokeStringDelegate startupDelegate;
    	private Dictionary<string, string> verLogs = new Dictionary<string, string>();
  		private Button selectDatasetBtn = null;
    	public WelcomeView()
        {
            InitializeComponent();
    	}
        
        public string GetSelectDataset()
		{
			if(selectDatasetBtn == null)
			{
				return null;
			}
			else
			{
				return selectDatasetBtn.Content.ToString();
			}
		}
		
		public void SetCallBack(InvokeStringDelegate startupDelegate)
		{
			this.startupDelegate = startupDelegate;
		}
		
		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			if(selectDatasetBtn != null)
			{
				if(selectDatasetBtn == btn)
				{
					return;
				}
			}
			selectDatasetBtn = btn;
			
			if(startupDelegate != null)
			{
				startupDelegate.Invoke(btn.Content.ToString());
			}
		}
    }
}
