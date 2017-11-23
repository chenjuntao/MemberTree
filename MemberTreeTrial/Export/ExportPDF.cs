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
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace MemberTree
{
	/// <summary>
	/// Description of ExportPDF.
	/// </summary>
	public class ExportPDF
	{
		private const string TMP_DIR = "temp";
		
		public static void Export2PDF(MyTreeView mytreeview, MyTreeNode node)
		{
			if(node == null)
        	{
        		  MessageBox.Show("必须选中一个节点！");
        		  return;
        	}
			// disable once SuggestUseVarKeywordEvident
			SaveFileDialog openfileDlg = new SaveFileDialog();
		    openfileDlg.Title = "选择将会员树导出为pdf文件的位置";
		    openfileDlg.Filter = "pdf文件|*.pdf";
		    openfileDlg.FileName = node.ToString();
		    if (openfileDlg.ShowDialog() == true)
		    {
		    	TimingUtil.StartTiming();
				Directory.CreateDirectory(TMP_DIR);
				ExportAllImgs(mytreeview, node);
				
				//定义一个Document，并设置页面大小为A4，竖向 
		//			Document doc = new Document(PageSize.A4);
				Document doc = new Document();
				PdfWriter.GetInstance(doc, new FileStream(openfileDlg.FileName, FileMode.Create));
		        
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
		        paragraph = new Paragraph("Sub1:" + node.ChildrenCount, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
		        doc.Add(paragraph);
		        paragraph = new Paragraph("SubLevel:" + node.ChildrenLevels, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
		        doc.Add(paragraph);
		        paragraph = new Paragraph("SubAll:" + node.ChildrenCount, new Font(Font.FontFamily.TIMES_ROMAN, 30, 0, BaseColor.BLUE));
		        doc.Add(paragraph);
		        paragraph = new Paragraph("----------------------------------------", new Font(Font.FontFamily.COURIER, 20, 0, BaseColor.GREEN));
		        doc.Add(paragraph);
		        paragraph = new Paragraph("Export pictures count:" + imgfiles.Length, new Font(Font.FontFamily.COURIER, 20, 0, BaseColor.GREEN));
		        doc.Add(paragraph);
		        
		        WindowView.notify.SetProcessBarVisible(true);
		        
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
					WindowView.notify.SetStatusMessage("正在生成PDF文件"+ i + "/" + imgfiles.Length);
					WindowView.notify.SetProcessBarValue(i * 100.0 /imgfiles.Length);
		        }
		
		        doc.Close();
		        Directory.Delete(TMP_DIR, true);
		        WindowView.notify.SetProcessBarVisible(false);
		        WindowView.notify.SetStatusMessage(TimingUtil.EndTiming());
		        MessageBox.Show("导出PDF完成！");
            }
		}
		
		//导出过程中未打开的子节点的集合
		private static List<MyTreeNode> exportNodes = new List<MyTreeNode>();
		private static void ExportAllImgs(MyTreeView mytreeview, MyTreeNode node)
		{
			mytreeview.BeginExportImg();
			exportNodes.Clear();
			exportNodes.Add(node);
			while(exportNodes.Count>0)
			{
				ExportImg(mytreeview, exportNodes[0]);
			}
			mytreeview.EndExportImg();
		}
		
		private static void ExportImg(MyTreeView mytreeview, MyTreeNode node)
		{
			mytreeview.SetRootNode(node);
			//如果全部子节点数量小于50，则直接全部打开
			if(node.ChildrenCountAll<50)
			{
				mytreeview.ExpandRootNode(node.ChildrenLevels);
			}
			else
			{
				//已打开的节点数量
				int expNodeCount = node.ChildrenCount;
				
				//这个循环用于防止出现大量的上下级只有一个子节点的情况导致打开节点数量过少
				TreeViewItem rootItem = mytreeview.memberTreeView.Items[0] as TreeViewItem;
				MyTreeNode rootNode = rootItem.Tag as MyTreeNode;
				while(rootNode.ChildrenCount == 1)
				{
					mytreeview.ExpandNode(rootItem, 1);
					rootItem = rootItem.Items[0] as TreeViewItem;
					rootNode = rootItem.Tag as MyTreeNode;
					expNodeCount++;
				}
				
				//打开一级子节点，再逐个判断数量
				mytreeview.ExpandNode(rootItem, 1);

				foreach (TreeViewItem subItem in rootItem.Items)
				{
					MyTreeNode subNode = subItem.Tag as MyTreeNode;
					if(subNode.ChildrenCountAll >0)
					{
						if(subNode.ChildrenCountAll < 10 || subNode.ChildrenCountAll + expNodeCount < 50)
						{//如果全部子节点数量小于10，或者全部子节点数量加上已打开的节点数量小于50，则该子节点全部打开
							mytreeview.ExpandNode(subItem, subNode.ChildrenLevels);
							expNodeCount += subNode.ChildrenCountAll;
						}
						else if(subNode.ChildrenCount + expNodeCount < 50)
						{//如果则该节点一级子节点数量加上已打开的节点数量小于50，则该子节点打开子一级节点
							mytreeview.ExpandNode(subItem, 1);
							expNodeCount += subNode.ChildrenCount;
							//遍历孙子节点
							foreach (TreeViewItem grandItem in subItem.Items)
							{
								MyTreeNode grandNode = grandItem.Tag as MyTreeNode;
								if(grandNode.ChildrenCountAll < 10 || grandNode.ChildrenCountAll + expNodeCount < 50)
								{
									mytreeview.ExpandNode(grandItem, grandNode.ChildrenLevels);
									expNodeCount += grandNode.ChildrenCountAll;
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
			}
			exportNodes.Remove(node);
			WindowView.notify.SetStatusMessage("正在导出图片"+ node.SysId);
			ExportIMG.SaveImage(mytreeview, TMP_DIR + "/" + node.SysId + ".png");
		}
	}
}
