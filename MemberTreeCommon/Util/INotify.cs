/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 08/28/2017
 * 时间: 16:13
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace MemberTree
{
	public interface INotify
    {
        void SetProcessBarVisible(bool visible);
        void SetProcessBarValue(double step);
        void SetStatusMessage(string message);
    }
}
