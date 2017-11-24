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
    	internal static string DatasetName;
    	
    	internal static string CSVHeader = "会员Id,上级Id,会员姓名,所在层级,下级层级,直接下级数,总下级会员数";
    	internal static void SetCSVHeader(string line)
    	{
    		string[] ary = line.Split(new char[] { ',' });
    		 for (int i = 3; i < ary.Length; i++) 
    		 {
        		CSVHeader+=(","+ary[i]);
        		MyTrees.TableOptCols.Add(ary[i]);
            }
    	}
    	
        #region 查询特定不同类型的节点
        
        private static Dictionary<string, MyTreeNode> allNodes = new Dictionary<string, MyTreeNode>();
        private static List<MyTreeNode> NoParentNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> TreeRootNodes = new List<MyTreeNode>();
        internal static List<MyTreeNode> IdConflictNodes = new List<MyTreeNode>();
        internal static Dictionary<string, MyTreeNode> LeafAloneNodes = new Dictionary<string, MyTreeNode>();
        internal static Dictionary<string, MyTreeNode> RingNodes = new Dictionary<string, MyTreeNode>();
        internal static List<string> TableOptCols = new List<string>();
        internal static int AllNodeCount
        {
        	get { return allNodes.Count + IdConflictNodes.Count; }
        }
        internal static int TreeNodeCount
        {
        	get
        	{
        		int count = TreeRootNodes.Count;
        		foreach (MyTreeNode node in TreeRootNodes) 
        		{
        			count += node.ChildrenCount;
        		}
        		return count;
        	}
        }
        	
        #endregion

        #region 自定义查找

        internal static List<MyTreeNode> FindToRootNodeList(string parentId)
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
        
        internal static MyTreeNode GetNodeBySysId(string sysId)
		{
			if(sysId != "")
        	{
	            if (allNodes.ContainsKey(sysId))
	            {
	                return allNodes[sysId];
	            }
        	}

            return null;
		}
		
		internal static List<MyTreeNode> GetNodesByTopId(string topId)
		{
			if(topId != "")
        	{
	            if (allNodes.ContainsKey(topId))
	            {
	                return allNodes[topId].ChildrenNodes;
	            }
        	}

            return null;
		}

        internal static List<MyTreeNode> FindBySql(List<string> searchParams)
        {
        	List<MyTreeNode> result = new List<MyTreeNode>();
        	string sysId = searchParams[0];
        	string opt = searchParams[1];
        	if(sysId!="")
        	{
        		if(opt=="等于")
        		{
        			MyTreeNode node = GetNodeBySysId(sysId);
        			if(node !=null)
        			{
        				result.Add(node);
        			}
        		}
        		else if(opt=="开头")
        		{
        			foreach (string id in allNodes.Keys) 
        			{
        				if(id.StartsWith(sysId))
        				{
        					result.Add(allNodes[id]);
        				}
        			}
        		}
        		else if(opt=="结尾")
        		{
        			foreach (string id in allNodes.Keys) 
        			{
        				if(id.EndsWith(sysId))
        				{
        					result.Add(allNodes[id]);
        				}
        			}
        		}
        		else if(opt=="包含")
        		{
        			foreach (string id in allNodes.Keys) 
        			{
        				if(id.Contains(sysId))
        				{
        					result.Add(allNodes[id]);
        				}
        			}
        		}
        	}
        	
        	return result;
        }
		
        #endregion
    
        public static void OpenSampleData(string datName)
        {
            int row = 0;
            DatasetName = datName;
            string datStr = SampleData1.DAT;
            if(datName == "样例数据2")
            {
            	datStr = SampleData2.DAT;
            }
            string[] allLines = datStr.Split(new String[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            
            string firstLine = allLines[0]; //第一行是表头，读取之后不处理，直接跳过
            SetCSVHeader(firstLine);
                
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
            	else if(!LeafAloneNodes.ContainsKey(node.SysId))
            	{
            		LeafAloneNodes.Add(node.SysId, node);
            	}
            }
            NoParentNodes.Clear();
            #endregion
        }

        //构建树（将节点加进树结构中合适的位置）
        private static void ConstructTree(MyTreeNode myNode)
        {
            //是否包含父节点
            MyTreeNode parentNode = GetNodeBySysId(myNode.TopId);
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
            MyTreeNode parent = GetNodeBySysId(node.TopId);
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
                parent = GetNodeBySysId(parent.TopId);
            }
            
            if(node.Level == 0)
            {
            	node.Level = (ushort)(deepLevel + 1);
            }
        }
    }
}
