using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemberTree
{
    /// <summary>
    /// MyNodeList.xaml 的交互逻辑
    /// </summary>
    public partial class MyNodeList : UserControl
    {
        public MyNodeList()
        {
            InitializeComponent();
        }
        
        public void InitCols()
        {
        	List<string> tableOptCols = MyTrees.GetTableOptCols();
        	for (int i = 0; i < tableOptCols.Count; i++) {
        		GridViewColumn col = new GridViewColumn();
	        	col.Header = tableOptCols[i];
	        	col.DisplayMemberBinding = new Binding("["+ (i + 7) +"]");
	        	gridView.Columns.Add(col);
        	}
        }

        public void SetDataSource(string dsType)
        {
        	List<string> nodes = new List<string>();
        	if (dsType == "conflict")
            {
        		grpHeader.Text = "ID重复的节点";
            	nodes = MyTrees.GetIdConflictNodes();
            }
            else if (dsType == "leaf")
            {
            	grpHeader.Text = "孤立的叶子节点";
            	nodes = MyTrees.GetLeafAloneNodes().Values.ToList();
            }
            else if (dsType == "ring")
            {
            	grpHeader.Text = "构成闭环的节点";
            	nodes = MyTrees.GetRingNodes().Values.ToList().ToList();
            }
            
        	foreach (string node in nodes) 
        	{
            	string[] nodeProp = node.Split(new String[]{","}, StringSplitOptions.None);
        		nodeList.Items.Add(nodeProp);
        	}
        }
    }
}
