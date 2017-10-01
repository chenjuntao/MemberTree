/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 10/01/2017
 * 时间: 10:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MemberTree
{
	/// <summary>
	/// Description of AutoUpdateVer.
	/// </summary>
	public class AutoUpdateVer
	{
		public string Version="0";
		public string VerInfo="";
		public string Url = "https://github.com/chenjuntao/MemberTree/raw/master/DLL/";
		//private string Url = "https://coding.net/u/cjtlp2006/p/TemberTreeDLL/git/raw/master/";
		public Dictionary<string, string> DLLs = new Dictionary<string, string>();
		
		public static AutoUpdateVer ReadVer()
		{
			AutoUpdateVer autoUpVer = new AutoUpdateVer();
			
			if(!Directory.Exists("dll"))
			{
				Directory.CreateDirectory("dll");
			}
			if(File.Exists("dll/ver.dll"))
			{
	     		XDocument document = XDocument.Load("dll/ver.dll");
	     		XElement root= document.Root;
	     		XElement vers = root.Element("vers");
	     		autoUpVer.Version = vers.Attribute("ver").Value;
	     		autoUpVer.Url = vers.Attribute("url").Value;
	     		autoUpVer.VerInfo = vers.Value;
	     		autoUpVer.DLLs = new Dictionary<string, string>();
	     		foreach (XElement ele in root.Element("dlls").Elements())
	     		{
	     			autoUpVer.DLLs.Add(ele.Name.ToString(), ele.Attribute("ver").Value);
	     		}
			}
     		
     		return autoUpVer;
		}
	}
}
