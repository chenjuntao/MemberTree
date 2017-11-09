/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 10/23/2017
 * 时间: 16:09
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
using System.Windows.Media.Imaging;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for BtnDataset.xaml
	/// </summary>
	public partial class BtnDataset : UserControl
	{
		public BtnDataset(DatasetInfo db)
		{
			InitializeComponent();
			this.txtName.Text = db.Name;
			this.ToolTip = db.GetOtherString();
		}
		
		public string DatasetName
		{
			get{return txtName.Text;}
		}
		
		public bool isSelected
		{
			get{return this.BorderThickness.Bottom == 1.0;}
			set
			{
				if(value)
				{
					this.BorderThickness = new Thickness(1);
					this.img.Source = new BitmapImage(new Uri("/MemberTree;component/Image/data1.png", UriKind.Relative));
				}
				else
				{
					this.BorderThickness = new Thickness(0);
					this.img.Source = new BitmapImage(new Uri("/MemberTree;component/Image/data.png", UriKind.Relative));
				}
			}
		}
	}
}