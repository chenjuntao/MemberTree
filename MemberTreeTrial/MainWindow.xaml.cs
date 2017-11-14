using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MemberTree
{
    public interface INotify
    {
        void SetProcessBarVisible(bool visible);
        void SetProcessBarValue(double step);
        void SetStatusMessage(string message);
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotify
    {
        internal static INotify notify = null;

        private List<MyTreeNode> findResultNodes = new List<MyTreeNode>();

        public MainWindow()
        {
            InitializeComponent();
            notify = this;
            commonView.mainWindow = this;
        }

        #region 左侧按钮事件
        //直接连接数据库
        private void ButtonConnectDB_Click(object sender, RoutedEventArgs e)
        {
        	MessageBox.Show("直接连接数据库，可以将数据库中的数据表作为数据源进行计算和导入，\n目前支持Mysql和Sqlserver。\n但当前版本为试用版，该功能无法使用，请使用正式版!");
        }
        //检查csv文件合法性
        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
        	MessageBox.Show("可以检查作为数据源的csv文件中是否存在数据错误，并提示用户修改保存。\n但当前版本为试用版，该功能无法使用，请使用正式版!");
        }
        
        //打开文件
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
        	string file = "SampleData\\sampledata1.csv";
        	if (File.Exists(file))
            {
            	homeView.Visibility = Visibility.Collapsed;
            	progressView.SetCsvFile(file);
            	
                int upperLower = comboToLower.SelectedIndex;
                int DBCSBC = comboToHalf.SelectedIndex;
                int trim = comboTrim.SelectedIndex;
                MyTrees.OpenCSVFile(file);
                
                tabView.SelectedItem = tabHome;
                
                myTreeView.SetRootNode(MyTrees.RootNode);
                
                //myGraphView.InitMyTree();
                //显示统计信息
                commonView.SetSummary(file, 
                                      MyTrees.ForestNodeCount,
                                      MyTrees.IdConflictNodes.Count,
                                      MyTrees.RingNodes.Count,
                                      MyTrees.IdNullNodes.Count,
                                      MyTrees.AllNodeCount);
                tabNoParent.Header = "森林（一共" + MyTrees.NoParentNodes.Count + "棵树）";
                tabIdConflict.Header = "Id有重复（" + MyTrees.IdConflictNodes.Count + "个）";
                tabIdNull.Header = "信息不完整（" + MyTrees.IdNullNodes.Count + "个）";
                tabRingErr.Header = "形成闭环（" + MyTrees.RingNodes.Count + "个）";
            }
        	else
        	{
        		MessageBox.Show("样例数据不存在！");
        	}
        }
        

        //查找
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
			string searchsql = mySearchFilter.SearchSql();
            listNodes.NodeListView.ItemsSource = MyTrees.FindBySql(searchsql);
            tabView.SelectedItem = tabFindResult;
        }

        #region 导出表格
                
        //导出选中的树节点
        private void ButtonExportSelectedNode_Click(object sender, RoutedEventArgs e)
        {
        	MyTreeNode node = myTreeView.GetSelectedNode();
        	if (node != null)
            {
        		ExportCSV.ExportNodes(new List<MyTreeNode>{node});
         	}
            else
            {
                MessageBox.Show("必须选中一个节点！");
            }
        }
        
        //导出全部森林中的树
        private void ButtonExportTrees_Click(object sender, RoutedEventArgs e)
        {
        	if(MyTrees.NoParentNodes.Count > 0)
        	{
        		ExportCSV.ExportNodes(MyTrees.NoParentNodes);
        	}
        	else
            {
                MessageBox.Show("森林中没有树！");
            }
        }
        
		//导出信息不完整的节点
        private void ButtonExportInfoErrNodes_Click(object sender, RoutedEventArgs e)
        {
            if(MyTrees.IdNullNodes.Count > 0)
        	{
        		ExportCSV.ExportNodes(MyTrees.IdNullNodes);
        	}
        	else
            {
                MessageBox.Show("没有信息不完整的节点！");
            }
        }

		//导出全部闭环中的节点
        private void ButtonExportRingNodes_Click(object sender, RoutedEventArgs e)
        {
            if(MyTrees.RingNodes.Count > 0)
        	{
        		ExportCSV.ExportNodes(MyTrees.RingNodes.Values.ToList());
        	}
        	else
            {
                MessageBox.Show("没有形成闭环的节点！");
            }
        }        
            	
        #endregion

        //导出图片
        private void btnExportImg_Click(object sender, RoutedEventArgs e)
        {
        	MyTreeNode node = myTreeView.GetSelectedNode();
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
	                FileStream fs = new FileStream(saveFileDlg.FileName, FileMode.Create);
	                int width = (int)myTreeView.memberTreeView.ActualWidth;
	                int height = (int)myTreeView.memberTreeView.ActualHeight;
	                RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
	                bmp.Render(myTreeView.memberTreeView);
	                BitmapEncoder encoder = new PngBitmapEncoder();
	                encoder.Frames.Add(BitmapFrame.Create(bmp));
	                encoder.Save(fs);
	                fs.Close();
	                fs.Dispose();
	            }
        	}
        }
        
        //导出PDF
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            MyTreeNode node = myTreeView.GetSelectedNode();
        	if(node == null)
        	{
        		  MessageBox.Show("必须选中一个节点！");
        	}
        	else
        	{
        		ExportPDF.Export2PDF(this ,myTreeView, node);
            }
        }

        //打印
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.UserPageRangeEnabled = true;

            if (printDlg.ShowDialog() == true)
            {
                 printDlg.PrintVisual(myTreeView.memberTreeView, "打印当前会员树视图");
            }
        }
        
        //切换树状视图的显示风格
        private void TreeStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myTreeView != null)
            {
                string selectedContent = ((ComboBoxItem)comboTreeStyle.SelectedItem).Content.ToString();
                switch (selectedContent)
                {
                    case "最简风格树视图":
                        myTreeView.memberTreeView.Resources.Clear();
                        break;
                    case "带连接线树视图":
                        myTreeView.memberTreeView.Resources.Source = new Uri("LineTreeStyle.xaml", UriKind.RelativeOrAbsolute);
                        break;
                    case "水平组织结构树视图":
                        myTreeView.memberTreeView.Resources.Source = new Uri("GraphTreeStyle.xaml", UriKind.RelativeOrAbsolute);
                        break;
                    case "垂直组织结构树视图":
                        myTreeView.memberTreeView.Resources.Source = new Uri("GraphTreeVStyle.xaml", UriKind.RelativeOrAbsolute);
                        break;
                    case "盒状风格树视图":
                        myTreeView.memberTreeView.Resources.Source = new Uri("BoxTreeStyle.xaml", UriKind.RelativeOrAbsolute);
                        break;
                    default:
                        break;
                }
            }
        }

        //切换视图
        private void TabView_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (tabView.SelectedItem == tabHome)
            {
            	commonView.Visibility = Visibility.Visible;
                myTreeView.Visibility = Visibility.Collapsed;
                listNodes.Visibility = Visibility.Collapsed;
            }
            else  if (tabView.SelectedItem == tabTree)
            {
                myTreeView.Visibility = Visibility.Visible;
                commonView.Visibility = Visibility.Collapsed;
                listNodes.Visibility = Visibility.Collapsed;
            }
            else
            {
                listNodes.Visibility = Visibility.Visible;
                commonView.Visibility = Visibility.Collapsed;
                myTreeView.Visibility = Visibility.Collapsed;
                if (tabView.SelectedItem == tabFindResult)
                {
                    listNodes.NodeListView.ItemsSource = findResultNodes;
                }
                else if (tabView.SelectedItem == tabIdConflict)
                {
                    listNodes.NodeListView.ItemsSource = MyTrees.IdConflictNodes;
                }
                else if (tabView.SelectedItem == tabNoParent)
                {
                    listNodes.NodeListView.ItemsSource = MyTrees.NoParentNodes;
                }
                else if (tabView.SelectedItem == tabIdNull)
                {
                    listNodes.NodeListView.ItemsSource = MyTrees.IdNullNodes;
                }
                else if (tabView.SelectedItem == tabRingErr)
                {
                    listNodes.NodeListView.ItemsSource = MyTrees.RingNodes.Values;
                }
            }
        }
        
        //用户双击选中进入查看某个查找结果
        private void NodeListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView currentList = (sender as MyNodeList).NodeListView;
            MyTreeNode selectedNode = currentList.SelectedItem as MyTreeNode;
            if (selectedNode != null)
            {
                myTreeView.SetRootNode(selectedNode);
                //myGraphView.AddFindedNode(selectedNode);
                tabView.SelectedItem = tabTree;

                int maxLevel = int.Parse((comboOpenLevel.SelectedItem as ComboBoxItem).Tag.ToString());
                myTreeView.ExpandRootNode(maxLevel);//打开所有节点
            }
        }
        #endregion
       
        #region INotify接口
        
        //设置进度条是否显示
        private delegate void ProcessBarVisibleDelegate(bool visible);
        private ProcessBarVisibleDelegate processBarVisibleDelegate = null;
        public void SetProcessBarVisible(bool visible)
        {
            if (processBarVisibleDelegate == null)
            {
                processBarVisibleDelegate = new ProcessBarVisibleDelegate(SetProcessBarVisibleImp);
            }
            this.Dispatcher.Invoke(processBarVisibleDelegate, visible);
            DoEvents();
        }
        private void SetProcessBarVisibleImp(bool visible)
        {
            progressBar.Value = 0;
            progressBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            progressView.ReSetProgressValue();
            progressView.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            statusView.Visibility = visible ? Visibility.Collapsed : Visibility.Visible;
            mainGrid.Visibility = visible ? Visibility.Collapsed : Visibility.Visible;
            leftMenu.IsEnabled = !visible;
            Cursor = visible ? Cursors.Wait : Cursors.Arrow;
        }

        //设置进度条进度(0-100)
        private delegate void ProcessBarValueDelegate(double step);
        private ProcessBarValueDelegate processBarValueDelegate = null;
        public void SetProcessBarValue(double step)
        {
            if (processBarValueDelegate == null)
            {
                processBarValueDelegate = new ProcessBarValueDelegate(SetProcessBarValueImp);
            }
            this.Dispatcher.Invoke(processBarValueDelegate, step);
            DoEvents();
        }
        private void SetProcessBarValueImp(double step)
        {
            this.progressBar.Value = step;
            this.progressView.SetProgressValue(step);
        }

        //设置状态栏提示文本
        private delegate void ShowTextDelegate(string message);
        private ShowTextDelegate showTextDelegate = null;
        public void SetStatusMessage(string message)
        {
            if (showTextDelegate == null)
            {
                showTextDelegate = new ShowTextDelegate(SetStatusMessageImp);
            }
            this.Dispatcher.Invoke(showTextDelegate, message);
            DoEvents();
        }
        private void SetStatusMessageImp(string message)
        {
            this.statusMessage.Text = message;
            this.progressView.SetWaitting();
        }

        private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate(object f)
                {
                    (f as DispatcherFrame).Continue = false;

                    return null;
                }
            ), frame);
            Dispatcher.PushFrame(frame);
        }
		
        #endregion 
    }

}
