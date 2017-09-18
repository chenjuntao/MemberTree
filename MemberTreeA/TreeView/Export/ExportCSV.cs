using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;
//using OfficeOpenXml;

namespace MemberTree
{
    public class ExportCSV
    {
        private static int row;
        private static int allRow;
        private static int step;        

        //public static void ExportAll2CSV(string outputfile)
        //{
        //    MainWindow.notify.SetProcessBarVisible(true);
        //    MainWindow.notify.SetStatusMessage("开始导出文件......");

        //    StringBuilder allLines = new StringBuilder("会员ID,会员姓名,所在层级,下线人数");
        //    row = 2;
        //    foreach (var item in MyTreeNode.AllNodes.Values)
        //    {
        //        foreach (MyTreeNode node in item.Values)
        //        {
        //            allLines.Append("\n");
        //            allLines.Append(node.SysId);
        //            allLines.Append(",");
        //            allLines.Append(node.RealName);
        //            allLines.Append(",");
        //            allLines.Append(node.Level);
        //            allLines.Append(",");
        //            allLines.Append(node.ChildrenCount);
        //            row++;
        //            if (row % 1000 == 0)
        //            {
        //                MainWindow.notify.SetProcessBarValue((int)(100.0 * row / MyTreeNode.allNodesCount));
        //                MainWindow.notify.SetStatusMessage("正在导出第" + row + "个节点（总共" + MyTreeNode.allNodesCount + "个节点）");
        //            }
        //        }
        //    }

        //    MainWindow.notify.SetStatusMessage("正在将数据写入CSV文件。。。");

        //    StreamWriter mysw = new StreamWriter(outputfile, false, Encoding.UTF8);
        //    mysw.Write(allLines);
        //    mysw.Close();

        //    MainWindow.notify.SetStatusMessage("会员树导出到CSV成功！");
        //    MainWindow.notify.SetProcessBarVisible(false);
        //}
        
        public static void ExportNodes(List<MyTreeNode> nodes)
        {
        	SaveFileDialog openfileDlg = new SaveFileDialog();
		    openfileDlg.Title = "选择将会员树导出为文件的位置";
		    openfileDlg.Filter = "CSV文件|*.csv|Excel2007文件|*.xlsx";
		    openfileDlg.FileName = nodes[0].ToString();
		    if (openfileDlg.ShowDialog() == true)
		    {
	            if (openfileDlg.FileName.EndsWith(".csv"))
	            {
	            	ExportCSV.Export2CSV(nodes[0], openfileDlg.FileName);
	            }
	            else
	            {
//	                Export.Export2Excel(nodes, openfileDlg.FileName);
	            }
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
            
            WindowView.notify.SetStatusMessage("正在将数据写入CSV文件。。。");
            StreamWriter mysw = new StreamWriter(outputfile, true, Encoding.Default);
            mysw.WriteLine(TextUtilTool.CSVOutHeader);

            StringBuilder allLines = new StringBuilder();
            List<MyTreeNode> parentNodes = MyTrees.FindToRootNodeList(node.TopId);
            parentNodes.Reverse();
            row = 2;
            allRow = node.ChildrenCount + 1;
            allRow += parentNodes.Count;
            step = allRow > 100 ? allRow / 100 : 1;
            for (int i = 0; i < parentNodes.Count; i++) 
            {
            	Export2CSVImp(mysw, allLines, parentNodes[i], false);
            }
            Export2CSVImp(mysw, allLines, node, true);

            mysw.Close();

            WindowView.notify.SetStatusMessage("");
            WindowView.notify.SetProcessBarVisible(false);
        }

        private static void Export2CSVImp(StreamWriter mysw, StringBuilder allLines, MyTreeNode node, bool recu)
        {
            if (isRingClose(node.SysId))
            {
                return;
            }
            
            MyTreeNodeDB nodeDB = MyTrees.treeDB[node.SysId];
            if(nodeDB == null)
            {
            	nodeDB = new MyTreeNodeDB();
            }

            allLines.Clear();
//            allLines.Append("\r\n");
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
            allLines.Append(",");
            allLines.Append(nodeDB.Account);
            allLines.Append(",");
            allLines.Append(nodeDB.IdCard);
            allLines.Append(",");
            allLines.Append(nodeDB.Tel);
            allLines.Append(",");
            allLines.Append(nodeDB.Addr);
            allLines.Append(",");
            allLines.Append(nodeDB.Bank);
            allLines.Append(",");
            allLines.Append(nodeDB.BankCard);
            allLines.Append(",");
            allLines.Append(nodeDB.Email);
            
            mysw.WriteLine(allLines.ToString());
            row++;
            if (row % step == 0)
            {
                WindowView.notify.SetProcessBarValue((int)(100.0 * row / allRow));
                WindowView.notify.SetStatusMessage("正在导出第" + row + "个节点（总共" + allRow + "个节点）");
            }

            if(recu)
            {
//            	List<MyTreeNode> childrenNodes = MyTrees.FindChildrenNodes(node.SysId);
				List<MyTreeNode> childrenNodes = node.ChildrenNodes;
	            foreach (MyTreeNode subNode in childrenNodes)
	            {
	                Export2CSVImp(mysw ,allLines, subNode, recu);
	            }
            }
        }
    }
}
