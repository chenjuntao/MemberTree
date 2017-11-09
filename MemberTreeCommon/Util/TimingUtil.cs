/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/9/4
 * 时间: 13:35
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace MemberTree
{
	/// <summary>
	/// Description of TimingUtil.
	/// </summary>
	public class TimingUtil
	{
		private static int startTickCount;
    	public static void StartTiming()
    	{
    		startTickCount = Environment.TickCount;
    	}
    	public static string EndTiming()
    	{
    		string usedTime = "总共用时";
    		int usedTickCount = Environment.TickCount - startTickCount;
    		
    		if(usedTickCount < 1000)
    		{
    			usedTime += usedTickCount + "毫秒";
    		}else 
    		{
    			usedTickCount = usedTickCount/1000;
    			if(usedTickCount < 60)
    			{
    				usedTime += usedTickCount + "秒";
    			}
    			else if(usedTickCount < 3600)
    			{
    				int usedMinute = usedTickCount/60;
    				usedTickCount = usedTickCount%60;
    				usedTime += usedMinute + "分钟" + usedTickCount + "秒";
    			}
    			else
    			{
    				int usedHour = usedTickCount/3600;
    				int usedMinute = (usedTickCount%3600)/60;
    				usedTickCount = usedTickCount%60;
    				usedTime += usedHour + "小时" + usedMinute + "分钟" + usedTickCount + "秒";
    			}
    		}
		
    		return usedTime;
    	} 
	}
}
