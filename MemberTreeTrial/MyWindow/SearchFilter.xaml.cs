/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 08/06/2017
 * 时间: 14:34
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
	/// Interaction logic for SearchFilter.xaml
	/// </summary>
	public partial class SearchFilter : UserControl
	{
		private List<ComboBox> comboCols = new List<ComboBox>();
		private List<TextBox> txtCols = new List<TextBox>();
		public SearchFilter()
		{
			InitializeComponent();
		}
		
		internal void InitCols()
		{		
			List<string> optCols = MyTrees.TableOptCols;
			for (int i = 0; i < optCols.Count; i++) 
			{
				mainGrid.RowDefinitions.Add(new RowDefinition());
			
				TextBlock header = new TextBlock();
				header.Text = optCols[i];
				mainGrid.Children.Add(header);
				Grid.SetRow(header, i+8);
				
				ComboBox comb = new ComboBox();
				ComboBoxItem comItem1 = new ComboBoxItem();
				comItem1.Content = "等于";
				comItem1.IsSelected = true;
				comb.Items.Add(comItem1);
				ComboBoxItem comItem2 = new ComboBoxItem();
				comItem2.Content = "开头";
				comb.Items.Add(comItem2);
				ComboBoxItem comItem3 = new ComboBoxItem();
				comItem3.Content = "结尾";
				comb.Items.Add(comItem3);
				ComboBoxItem comItem4 = new ComboBoxItem();
				comItem4.Content = "包含";
				comb.Items.Add(comItem4);
				mainGrid.Children.Add(comb);
				Grid.SetRow(comb, i+8);
				Grid.SetColumn(comb, 1);
				comboCols.Add(comb);
				
				TextBox val = new TextBox();
				val.Text = "试用版不支持";
				val.IsEnabled = false;
				mainGrid.Children.Add(val);
				Grid.SetRow(val, i+8);
				Grid.SetColumn(val, 2);
				txtCols.Add(val);
			}
		}
		
		public List<string> GetSearchParams()
		{
			return new List<string>(){txtSysid.Text, comboSysid.SelectionBoxItem.ToString()};
		}
		
		//清除重置查找条件
        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
        	txtSysid.Clear();
        }
	}
}