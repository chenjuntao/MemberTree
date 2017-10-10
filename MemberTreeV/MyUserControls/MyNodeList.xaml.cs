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
    	private string dsType;
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
        	this.dsType = dsType;
        	
        	int nodesCount = 0;
        	if (dsType == "conflict")
            {
        		grpHeader.Text = "ID重复的节点";
        		nodesCount = MyTrees.GetIdConflictNodesCount();
            }
            else if (dsType == "leaf")
            {
            	grpHeader.Text = "孤立的叶子节点";
            	nodesCount = MyTrees.GetLeafAloneNodesCount();
            }
            else if (dsType == "ring")
            {
            	grpHeader.Text = "构成闭环的节点";
            	nodesCount = MyTrees.GetRingNodesCount();
            }
            
            pager.Init(nodesCount);
        }

		/// <summary>  
		/// 分页控件回调函数  
		/// </summary>  
		/// <param name="pageNo">页码，由分页控件传入</param>
		/// <param name="pageSize">每页记录数</param>
		/// <returns></returns>  
		private void LoadPageData(int pageNo, int pageSize) 
        {
			nodeList.Items.Clear();
			List<string> nodes = new List<string>();
        	if (dsType == "conflict")
            {
        		grpHeader.Text = "ID重复的节点";
            	nodes = MyTrees.GetIdConflictNodes(pageNo, pageSize);
            }
            else if (dsType == "leaf")
            {
            	grpHeader.Text = "孤立的叶子节点";
            	nodes = MyTrees.GetLeafAloneNodes(pageNo, pageSize).Values.ToList();
            }
            else if (dsType == "ring")
            {
            	grpHeader.Text = "构成闭环的节点";
            	nodes = MyTrees.GetRingNodes(pageNo, pageSize).Values.ToList().ToList();
            }
			foreach (string node in nodes) 
        	{
            	string[] nodeProp = node.Split(new String[]{","}, StringSplitOptions.None);
        		nodeList.Items.Add(nodeProp);
        	}
        }
    }
}
