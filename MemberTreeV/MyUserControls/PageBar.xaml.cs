/*
 * 由SharpDevelop创建。
 * 用户： cjt
 * 日期: 2017/10/10
 * 时间: 21:29
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MemberTree
{
	/// <summary>
	/// Interaction logic for PageBar.xaml
	/// </summary>
	public partial class PageBar : UserControl
	{
		private int pageNo = 1;  // 当前页  
        private int pageSize = 50;  // 每页记录数  
        private int totalCount = 0;  // 总记录数
        private int pageCount = 1;  // 总页数
  
        public PageBar() 
        {
            InitializeComponent();
            this.prePageBtn.IsEnabled = false;  
        }
        
        public int PageSize {
            get {  
                return pageSize;  
            }  
            set {  
                if (value > 0) {  
                    pageSize = value;  
                }  
            }  
        } 
        /// <summary>
		/// 获取分页数据委托
		/// </summary>
		/// <param name="pageNo">页码数</param>
		/// <param name="pageSize">每页记录数</param>
        public delegate void GetDataDelegate(int pageNo, int pageSize);  
        private GetDataDelegate getDataDelegateHandler;  
        public PageBar.GetDataDelegate GetDataDelegateHandler {  
            set {  
                getDataDelegateHandler = value;  
            }  
        }
        
        public void Init(int totalCount)
        {
        	this.totalCount = totalCount;
        	if(totalCount % pageSize == 0)
        	{
        		pageCount = totalCount / pageSize;
        	}
        	else
        	{
            	pageCount = totalCount / pageSize + 1;  
        	}
        	if(pageCount == 1)
        	{
        		this.Visibility = Visibility.Collapsed;
        	}
        	else
        	{
        		this.Visibility = Visibility.Visible;
        	}
        	
        	GotoPage(1);
        }
        
        public void GotoPage(int pageNo) {
            if (pageNo <= 0) {  
                pageNo = 1;  
            }  
  
            this.pageNo = pageNo;  
  
            try {  
                getDataDelegateHandler(pageNo - 1, pageSize);
                // 页码显示  
                this.totalCountTbk.Text = totalCount.ToString();
                this.pageNoTbk.Text = pageNo.ToString();
                this.pageCountTbk.Text = pageCount.ToString();
  
                // 按钮状态  
                this.prePageBtn.IsEnabled = pageNo > 1;  
                this.firstPageBtn.IsEnabled = pageNo > 1;  
                this.nextPageBtn.IsEnabled = pageNo < pageCount;  
                this.lastPageBtn.IsEnabled = pageNo < pageCount;  
            } catch (Exception ex) {
                this.pageNoTbk.Text = "";  
                this.pageCountTbk.Text = "";  
            }  
        }
  
        // 首页事件     
        private void firstPageBtn_Click(object sender, RoutedEventArgs e) {  
            GotoPage(1); 
        }  
  
        // 上一页 
        private void prePageBtn_Click(object sender, RoutedEventArgs e) {  
            if (pageNo > 1) {  
                pageNo -= 1;  
                GotoPage(pageNo);  
            }  
        }  
  
        // 下一页   
        private void nextPageBtn_Click(object sender, RoutedEventArgs e) {  
            if (pageNo == 1 || pageNo < pageCount) {  
                pageNo += 1;  
                GotoPage(pageNo);  
            }  
        }  
  
        // 末页
        private void lastPageBtn_Click(object sender, RoutedEventArgs e) {  
            GotoPage(pageCount);  
        }  
  
        // 跳转到指定页 
        private void gotoBtn_Click(object sender, RoutedEventArgs e) {  
            try {  
                int pageNo = Convert.ToInt32(this.gotoPageNoTb.Text);  
                if (pageNo >= 1 && pageNo <= pageCount) {  
                    GotoPage(pageNo);  
                } else {  
                    MessageBox.Show("请输入正确的页码范围：1 ~ " + pageCount);  
                }  
            } catch (Exception ex) {
        		MessageBox.Show("输入的页码不正确！"+ex.Message);
            }  
        }
    }  
}