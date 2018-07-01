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
    	public string SysId;
    	public string TopId;
    	public int Level;
        
    	//直接下属孩子节点数量
    	public int SubCount;
        //所有的后代子孙节点数量
        public int ChildrenCount;
        //所有的后代子孙最深级别数
        public int ChildrenLevels;
        
        //子节点集合
//        public List<MyTreeNode> ChildrenNodes = new List<MyTreeNode>();
        
        public MyTreeNode()
        {
        }
        
        public MyTreeNode(string sysId, string topId)
        {
            this.SysId = sysId;
            //如果sysId = topId，为避免误读为闭环，将topId设置为空
            if(sysId == topId)
            {
            	 this.TopId = "";
            }
            else
            {
            	 this.TopId = topId;
            }
           
            this.Level = 0;
 
            switch (TextUtil.enUpperLower) 
            {
        		case EnumUpperLower.Lower:
        			SysId = SysId.ToLower();
               		TopId = TopId.ToLower();
        			break;
        		case EnumUpperLower.Upper:
        			SysId = SysId.ToUpper();
                	TopId = TopId.ToUpper();
        			break;
        		default:
        			break;
            }
           
            switch (TextUtil.enTrim)
            {
            	case EnumTrim.All:
            		SysId = SysId.Trim();
            		TopId = TopId.Trim();
            		break;
            	case EnumTrim.Start:
            		SysId = SysId.TrimStart();
            		TopId = TopId.TrimStart();
            		break;
            	case EnumTrim.End:
            		SysId = SysId.TrimEnd();
            		TopId = TopId.TrimEnd();
            		break;
            	default:
            		break;
            }
           
            switch (TextUtil.enDBCSBC)
            {
            	case EnumDBCSBC.DBC:
            		SysId = TextUtil.SBCToDBC(SysId);
                	TopId = TextUtil.SBCToDBC(TopId);
            		break;
            	case EnumDBCSBC.SBC:
            		SysId = TextUtil.DBCToSBC(SysId);
                	TopId = TextUtil.DBCToSBC(TopId);
                	break;
                default:
                	break;
            }
        }
    }
}
