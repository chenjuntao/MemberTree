using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
//using OfficeOpenXml;

namespace MemberTree
{
    public class ExportCSV
    {
        private static int row;
        private static int allRow;      
        
        public static void ExportNodes(MyTreeNode node)
        {
        	if (node == null)
            {
                MessageBox.Show("必须选中一个节点！");
                return;
            }
        	SaveFileDialog saveFileDlg = new SaveFileDialog();
		    saveFileDlg.Title = "选择将会员树导出为文件的位置";
//		    saveFileDlg.Filter = "CSV文件|*.csv|Excel2007文件|*.xlsx";
		    saveFileDlg.Filter = "CSV文件|*.csv";
		    saveFileDlg.FileName = node.ToString();
		    if (saveFileDlg.ShowDialog() == true)
		    {
            	if(File.Exists(saveFileDlg.FileName))
            	{
            		File.Delete(saveFileDlg.FileName);
            	}
            	ExportCSV.Export2CSV(node, saveFileDlg.FileName);
		    }
        }

        private static List<string> ringNodeIds = new List<string>();
        //判断闭环是否关闭
        private static bool isRingClose(string id)
        {
        	if (MyTrees.RingNodes.ContainsKey(id))
            {
                if (ringNodeIds.Contains(id))
                {
                    return true;
                }
                else
                {
                    ringNodeIds.Add(id);
                }
            }
            return false;
        }

        public static void Export2CSV(MyTreeNode node, string outputfile)
        {
            ringNodeIds.Clear();
            WindowView.notify.SetProcessBarVisible(true);
            WindowView.notify.SetStatusMessage("开始导出文件......");
            TimingUtil.StartTiming();
  
            StreamWriter mysw = new StreamWriter(outputfile, true, Encoding.Default);
           
            List<string> optCols = MyTrees.TableOptCols;
            string header = string.Join(",", optCols.ToArray());
            header = "会员ID,父级ID,会员姓名,所在层级,下级层数,直接下级会员数,下级会员总数," + header;
            mysw.WriteLine(header);

            StringBuilder allLines = new StringBuilder();
         
            row = 2;
            allRow = node.ChildrenCount + 1;
  
            //导出所有父节点
            ExportAllParents2CSV(mysw, allLines, node);
            
            Export2CSVImp(mysw, allLines, node, true);
            
            mysw.Close();

            WindowView.notify.SetStatusMessage(TimingUtil.EndTiming());
            WindowView.notify.SetProcessBarVisible(false);
        }
        
        //导出所有父节点
        private static void ExportAllParents2CSV(StreamWriter mysw, StringBuilder allLines, MyTreeNode node)
        {
        	WindowView.notify.SetStatusMessage("正在导出该节点的所有父节点。。。");
            List<MyTreeNode> parentNodes = MyTrees.FindToRootNodeList(node.TopId);
            parentNodes.Reverse();
            allRow += parentNodes.Count;
            
            for (int i = 0; i < parentNodes.Count; i++) 
            {
            	MyTreeNode parentNode = parentNodes[i];
            	allLines.Clear();
	            allLines.Append(parentNode.SysId);
	            allLines.Append(",");
	            allLines.Append(parentNode.TopId);
	            allLines.Append(",");
	            allLines.Append(parentNode.Name);
	            allLines.Append(",");
	            allLines.Append(parentNode.Level);
	            allLines.Append(",");
	            allLines.Append(parentNode.ChildrenLevels);
	            allLines.Append(",");
	            allLines.Append(parentNode.ChildrenNodes.Count);
	            allLines.Append(",");
	            allLines.Append(parentNode.ChildrenCount);
	            foreach (string otherProp in parentNode.OtherProps) 
	            {
	            	allLines.Append(",");
	            	allLines.Append(otherProp);
	            }
	            
	            mysw.WriteLine(allLines.ToString());
            }
        }

        private static void Export2CSVImp(StreamWriter mysw, StringBuilder allLines, MyTreeNode node, bool recu)
        {
            if (isRingClose(node.SysId))
            {
                return;
            }

            allLines.Clear();
            allLines.Append(node.SysId);
            allLines.Append(",");
            allLines.Append(node.TopId);
            allLines.Append(",");
            allLines.Append(node.Name);
            allLines.Append(",");
            allLines.Append(node.Level);
            allLines.Append(",");
            allLines.Append(node.ChildrenLevels);
            allLines.Append(",");
            allLines.Append(node.ChildrenNodes.Count);
            allLines.Append(",");
            allLines.Append(node.ChildrenCount);
            foreach (string otherProp in node.OtherProps) 
            {
            	allLines.Append(",");
            	allLines.Append(otherProp);
            }
            
            mysw.WriteLine(allLines.ToString());
            row++;
            if (row % 100 == 0)
            {
                WindowView.notify.SetProcessBarValue((int)(100.0 * row / allRow));
                WindowView.notify.SetStatusMessage("正在导出第" + row + "个节点（总共" + allRow + "个节点）");
            }

            if(recu)
            {
				List<MyTreeNode> childrenNodes = MyTrees.GetNodesByTopId(node.SysId);
	            foreach (MyTreeNode subNode in childrenNodes)
	            {
	                Export2CSVImp(mysw ,allLines, subNode, recu);
	            }
            }
        }
    }
}
