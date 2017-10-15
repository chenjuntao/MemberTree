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
    	internal static IMyTreeDBA treeDB;
    	public static void InitTreeDB(bool isSqlite)
    	{
    		if(isSqlite)
			{
				treeDB = new MyTreeDBASqlite();
			}
			else
			{
				treeDB = new MyTreeDBAMysql();
			}
    	}
    	
    	public static void SetDBName(string dbName)
    	{
    		 treeDB.ConnectDB(dbName);
    	}
    	
        #region 查找
        
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
        
        #endregion
        
       
        #region 计算构造树结构的具体数据结构和算法

        internal static int allNodeCount = 0;
        
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
         
        private static Dictionary<string, MyTreeNode> allNodes = new Dictionary<string, MyTreeNode>();
        
        internal static List<MyTreeNode> TreeRootNodes = new List<MyTreeNode>();

        internal static List<MyTreeNode> NoParentNodes = new List<MyTreeNode>();
        
        internal static List<MyTreeNode> IdConflictNodes = new List<MyTreeNode>();
        
        internal static Dictionary<string, MyTreeNode> RingNodes = new Dictionary<string, MyTreeNode>();
        
        #endregion
        
        #region 读取数据(csv或tab)，计算树结构，写入数据库

        public static void OpenDBFile(string filepath, string separator, bool checkHead)
        {
        	List<string> dbs = treeDB.GetDBs();
        	FileInfo fileInfo = new FileInfo(filepath);
        	string dbName = fileInfo.Name.Replace(fileInfo.Extension, "").ToLower();
	        if(dbs.Contains(dbName))
	        {
	        	MessageBoxResult msgResult = MessageBox.Show("该数据文件已经存在，是否重新计算并覆盖旧文件？","提示",MessageBoxButton.OKCancel);
	        	if(msgResult == MessageBoxResult.OK)
	        	{
	        		treeDB.DeleteDB(dbName);
	        	}
	        	else
	        	{
	        		return;
	        	}
	        }
	        
	        TimingUtil.StartTiming();
           
	        //读取文件数据并构造树节点-------------------------------------
	        bool readSuccess = ReadLine2Node(filepath, separator, checkHead);
			if(readSuccess)
			{
				//将树节点计算并构造树结构-------------------------------------
				ConstructTree();
            	//写入数据库-------------------------------------------------
            	Write2DB(filepath, dbName, separator);
            	
            	WindowAdmin.notify.SetStatusMessage(TimingUtil.EndTiming());
			}
			else
			{
				WindowAdmin.notify.SetProcessBarVisible(false);
			}
            allNodes.Clear();
            NoParentNodes.Clear();
            IdConflictNodes.Clear();
            TreeRootNodes.Clear();
            RingNodes.Clear();
        }
        
        //读取文件数据并构造树节点
        private static bool ReadLine2Node(string filepath, string separator, bool confirm)
        {
	        Encoding encoding = TextUtil.GetFileEncodeType(filepath);
            StreamReader mysr = new StreamReader(filepath, encoding);
            int row = 0;
            try
            {
            	WindowAdmin.notify.SetProcessBarVisible(true);
                WindowAdmin.notify.SetStatusMessage("开始读取文件......");

                string firstLine = mysr.ReadLine(); //第一行是表头，读取之后不处理，直接跳过
                if (!DBUtil.CheckHead(firstLine, separator, confirm)) //检查表头
                {
                	string errMsg = "文件格式不正确，最少必须包含三列，前三列为“会员id,上级会员id,会员姓名”且顺序固定，请重新选择正确的文件！";
                	MessageBox.Show(errMsg);
                	WindowAdmin.notify.SetStatusMessage(errMsg);
                	mysr.Close();
					return false;
                }
                while(!mysr.EndOfStream)
                {
                	string line = mysr.ReadLine();
                	string[] aryline = line.Split(new String[] { separator }, StringSplitOptions.None);
                	if(!DBUtil.CheckLen(aryline))
		        	{
                		//发现出错的数据
                		string errMsg = "该csv数据文件中包含错误数据，请先对csv文件进行检查校准！";
						MessageBox.Show(errMsg);
						WindowAdmin.notify.SetStatusMessage(errMsg);
						mysr.Close();
						return false;
		        	}

		            MyTreeNode myNode = new MyTreeNode(aryline[0], aryline[1]);
		        	
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
		                WindowAdmin.notify.SetStatusMessage("【第一步：正在读取第" + row + "个节点】——>【第二步：构造树结构】——>【第三步：写入数据库】");
		            }
                }
                
                allNodeCount = row;
            }
            catch (Exception ex)
            {
            	WindowAdmin.notify.SetProcessBarVisible(false);
                WindowAdmin.notify.SetStatusMessage("文件读取出错！+\n" + ex.Message);
                mysr.Close();
            	return false;
            }
            
            mysr.Close();
            return true;
        }

        //将树节点计算并构造树结构
        private static void ConstructTree()
        {
        	int row = 0;
            try
            {
                foreach (MyTreeNode node in allNodes.Values)
                {
                    //将节点加入树中合适的位置去
                    ConstructTree(node);

                    row++;
                    if (row % 1000 == 0)
                    {
                    	WindowAdmin.notify.SetProcessBarValue(row * 100.0 / allNodeCount);
                        WindowAdmin.notify.SetStatusMessage("【第一步：读取数据完成】——>【第二步：正在构造树结构" + row + "/" + allNodeCount +"】——>【第三步：写入数据库】");
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
            }
            catch (Exception ex)
            {
                WindowAdmin.notify.SetStatusMessage("发生异常：" + ex.Message + "在第" + row + "行!");
            }
        }

        //构建树（将节点加进树结构中合适的位置）
        private static void ConstructTree(MyTreeNode myNode)
        {
            //是否包含父节点
            MyTreeNode parentNode = FindParentNode(myNode.TopId);
            if (parentNode != null)
            {
                ChildrenCountInc(myNode);//所有父节点的子孙节点加1
//                parentNode.ChildrenNodes.Add(myNode);
                parentNode.SubCount++;
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
        	
        private static void Write2DB(string filepath, string dbName, string separator)
        {
        	if(treeDB.CreateDB(dbName))
            {
	            int row = 0;
	            Encoding encoding = TextUtil.GetFileEncodeType(filepath);
	            StreamReader mysr = new StreamReader(filepath, encoding);
	            treeDB.BeginInsert();
//	            try
//	            {
	            	string line = mysr.ReadLine();
	                while(!mysr.EndOfStream)
	                {
	                	line = mysr.ReadLine();
	                	string[] aryline = line.Split(new String[] { separator }, StringSplitOptions.None);
	                	treeDB.InsertNodes(MyTrees.allNodes[aryline[0]] ,aryline);

	                    row++;
	                    if (row % 100 == 0)
	                    {
	                    	treeDB.TransCommit(false);
	                    	WindowAdmin.notify.SetProcessBarValue(row * 100.0 / allNodeCount);
	                        WindowAdmin.notify.SetStatusMessage("【第一步：读取数据完成】——>【第二步：构造树结构完成】——>【第三步：正在写入数据库" + row + "/" + allNodeCount +"】");
	                    }
	                }
	                treeDB.TransCommit(true);
	                
	                WindowAdmin.notify.SetStatusMessage("【第一步：读取数据完成】——>【第二步：构造树结构完成】——>【第三步：正在创建数据库索引。。。】");
	                treeDB.CreateIndex();
	                
	                WindowAdmin.notify.SetStatusMessage("【第一步：读取数据完成】——>【第二步：构造树结构完成】——>【第三步：正在写入数据概要信息。。。】");
	                treeDB.CreateProfile();
	                
	                WindowAdmin.notify.SetProcessBarVisible(false);
            		WindowAdmin.notify.SetStatusMessage("计算完成！");
//	            }
//	            catch (Exception ex)
//	            {
//	            	WindowAdmin.notify.SetProcessBarVisible(false);
//	                WindowAdmin.notify.SetStatusMessage("文件读取出错！+\n" + ex.Message);
//	            }
	            mysr.Close();
            }
        }
        
        #endregion
    }
}
