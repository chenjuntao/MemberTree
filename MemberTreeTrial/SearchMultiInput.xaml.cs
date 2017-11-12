/*
 * CopyRight(C) Tanry Tech Co.,Ltd
 * 作者： 陈俊涛
 * 日期: 06/04/2015
 * 时间: 11:38
 * 中国，湖南，长沙
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
using System.Windows.Threading;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for SearchMultiInput.xaml
	/// </summary>
	public partial class SearchMultiInput : Window
	{
		public SearchMultiInput()
		{
			InitializeComponent();
			btnOK.Content = "开始查找";
		}
		
		private void Browse_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog openfileDlg = new SaveFileDialog();
            openfileDlg.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            openfileDlg.Title = "选择导出为文件的位置";
            openfileDlg.Filter = "CSV文件|*.csv";
            if (openfileDlg.ShowDialog() == true)
            {
            	txtPath.Text = openfileDlg.FileName;
                btnOK.IsEnabled = true;
            }
		}
		
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if(btnOK.Content.ToString() == "开始查找")
			{
				btnOK.IsEnabled = false;
				groupUsr.Visibility = Visibility.Hidden;
				scrollUsr.Visibility = Visibility.Visible;
				
				string[] usrIds =  txtUsr.Text.Split(new String[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				List<MyTreeNode> findResultNodes = new List<MyTreeNode>();
				for (int i = 0; i < usrIds.Length; i++) 
				{
					CheckBox newCheck = new CheckBox();
					newCheck.Content = usrIds[i];
					newCheck.IsEnabled = false;
					listUsr.Children.Add(newCheck);
					
					//查找
					List<MyTreeNode> findNodes = SearchNode(usrIds[i]);
					if(findNodes.Count > 0)
					{
						findResultNodes.AddRange(findNodes);
						newCheck.IsChecked = true;
						scrollUsr.ScrollToEnd();
					}
					
	   				DoEvents();
				}
				
				//导出
				Export2CSV(findResultNodes, txtPath.Text);
				
				if(findResultNodes.Count > 0)
				{
					MessageBox.Show("查找并导出结果完成！\n一共查找到了"+findResultNodes.Count+"个结果！");
				}
				else
				{
					MessageBox.Show("没有查找到任何结果！");
				}
				btnOK.IsEnabled = true;
				btnOK.Content = "返回编辑模式";
			}
			else
			{
				groupUsr.Visibility = Visibility.Visible;
				scrollUsr.Visibility = Visibility.Hidden;
				listUsr.Children.Clear();
				btnOK.Content = "开始查找";
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
		
		private List<MyTreeNode> SearchNode(string searchTxt)
		{
			List<MyTreeNode> findNodes = new List<MyTreeNode>();
			string searchType = (comboSearchType.SelectedItem as ComboBoxItem).Content.ToString();
            if (searchType == "根据ID查找")
            {
                findNodes = MyTrees.FindNodeById(searchTxt);
            }
            else if (searchType == "根据姓名查找")
            {
                findNodes = MyTrees.FindNodeByName(searchTxt);
            }
            else if (searchType == "根据级别查找")
            {
            	ushort txt2num = 0;
                if (ushort.TryParse(searchTxt, out txt2num))
                {
               		findNodes = MyTrees.FindNodeByLevel(txt2num);
                }
            }
            else if (searchType == "根据下线人数查找")
            {
                int txt2num = 0;
                if (int.TryParse(searchTxt, out txt2num))
                {
                    findNodes = MyTrees.FindNodeByChildrenCount(txt2num);
                }
            }
            else if (searchType == "根据下线层数查找")
            {
                ushort txt2num = 0;
                if (ushort.TryParse(searchTxt, out txt2num))
                {
                    findNodes = MyTrees.FindNodeByChildrenLevels(txt2num);
                }
            }
            return findNodes;
		}
		
		private void Export2CSV(List<MyTreeNode> nodes ,string outputfile)
        {
            StringBuilder allLines = new StringBuilder("会员ID,会员姓名,所在层级,父节点ID,下线人数,下线层数,所在行数");
            for (int i = 0; i < nodes.Count; i++) 
            {
            	MyTreeNode node = nodes[i];
            	allLines.Append("\r\n");
	            allLines.Append(node.SysId);
	            allLines.Append(",");
	            allLines.Append(node.Name);
	            allLines.Append(",");
	             allLines.Append(node.Level);
	            allLines.Append(",");
	            allLines.Append(node.TopId);
	            allLines.Append(",");
	            allLines.Append(node.ChildrenCount);
	            allLines.Append(",");
	            allLines.Append(node.ChildrenLevels);
	            allLines.Append(",");
	            allLines.Append(node.LineCount);
            }

            StreamWriter mysw = new StreamWriter(outputfile, false, Encoding.Default);
            mysw.Write(allLines);
            mysw.Close();
        }
	}
}