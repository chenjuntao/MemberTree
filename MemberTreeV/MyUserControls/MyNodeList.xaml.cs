using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemberTree
{
    /// <summary>
    /// MyNodeList.xaml 的交互逻辑
    /// </summary>
    public partial class MyNodeList : UserControl
    {
        public MyNodeList()
        {
            InitializeComponent();
        }
        
        public void InitCols()
        {
        	List<string> tableOptCols = MyTrees.GetTableOptCols();
        	foreach (string optCol in tableOptCols) {
        		GridViewColumn col = new GridViewColumn();
	        	col.Header = optCol;
	        	col.DisplayMemberBinding = new Binding(optCol);
	        	gridView.Columns.Add(col);
        	}
        }
    }
}
