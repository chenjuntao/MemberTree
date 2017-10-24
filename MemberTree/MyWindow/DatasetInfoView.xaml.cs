/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 12/24/2016
 * 时间: 18:41
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MemberTree
{
	/// <summary>
	/// 单个数据集概要信息
	/// </summary>
	public partial class DatasetInfoView : UserControl
	{
		private InvokeStringDelegate btnClickDel;
		private Button btnPressed;
		public DatasetInfoView()
		{
			InitializeComponent();
		}
		
		public void Init(IMyTreeDB treeDB)
		{
			txtDB.Text = treeDB.DatasetName;
			
			Dictionary<string, int> counts = treeDB.GetCounts();
			
			btnAll.Content = "所有节点总数" + counts["AllNodeCount"];
			btnTree.Content =  counts["TreeNodeCount"] + "个节点构成" + counts["TreeCount"] + "课树";
			btnLeaf.Content = counts["LeafCount"] + "个孤立的叶子节点" ;
			btnLeaf.Visibility = counts["LeafCount"] > 0 ? Visibility.Visible : Visibility.Collapsed;
			btnRing.Content = counts["RingCount"] + "个构成闭环的节点";
			btnRing.Visibility = counts["RingCount"] > 0 ? Visibility.Visible : Visibility.Collapsed;
			btnConflict.Content = counts["ConflictCount"] + "个ID重复的节点";
			btnConflict.Visibility = counts["ConflictCount"] > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		
		public void SetCallBack(InvokeStringDelegate btnClickDelegate)
		{
			this.btnClickDel = btnClickDelegate;
			btnTree.FontWeight = FontWeights.Bold;
			btnTree.Background = Brushes.AntiqueWhite;
			btnTree.BorderThickness = new Thickness(1);
			btnTree.FontSize = 13;
			btnPressed = btnTree;
		}
		
		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			if(btnClickDel != null)
			{
				Button btn = sender as Button;
				btnClickDel.Invoke(btn.Content.ToString());
			}
		}
		
		private void BtnTree_Click(object sender, RoutedEventArgs e)
		{
			if(btnClickDel != null)
			{
				Button btnClick = sender as Button;
				if(btnClick.FontWeight == FontWeights.Normal)
				{
					btnClick.FontWeight = FontWeights.Bold;
					btnClick.Background = Brushes.AntiqueWhite;
					btnClick.BorderThickness = new Thickness(1);
					btnClick.FontSize = 13;
					btnPressed.FontWeight = FontWeights.Normal;
					btnPressed.Background = Brushes.White;
					btnPressed.BorderThickness = new Thickness(0);
					btnPressed.FontSize = 12;
					btnPressed = btnClick;
					
					if(btnClick == btnTree)
					{
						btnClickDel.Invoke("tree");
					}
					else if(btnClick == btnLeaf)
					{
						btnClickDel.Invoke("leaf");
					}
					else if(btnClick == btnRing)
					{
						btnClickDel.Invoke("ring");
					}
					else if(btnClick == btnConflict)
					{
						btnClickDel.Invoke("conflict");
					}
				}
			}
		}
		
		public void SelectTab(string tab)
		{
			Button btnClick = btnTree;
			if(tab == "leaf")
			{
				btnClick = btnLeaf;
			}
			else if(tab == "ring")
			{
				btnClick = btnRing;
			}
			else if(tab == "conflict")
			{
				btnClick = btnConflict;
			}
			if(btnClick.FontWeight == FontWeights.Normal)
			{
				btnClick.FontWeight = FontWeights.Bold;
				btnClick.Background = Brushes.AntiqueWhite;
				btnClick.BorderThickness = new Thickness(1);
				btnClick.FontSize = 13;
				if(btnPressed != null)
				{
					btnPressed.FontWeight = FontWeights.Normal;
					btnPressed.Background = Brushes.White;
					btnPressed.BorderThickness = new Thickness(0);
					btnPressed.FontSize = 12;
				}
				btnPressed = btnClick;
			}
		}
	}
}