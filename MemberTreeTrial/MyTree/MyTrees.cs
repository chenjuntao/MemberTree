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
    }
}
