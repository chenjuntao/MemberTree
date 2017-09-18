using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
//using OfficeOpenXml;

namespace MemberTree
{
    public class MyTreeNode
    {
        public string SysId { get; set; }
        public string Name { get; set; }
        public string TopId { get; set; }
        public int Level { get; set; }
        public int LineCount { get; set; }
        
        //所有的后代子孙节点数量
        public int ChildrenCount { get; set; }
        //所有的后代子孙最深级别数
        public int ChildrenLevels { get; set; }
        
        //子节点集合
        public List<MyTreeNode> ChildrenNodes = new List<MyTreeNode>();
        
        public MyTreeNode()
        {
        }
        
        public MyTreeNode(string sysId, string topId, string name, int lineCount, int upperLower, int DBCSBC, int trim)
        {
            this.SysId = sysId;
            this.Name = name;
            this.TopId = topId;
            this.Level = 0;
           
            this.LineCount = lineCount;
            
            if(trim == 1)
            {
            	SysId = SysId.Trim();
            	TopId = TopId.Trim();
            }
            else if(trim == 2)
            {
            	SysId = SysId.TrimStart();
            	TopId = TopId.TrimStart();
            }
            else if(trim == 3)
            {
            	SysId = SysId.TrimEnd();
            	TopId = TopId.TrimEnd();
            }
            if (upperLower == 1)
            {
                SysId = SysId.ToLower();
                TopId = TopId.ToLower();
            }
            else if (upperLower == 2)
            {
                SysId = SysId.ToUpper();
                TopId = TopId.ToUpper();
            }
            if (DBCSBC == 1)
            {
                SysId = TextUtil.SBCToDBC(SysId);
                TopId = TextUtil.SBCToDBC(TopId);
            }
            else if (DBCSBC == 2)
            {
                SysId = TextUtil.DBCToSBC(SysId);
                TopId = TextUtil.DBCToSBC(TopId);
            }
        }
    }
}
