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
    	public static string CSVHeader = "会员Id,上级Id,会员姓名,所在层级,下级层级,直接下级数,总下级会员数";
    	public static void SetCSVHeader(string line)
    	{
    		string[] ary = line.Split(new char[] { ',' });
    		 for (int i = 3; i < ary.Length; i++) {
        		CSVHeader+=(","+ary[i]);
            }
    	}
    	
    	
        public string SysId { get; set; }
        public string TopId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int ChildrenLevels { get; set; }
        public int ChildrenCount { get; set; }
        public int ChildrenCountAll { get; set; }
        
        public List<string> OtherProps = new List<string>();
        
        public MyTreeNode()
        {
        }
        
        public MyTreeNode(string line)
        {
        	string[] ary = line.Split(new char[] { ',' });
        	this.SysId = ary[0];
        	this.TopId = ary[1];
        	this.Name = ary[2];
            for (int i = 3; i < ary.Length; i++) {
        		OtherProps.Add(ary[i]);
            }
        }
        
        //子节点集合
        public List<MyTreeNode> ChildrenNodes = new List<MyTreeNode>();
        
        public override string ToString()
        {
        	return string.Format("{0}({1}),父ID{2},所在层级{3},下级层数{4},直接下级数{5},总下级会员数{6}",
        	                     Name, SysId, TopId, Level, ChildrenLevels, ChildrenCount, ChildrenCountAll);
        }
    }
}
