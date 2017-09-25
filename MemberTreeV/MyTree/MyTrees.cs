using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Windows;
//using OfficeOpenXml;

namespace MemberTree
{
	/// <summary>
	/// 保存树节点数据，计算树结构算法
	/// </summary>
    public static class MyTrees
    {
    	internal static IMyTreeDBV treeDB;
    	public static void InitTreeDB(bool isSqlite)
    	{
    		if(isSqlite)
			{
				treeDB = new MyTreeDBVSqlite();
			}
			else
			{
				treeDB = new MyTreeDBVMysql();
			}
    	}
    	
    	public static void SetDBName(string dbName)
    	{
    		 treeDB.ConnectDB(dbName);
    	}
    	
    	public static List<string> GetDBs()
    	{
    		return treeDB.GetDBs();
    	}
    	
    	#region 打开关闭连接
    	private static int openTimes = 0;
		public static void OpenDB()
		{
			treeDB.OpenDB();
			openTimes++;
		}
		public static void CloseDB()
		{
			openTimes--;
			if(openTimes == 0)
			{
				treeDB.CloseDB();
			}
		}
		#endregion
		
        #region 查询特定不同类型的节点
        
        private static List<MyTreeNode> treeRootNodes;
        internal static List<MyTreeNode> GetTreeRootNodes()
        {
        	if(treeRootNodes == null)
        	{
	        	MyTrees.OpenDB();
	        	string sql = "select sysid,topid,name,level,sublevel,subcount,subcountall from "
					+ treeDB.TableName + "_calc where sysid in (select v from " 
	        		+ treeDB.TableName + "_profile where k='Tree') order by subcountall desc";
	            treeRootNodes = treeDB.SearchNode(sql);
	            MyTrees.CloseDB();
        	}
            return treeRootNodes;
        }

        private static List<string> conflictNodes;
        internal static List<string> GetIdConflictNodes()
        {
        	if(conflictNodes == null)
        	{
	        	MyTrees.OpenDB();
	        	string sql = "select * from "
					+ treeDB.TableName + "_calc where sysid in (select v from " 
	        		+ treeDB.TableName + "_profile where k='Conflict')";
	            conflictNodes = treeDB.SearchString(sql);
	            MyTrees.CloseDB();
        	}
            return conflictNodes;
        }
        
        private static Dictionary<string, string> leafAloneNodes;
        internal static Dictionary<string, string> GetLeafAloneNodes()
        {
        	if(leafAloneNodes == null)
        	{
	        	MyTrees.OpenDB();
	        	string sql = "select * from "
					+ treeDB.TableName + "_calc where sysid in (select v from " 
	        		+ treeDB.TableName + "_profile where k='Leaf')";
	            List<string> nodes = treeDB.SearchString(sql);
	            MyTrees.CloseDB();
	            
	            leafAloneNodes = new Dictionary<string, string>();
	            foreach (string node in nodes) {
	            	string sysid = node.Substring(0,node.IndexOf(","));
	            	if(!leafAloneNodes.ContainsKey(sysid))
	            	{
	            		leafAloneNodes.Add(sysid, node);
	            	}
	            }
        	}
            return leafAloneNodes;
        }
        
        private static Dictionary<string, string> ringNodes;
        internal static Dictionary<string, string> GetRingNodes()
        {
        	if(ringNodes == null)
        	{
	        	MyTrees.OpenDB();
	        	string sql = "select * from "
					+ treeDB.TableName + "_calc where sysid in (select v from " 
	        		+ treeDB.TableName + "_profile where k='Ring')";
	            List<string> nodes = treeDB.SearchString(sql);
	            MyTrees.CloseDB();
	            
	            ringNodes = new Dictionary<string, string>();
	            foreach (string node in nodes) {
	            	string sysid = node.Substring(0,node.IndexOf(","));
	            	if(!ringNodes.ContainsKey(sysid))
	            	{
	            		ringNodes.Add(sysid, node);
	            	}
	            }
        	}
            return ringNodes;
        }
        
        private static List<string> tableOptCols;
        internal static List<string> GetTableOptCols()
        {
        	if(tableOptCols == null)
        	{
	        	MyTrees.OpenDB();
	        	string sql = "select v from "
	        		+ treeDB.TableName 
	        		+ "_profile where k='TableOptCol'";
	            tableOptCols = treeDB.SearchString(sql);
	            MyTrees.CloseDB();
        	}
            return tableOptCols;
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
        	treeDB.OpenDB();
        	List<MyTreeNode> result = treeDB.SearchNode(sql, param);
        	treeDB.CloseDB();
        	return result;
        }
        
        public static MyTreeNode GetNodeBySysId(string sysId)
		{
			string sql = "select sysid,topid,name,level,sublevel,subcount,subcountall from "
				+ treeDB.TableName + "_calc where sysid='" + sysId + "' order by subcountall desc";
			List<MyTreeNode> result = treeDB.SearchNode(sql);
			if(result.Count>0)
				return result[0];
			else
				return null;
		}

        public static string GetStringBySysId(string sysId)
		{
			string sql = "select * from " + treeDB.TableName + "_calc where sysid='" + sysId + "' order by subcountall desc";
			List<string> result = treeDB.SearchString(sql);
			if(result.Count>0)
				return result[0];
			else
				return null;
		}
		
		public static List<MyTreeNode> GetNodesByTopId(string topId)
		{
			string sql = "select sysid,topid,name,level,sublevel,subcount,subcountall from "
				+ treeDB.TableName + "_calc where topid='" + topId + "' order by subcountall desc";
            return treeDB.SearchNode(sql);
		}
		
		public static List<string> GetAllByTopIds(string topIds)
		{
			string sql = "select * from "+treeDB.TableName+"_calc where topid in ("+topIds+") order by subcountall desc";
            return treeDB.SearchString(sql);
		}
        
        #endregion
    }
}
