/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 10/20/2017
 * 时间: 09:55
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

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for DatasetBtn.xaml
	/// </summary>
	public partial class DatasetBtn : UserControl
	{
		public DatasetBtn(DatasetInfo dsInfo)
		{
			InitializeComponent();
			SetDatasetInfo(dsInfo);
		}
		
		private void SetDatasetInfo(DatasetInfo dsInfo)
		{
			dsName.Text = dsInfo.Name;
			dsCol.Text = dsInfo.ColCount.ToString();
			dsCrateDate.Text = dsInfo.CreateData.ToString();
			
			int row = dsInfo.RowCount;
			if(row <10000)
			{
				dsRow.Text = row.ToString();
			}
			else
			{
				int big = row / 10000;
				int small = row % 10000;
				dsRow.Text = string.Format("{0}万{1}", big, small);
			}
			
			dsSize.Value = ((double)row * dsInfo.ColCount * 100) / 100000000;
		}
		
		public void Select()
		{
			this.Background = Brushes.LightSkyBlue;
		}
		
		public void UnSelect()
		{
			this.Background = Brushes.Azure;
		}
		
		public string DatasetName
		{
			get
			{
				return dsName.Text;
			}
		}
	}
}