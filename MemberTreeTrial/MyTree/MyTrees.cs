using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace MemberTree
{
	/// <summary>
	/// 保存树节点数据，计算树结构算法
	/// </summary>
    public static class MyTrees
    {
    	public static DatasetInfo dataset;
    	
    	public static void InitTreeDB(bool isSqlite)
    	{
    	}
    	
    	public static void SetDBName(string dbName)
    	{
    		
    	}
    	
    	private static int openTimes = 0;
		public static void OpenDB()
		{
		}
		public static void CloseDB()
		{
		}
		
		public static MyTreeNode FindParentNode(string parentId)
        {
        	if(parentId != "")
        	{
	            if (allNodes.ContainsKey(parentId))
	            {
	                return allNodes[parentId];
	            }
        	}

            return null;
        }
		
        #region 查询特定不同类型的节点
        
        private static Dictionary<string, MyTreeNode> allNodes = new Dictionary<string, MyTreeNode>();
        internal static List<MyTreeNode> NoParentNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> TreeRootNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> LeafAloneNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> IdConflictNodes = new List<MyTreeNode>();
        internal static Dictionary<string, MyTreeNode> RingNodes = new Dictionary<string, MyTreeNode>();
        internal static List<string> TableOptCols = new List<string>();
        
        internal static int GetTreeRootNodesCount()
        {
            return 0;
        }
        internal static List<MyTreeNode> GetTreeRootNodes(int pageNo, int pageSize)
        {
            return null;
        }

        internal static int GetIdConflictNodesCount()
        {
            return 0;
        }
        internal static List<string> GetIdConflictNodes(int pageNo, int pageSize)
        {
            return null;
        }
        
        internal static int GetLeafAloneNodesCount()
        {
            return 0;
        }
        internal static Dictionary<string, string> GetLeafAloneNodes(int pageNo, int pageSize)
        {
            return null;
        }
        internal static List<string> GetLeafAloneNodeIds()
        {
        	return null;
        }
        
       	internal static int GetRingNodesCount()
       	{
       		return 0;
       	}
        internal static Dictionary<string, string> GetRingNodes(int pageNo, int pageSize)
        {
        	return null;
        }
        internal static List<string> GetRingNodeIds()
        {
        	return null;
        }
        	
        #endregion

        #region 自定义查找
        
        public static List<MyTreeNode> FindToRootNodeList(string parentId)
        {
        	List<MyTreeNode> nodes = new List<MyTreeNode>();
        	MyTreeNode parentNode = GetNodeBySysId(parentId);
        	while(parentNode != null)
        	{
        		nodes.Add(parentNode);
        		parentNode = GetNodeBySysId(parentNode.TopId);
        	}
        	
        	return nodes;
        }
        
        public static List<string> FindToRootAllList(string parentId)
        {
        	List<string> nodes = new List<string>();
        	string parentNode = GetStringBySysId(parentId);
        	while(parentNode != null)
        	{
        		nodes.Add(parentNode);
        		string[] parentNodes = parentNode.Split(new char[] { ',' });
        		parentNode = GetStringBySysId(parentNodes[1]);
        	}
        	
        	return nodes;
        }
        
        public static List<MyTreeNode> FindBySql(string sql, List<string> param)
        {
        	return null;
        }
        
        public static MyTreeNode GetNodeBySysId(string sysId)
		{
			return null;
		}

        public static string GetStringBySysId(string sysId)
		{
			return null;
		}
		
		public static List<MyTreeNode> GetNodesByTopId(string topId)
		{
			return null;
		}
		
		public static List<string> GetAllByTopIds(string topIds)
		{
			return null;
		}
        
        #endregion
    
        public static void OpenSampleData(string datName)
        {
            int row = 0;
            string datStr = SampleData1.DAT;
            if(datName == "样例数据2")
            {
            	datStr = SampleData2.DAT;
            }
            string[] allLines = datStr.Split(new String[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            
            string firstLine = allLines[0]; //第一行是表头，读取之后不处理，直接跳过
            DatasetInfo.SetCSVHeader(firstLine);
                
            for (int i = 1; i < allLines.Length; i++) 
            {
            	string line = allLines[i];
		        MyTreeNode myNode = new MyTreeNode(line);
		        	
		        if(allNodes.ContainsKey(myNode.SysId)) //ID有重复的节点
		        {
		            IdConflictNodes.Add(myNode);
		        }
		        else
		        {
		            allNodes.Add(myNode.SysId, myNode);
		        }
		
		        row++;
		        if (row % 1000 == 0)
	            {
//		            MainWindow.notify.SetStatusMessage("【第一步：正在读取第" + row + "个节点】——>【第二步：构造树结构】——>【第三步：写入数据库】");
	            }
            }
            
			//构造计算树结构----------------------------------------------------------------------------------------------------------
            row = 0;
            foreach (MyTreeNode node in allNodes.Values)
            {
                //将节点加入树中合适的位置去
                ConstructTree(node);

                row++;
                if (row % 1000 == 0)
                {
//                    	MainWindow.notify.SetProcessBarValue(row * 100.0 / allNodeCount);
//                      MainWindow.notify.SetStatusMessage("【第一步：读取数据完成】——>【第二步：正在构造树结构" + row + "/" + allNodeCount +"】——>【第三步：写入数据库】");
                }
            }
            foreach (MyTreeNode node in IdConflictNodes)
            {
                //将节点加入树中合适的位置去
                ConstructTree(node);
            }
            
            #region 找出所有构成树的节点
            foreach (MyTreeNode node in NoParentNodes) 
            {
            	if(node.ChildrenCount > 0)
            	{
            		TreeRootNodes.Add(node);
            	}
            }
            foreach (MyTreeNode node in TreeRootNodes) 
            {
            	NoParentNodes.Remove(node);
            }
            #endregion
			dataset = new DatasetInfo();
			dataset.Name = datName;
			dataset.CreateData = DateTime.Today;
			dataset.ColCount = 33;
			dataset.AllNodeCount = allNodes.Count;
			dataset.RowCount = allNodes.Count;
			dataset.TreeCount = TreeRootNodes.Count;
			dataset.TreeNodeCount = TreeRootNodes.Count;
			dataset.ConflictCount = IdConflictNodes.Count;
			dataset.LeafCount = LeafAloneNodes.Count;
			dataset.RingCount = RingNodes.Count;
        }

        //构建树（将节点加进树结构中合适的位置）
        private static void ConstructTree(MyTreeNode myNode)
        {
            //是否包含父节点
            MyTreeNode parentNode = FindParentNode(myNode.TopId);
            if (parentNode != null)
            {
                ChildrenCountInc(myNode);//所有父节点的子孙节点加1
                parentNode.ChildrenNodes.Add(myNode);
//                myNode.ParentNode = parentNode;
            }
            else
            {
                //父节点不存在
                NoParentNodes.Add(myNode);
            }
        }

        //所有父节点的子孙节点数自增，（如果需要的话，所有父节点的子孙节点最深层级数自增）
        private static void ChildrenCountInc(MyTreeNode node)
        {
        	//帮助判断是否存在闭环
            Dictionary<string, MyTreeNode> parentList = new Dictionary<string, MyTreeNode>();
            //parentList.Add(node.SysId, node);

            ushort deepLevel = 0; //深度（父节点到子节点之间的层级数之差）
            MyTreeNode parent = FindParentNode(node.TopId);
            while (parent != null)
            {
            	//判断是否构成闭环
                if (parentList.ContainsKey(parent.SysId))
                {
                    if (!RingNodes.ContainsKey(node.SysId))
                    {
                        RingNodes.Add(node.SysId, node);
                    }

                    foreach (string item in parentList.Keys)
                    {
                        if (!RingNodes.ContainsKey(item))
                        {
                            RingNodes.Add(item, parentList[item]);
                        }
                    }
                    break;
                }
                parentList.Add(parent.SysId, parent);

                parent.ChildrenCount++;
                deepLevel++;
                if(parent.ChildrenLevels < deepLevel)
                {
                	parent.ChildrenLevels = deepLevel;
                }
                
                //继续循环遍历查找父节点的父节点，直到根节点
                parent = FindParentNode(parent.TopId);
            }
            
            if(node.Level == 0)
            {
            	node.Level = (ushort)(deepLevel + 1);
            }
        }
    }
}
