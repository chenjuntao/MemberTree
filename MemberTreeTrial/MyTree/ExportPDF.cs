/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/8/16
 * 时间: 16:06
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MemberTree
{
	/// <summary>
	/// Description of ExportPDF.
	/// </summary>
	public class ExportPDF
	{
		private const string TMP_DIR = "temp";
		
		public static void Export2PDF(INotify notify, MyTreeView mytreeview, MyTreeNode node)
		{
			Directory.CreateDirectory(TMP_DIR);
			ExportAllImgs(notify, mytreeview, node);
			
			//定义一个Document，并设置页面大小为A4，竖向 
//			Document doc = new Document(PageSize.A4);
			Document doc = new Document();
			PdfWriter.GetInstance(doc, new FileStream(node.ToString()+".pdf", FileMode.Create));
            
            //设置PDF的头信息，一些属性设置，在Document.Open 之前完成
	        doc.AddAuthor("TomChen");
	        doc.AddCreationDate();
	        doc.AddCreator("湖南警察学院");
	        doc.AddSubject("将选中的会员树导出为PDF格式，如果数据量大，则导出为多张PDF页面");
	        doc.AddTitle("将选中的会员树导出为PDF");
	        doc.AddKeywords("会员树,会员层级,PDF");
	        //自定义头 
	        //doc.AddHeader("Expires", "0");
	        
	        doc.Open();
	        
	        string[] imgfiles = Directory.GetFiles(TMP_DIR, "*.png");
	        
	        //首页
	        //写入文字
	        Paragraph paragraph = new Paragraph("ID:" + node.SysId, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("TopID:" + node.TopId, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("Level:" + node.Level, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("Sub1:" + node.ChildrenNodes.Count, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("SubLevel:" + node.ChildrenLevels, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("SubAll:" + node.ChildrenCount, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("----------------------------------------", new Font(Font.FontFamily.COURIER, 20, 0, BaseColor.GREEN));
	        doc.Add(paragraph);
	        paragraph = new Paragraph("Export pictures count:" + imgfiles.Length, new Font(Font.FontFamily.COURIER, 20, 0, BaseColor.GREEN));
	        doc.Add(paragraph);
	        
	        notify.SetProcessBarVisible(true);
	        
	        for (int i = 0; i < imgfiles.Length; i++) 
	        {
	            //写入图片
	            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgfiles[i]);
	            doc.SetPageSize(new Rectangle(img.Width, img.Height));
	        	//新建一页
	        	doc.NewPage();
	            //img.ScaleAbsoluteWidth(PageSize.A4.Width);
	            //img.ScaleAbsoluteHeight(PageSize.A4.Height - 100);
	            //img.SetAbsolutePosition((PageSize.POSTCARD.Width - img.ScaledWidth) / 2, (PageSize.POSTCARD.Height - img.ScaledHeight) / 2);
				doc.Add(img);
				notify.SetStatusMessage("正在生成PDF文件"+ i + "/" + imgfiles.Length);
				notify.SetProcessBarValue(i * 100.0 /imgfiles.Length);
	        }

            doc.Close();
            Directory.Delete(TMP_DIR, true);
            notify.SetProcessBarVisible(false);
            notify.SetStatusMessage("导出PDF完成！");
            MessageBox.Show("导出PDF完成！");
		}
		
		//导出过程中未打开的子节点的集合
		private static List<MyTreeNode> exportNodes = new List<MyTreeNode>();
		private static void ExportAllImgs(INotify notify, MyTreeView mytreeview, MyTreeNode node)
		{
			mytreeview.BeginExportImg();
			exportNodes.Clear();
			exportNodes.Add(node);
			while(exportNodes.Count>0)
			{
				ExportImg(notify, mytreeview, exportNodes[0]);
			}
			mytreeview.EndExportImg();
		}
		
		private static void ExportImg(INotify notify, MyTreeView mytreeview, MyTreeNode node)
		{
			mytreeview.SetRootNode(node);
			//如果全部子节点数量小于50，则直接全部打开
			if(node.ChildrenCount<50)
			{
				mytreeview.ExpandRootNode(node.ChildrenLevels);
			}
			else
			{
				//已打开的节点数量
				int expNodeCount = node.ChildrenNodes.Count;
				
				//这个循环用于防止出现大量的上下级只有一个子节点的情况导致打开节点数量过少
				TreeViewItem rootItem = mytreeview.memberTreeView.Items[0] as TreeViewItem;
				while(rootItem.Items.Count == 1)
				{
					mytreeview.ExpandNode(rootItem, 1);
					rootItem = rootItem.Items[0] as TreeViewItem;
					expNodeCount++;
				}
				
				//打开一级子节点，再逐个判断数量
				mytreeview.ExpandNode(rootItem, 1);

				foreach (TreeViewItem subItem in rootItem.Items)
				{
					MyTreeNode subNode = subItem.Tag as MyTreeNode;
					
					if(subNode.ChildrenCount < 10 || subNode.ChildrenCount + expNodeCount < 50)
					{//如果全部子节点数量小于10，或者全部子节点数量加上已打开的节点数量小于50，则该子节点全部打开
						mytreeview.ExpandNode(subItem, subNode.ChildrenLevels);
						expNodeCount += subNode.ChildrenCount;
					}
					else if(subNode.ChildrenNodes.Count + expNodeCount < 50)
					{//如果则该节点一级子节点数量加上已打开的节点数量小于50，则该子节点打开子一级节点
						mytreeview.ExpandNode(subItem, 1);
						expNodeCount += subNode.ChildrenNodes.Count;
						//遍历孙子节点
						foreach (TreeViewItem grandItem in subItem.Items)
						{
							MyTreeNode grandNode = grandItem.Tag as MyTreeNode;
							if(grandNode.ChildrenCount < 10 || grandNode.ChildrenCount + expNodeCount < 50)
							{
								mytreeview.ExpandNode(grandItem, grandNode.ChildrenLevels);
								expNodeCount += grandNode.ChildrenCount;
							}
							else
							{
								exportNodes.Add(grandNode);
							}
						}
					}
					else
					{//如果全部子节点数量加上已打开的节点数量大于50，则该子节点不打开
						exportNodes.Add(subNode);
					}
				}
			}
			exportNodes.Remove(node);
			notify.SetStatusMessage("正在导出图片"+ node.SysId);
			SaveImage(mytreeview, node.SysId);
		}
		
		private static void SaveImage(MyTreeView mytreeview, string imgName)
		{
			FileStream fs = new FileStream(TMP_DIR + "/" + imgName + ".png", FileMode.Create);
            int width = (int)mytreeview.memberTreeView.ActualWidth;
            int height = (int)mytreeview.memberTreeView.ActualHeight;
            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            bmp.Render(mytreeview.memberTreeView);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
            fs.Dispose();
		}
	}
}
