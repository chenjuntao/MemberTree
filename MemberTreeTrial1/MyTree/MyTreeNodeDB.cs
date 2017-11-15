/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 2017/8/6
 * 时间: 9:07
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
 
using System;
using System.Collections.Generic;

namespace MemberTree
{
	public class MyTreeNodeDB
    {
        public string SysId { get; set; }
        public string TopId { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string IdCard { get; set; }
		public string Tel { get; set; }
		public string Email { get; set; }
		public string Bank { get; set; }
		public string BankCard { get; set; }
		public string Addr { get; set; }
		
		public string ywczyze { get; set; }
		public string ywcszze { get; set; }
		public string ywczycs { get; set; }
		public string ywcszcs { get; set; }
		public string yzcszz { get; set; }
		public string yzcsxb { get; set; }
		public string yzcsjb { get; set; }
		public string cjqbyzc { get; set; }
		public string glqbyzc { get; set; }
		public string syszz { get; set; }
		public string sysxb { get; set; }
		public string sysjb { get; set; }
		public string cjqbsy { get; set; }
		public string glqbsy { get; set; }
		
		public MyTreeNodeDB()
		{}
		
		public MyTreeNodeDB(string[] node)
		{
			SysId = node[0];
			TopId = node[1];
			Name = node[2];
			Account = node[3];
			IdCard = node[4];
			Tel = node[5];
			Email = node[6];
			Bank = node[7];
			BankCard = node[8];
			Addr = node[9];
			
			ywczyze = node[10];
			ywcszze = node[11];
			ywczycs = node[12];
			ywcszcs = node[13];
			yzcszz = node[14];
			yzcsxb = node[15];
			yzcsjb = node[16];
			cjqbyzc = node[17];
			glqbyzc = node[18];
			syszz = node[19];
			sysxb = node[20];
			sysjb = node[21];
			cjqbsy  = node[22];
			glqbsy  = node[23];
		}
		
		public static bool Check(string[] node)
		{
			if(node[0].Length>11)
				return false;
			if(node[1].Length>11)
				return false;
			if(node[2].Length>20)
				return false;
			if(node[3].Length>20)
				return false;
			if(node[4].Length>18)
				return false;
			if(node[5].Length>11)
				return false;
			if(node[6].Length>25)
				return false;
			if(node[7].Length>60)
				return false;
			if(node[8].Length>20)
				return false;
			if(node[9].Length>100)
				return false;
			
			if(node[10].Length>33)
				return false;
			if(node[11].Length>33)
				return false;
			if(node[12].Length>21)
				return false;
			if(node[13].Length>21)
				return false;
			if(node[14].Length>65)
				return false;
			if(node[15].Length>65)
				return false;
			if(node[16].Length>65)
				return false;
			if(node[17].Length>65)
				return false;
			if(node[18].Length>65)
				return false;
			if(node[19].Length>11)
				return false;
			if(node[20].Length>11)
				return false;
			if(node[21].Length>11)
				return false;
			if(node[22].Length>20)
				return false;
			if(node[23].Length>11)
				return false;
			
			return true;
		}
    }
}