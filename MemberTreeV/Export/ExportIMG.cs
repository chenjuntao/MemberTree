/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 09/16/2017
 * 时间: 18:59
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Description of ExportIMG.
	/// </summary>
	public class ExportIMG
	{		
		internal static void SaveNode2Image(MyTreeView mytreeview, MyTreeNode node)
		{
			if(node == null)
        	{
        		  MessageBox.Show("必须选中一个节点！");
        	}
        	else
        	{
		        SaveFileDialog saveFileDlg = new SaveFileDialog();
	            saveFileDlg.Title = "选择将会员树导出为文件的位置";
	            saveFileDlg.Filter = "png格式|*.png";
	            saveFileDlg.FileName = node.ToString();
	            if (saveFileDlg.ShowDialog() == true)
	            {
	                SaveImage(mytreeview.memberTreeView, saveFileDlg.FileName);
	            }
        	}
		}
		
		internal static void SaveImage(FrameworkElement frmEle, string imgFile)
		{
			FileStream fs = new FileStream(imgFile, FileMode.Create);
            int width = (int)frmEle.ActualWidth;
            int height = (int)frmEle.ActualHeight;
            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            bmp.Render(frmEle);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
            fs.Dispose();
		}
	}
}
