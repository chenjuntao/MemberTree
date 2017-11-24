/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 09/20/2017
 * 时间: 14:15
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for WindowVerLog.xaml
	/// </summary>
	public partial class WindowAbout : Window
	{
		public WindowAbout()
		{
			InitializeComponent();
		}
		
		void BtnOpenImg_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			string imgUri = "/MemberTreeTrial;component/Image/" + btn.Name + ".png";
			imgCover.Source = new BitmapImage(new Uri(imgUri, UriKind.Relative));
			imgCover.OpacityMask = this.Resources["OpenBrush"] as LinearGradientBrush;
		    Storyboard std = this.Resources["OpenStoryboard"] as Storyboard;
		    std.Begin();
		}
		
		void ImgCover_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			imgCover.OpacityMask = this.Resources["CloseBrush"] as LinearGradientBrush;
		    Storyboard std = this.Resources["CloseStoryboard"] as Storyboard;
		    std.Completed += delegate { imgCover.Source = null; };
		    std.Begin();
		}
	}
}