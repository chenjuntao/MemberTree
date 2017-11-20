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
		
        #region 查询特定不同类型的节点
        
        private static Dictionary<string, MyTreeNode> allNodes = new Dictionary<string, MyTreeNode>();
        internal static MyTreeNode RootNode=null;
        internal static List<MyTreeNode> NoParentNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> IdConflictNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> LeafAloneNodes = new List<MyTreeNode>();
        internal static Dictionary<string, MyTreeNode> RingNodes = new Dictionary<string, MyTreeNode>();
        
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
        private static List<string> leafAloneNodeIds;
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
       	private static List<string> ringNodes;
        internal static List<string> GetRingNodeIds()
        {
        	return null;
        }
        
        private static List<string> tableOptCols;
        internal static List<string> GetTableOptCols()
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
    
        
        
         
        public static void OpenCSVFile(string filepath)
        {
            ClearAllNodes();
            StreamReader mysr = new StreamReader(filepath, Encoding.UTF8);
            int row = 0;
            List<string> errLines = new List<string>();
            try
            {
                string firstLine = mysr.ReadLine(); //第一行是表头，读取之后不处理，直接跳过
                MyTreeNode.SetCSVHeader(firstLine);
                
                while(!mysr.EndOfStream)
                {
                	string line = mysr.ReadLine();
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
//		                MainWindow.notify.SetStatusMessage("【第一步：正在读取第" + row + "个节点】——>【第二步：构造树结构】——>【第三步：写入数据库】");
		            }
                }
                
                //处理出错的数据
	            if(errLines.Count>0)
	            {
	            	MessageBox.Show("该csv数据文件中包含大量的错误数据，请先对csv文件进行检查校准！");
	            	return;
	            }
                
                allNodeCount = row;
            }
            catch (Exception ex)
            {
            	MessageBox.Show("文件读取出错！+\n" + ex.Message);
                return;
            }
            finally
            {
                mysr.Close();
            }
            
			//构造计算树结构----------------------------------------------------------------------------------------------------------
            try
            {
                row = 0;
                foreach (MyTreeNode node in allNodes.Values)
                {
                    //将节点加入树中合适的位置去
                    ConstructTree(node);

                    row++;
                    if (row % 1000 == 0)
                    {
//                    	MainWindow.notify.SetProcessBarValue(row * 100.0 / allNodeCount);
//                        MainWindow.notify.SetStatusMessage("【第一步：读取数据完成】——>【第二步：正在构造树结构" + row + "/" + allNodeCount +"】——>【第三步：写入数据库】");
                    }
                }
                foreach (MyTreeNode node in IdConflictNodes)
                {
                    //将节点加入树中合适的位置去
                    ConstructTree(node);
                }

//                MainWindow.notify.SetProcessBarVisible(false);
//                MainWindow.notify.SetStatusMessage("计算完成！");
            }
            catch (Exception ex)
            {
            	MessageBox.Show("发生异常：" + ex.Message + "在第" + row + "行!");
            }
        }
        
        private static void ClearAllNodes()
        {
            allNodes.Clear();
            NoParentNodes.Clear();
            IdConflictNodes.Clear();
            LeafAloneNodes.Clear();
            RingNodes.Clear();
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
