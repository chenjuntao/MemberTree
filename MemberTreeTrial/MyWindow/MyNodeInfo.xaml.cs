/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 08/06/2017
 * 时间: 11:20
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for MyNodeInfo.xaml
	/// </summary>
	public partial class MyNodeInfo : UserControl
	{
		private List<TextBlock> nodeInfoCols = new List<TextBlock>();
		public MyNodeInfo()
		{
			InitializeComponent();
		}
		
		internal void InitCols()
		{
			addCol("会员ID：");
			addCol("父级ID：");
			addCol("会员姓名：");
			addCol("所在等级：");
			addCol("下线层级数：");
			addCol("直接下级数：");
			addCol("下级总数：");
			
			List<string> optCols = MyTrees.TableOptCols;
			for (int i = 0; i < optCols.Count; i++) 
			{
				addCol(optCols[i] + "：");
			}
		}
		
		private void addCol(string colName)
		{
			TextBlock header = new TextBlock();
			header.Text = colName;
			leftPanel.Children.Add(header);
			
			TextBlock val = new TextBlock();
			rightPanel.Children.Add(val);
			
			nodeInfoCols.Add(val);
		}
		
		internal void SetNode(MyTreeNode node)
		{
			grpNodeInfo.Header = "单个节点详细信息 - " +  node.Name + "(" + node.SysId + ")";

			nodeInfoCols[0].Text = node.SysId;
			nodeInfoCols[1].Text = node.TopId;
			nodeInfoCols[2].Text = node.Name;
			nodeInfoCols[3].Text = node.Level.ToString();
			nodeInfoCols[4].Text = node.ChildrenLevels.ToString();
			nodeInfoCols[5].Text = node.ChildrenNodes.Count.ToString();
			nodeInfoCols[6].Text = node.ChildrenCount.ToString();
			for (int i = 0; i < node.OtherProps.Count; i++) 
			{
				nodeInfoCols[i+7].Text = node.OtherProps[i];
			}
		}
		
		 //导出图片
        private void btnExportImg_Click(object sender, RoutedEventArgs e)
        {
        	SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Title = "选择将节点信息导出为文件的位置";
            saveFileDlg.Filter = "png格式|*.png";
            saveFileDlg.FileName = grpNodeInfo.Header.ToString().Split(new String[]{" - "}, StringSplitOptions.None)[1];
            if (saveFileDlg.ShowDialog() == true)
            {
                ExportIMG.SaveImage(mainContent, saveFileDlg.FileName);
            }
        }
        
        //打印
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.UserPageRangeEnabled = true;

            if (printDlg.ShowDialog() == true)
            {
                 printDlg.PrintVisual(mainContent, "打印当前节点信息");
            }
        }
	}
}