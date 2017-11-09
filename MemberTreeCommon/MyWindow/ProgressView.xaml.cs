/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 12/24/2016
 * 时间: 18:57
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
using System.Windows.Media.Animation;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for ProgressView.xaml
	/// </summary>
	public partial class ProgressView : UserControl
	{
		public ProgressView()
		{
			InitializeComponent();
		}
		
		public void SetCsvFile(string csvFile)
		{
			this.txtCsvFile.Text = csvFile;
		}
		
		public void ReSetProgressValue()
		{
			treeRotate.Angle = 90;
			treeScale0.ScaleX = 0;
			treeScale0.ScaleY = 0;
			treeScale1.ScaleX = 0;
			treeScale1.ScaleY = 0;
			treeScale2.ScaleX = 0;
			treeScale2.ScaleY = 0;
			treeScale3.ScaleX = 0;
			treeScale3.ScaleY = 0;
			treeScale4.ScaleX = 0;
			treeScale4.ScaleY = 0;
			treeScale5.ScaleX = 0;
			treeScale5.ScaleY = 0;
			treeScale6.ScaleX = 0;
			treeScale6.ScaleY = 0;
			treeScale7.ScaleX = 0;
			treeScale7.ScaleY = 0;
		}
		
		//更新等待状态
		int angle = 90;
		public void SetWaitting()
		{
			treeRotate.Angle = angle++;
			if(angle == 360)
			{
				angle=0;
			}
		}
		
		//更新进度，0-100
		public void SetProgressValue(double progress)
		{
			progress = progress * 8 /10;
			
			if(progress>0 && progress<=10)
			{
				treeScale0.ScaleX = progress / 10.0;
				treeScale0.ScaleY = progress / 10.0;
			}
			else if(progress>10 && progress<=20)
			{
				treeScale1.ScaleX = (progress - 10) / 10.0;
				treeScale1.ScaleY = (progress - 10) / 10.0;
			}
			else if(progress>20 && progress<=30)
			{
				treeScale2.ScaleX = (progress - 20) / 10.0;
				treeScale2.ScaleY = (progress - 20) / 10.0;
			}
			else if(progress>30 && progress<=40)
			{
				treeScale3.ScaleX = (progress - 30) / 10.0;
				treeScale3.ScaleY = (progress - 30) / 10.0;
			}
			else if(progress>40 && progress<=50)
			{
				treeScale4.ScaleX = (progress - 40) / 10.0;
				treeScale4.ScaleY = (progress - 40) / 10.0;
			}
			else if(progress>50 && progress<=60)
			{
				treeScale5.ScaleX = (progress - 50) / 10.0;
				treeScale5.ScaleY = (progress - 50) / 10.0;
			}
			else if(progress>60 && progress<=70)
			{
				treeScale6.ScaleX = (progress - 60) / 10.0;
				treeScale6.ScaleY = (progress - 60) / 10.0;
			}
			else if(progress>70 && progress<=80)
			{
				treeScale7.ScaleX = (progress - 70) / 10.0;
				treeScale7.ScaleY = (progress - 70) / 10.0;
			}
		}
	}
}