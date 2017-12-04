/*
 * 由SharpDevelop创建。
 * 用户： TomChen
 * 日期: 2017/11/30
 * 时间: 11:12
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
using MemberTree;
using Microsoft.Win32;
using RSACommon;

namespace SoftRegister
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private string publicKey ="AwEAAdRuUg26vNnnSLp1JRPsDWQWjr/S+fX67Blv2Er5XiIuksPWbBq9L7WpcPN2yiiQdlOlqhLgMigKaDaHwRp2ob8y2aCCja1Vi3nZymFK23h9wWwdfPuV0vfnuQ74EcF7K6vOTw6iOcaOUTvDe3tZuS9raCgdfaLrPKzwotc0Jn31";
		private string privateKey = "gBwg95CF15fq/kBiXqSCr0s/iWtxHlQqA7Vij/tthb90904jSHFJ99VQOHqkkiRI7MIqv5h8Q2f16NK/qxw79TLEJZOYqH6l+EUzS/kOvGKDJ1zUEZqhNCye+J7X4/hfPCSy9fGIkDXfSkAiYPZpDM9QPA6Drj7VcL3jvfA2ZwsJ1G5SDbq82edIunUlE+wNZBaOv9L59frsGW/YSvleIi6Sw9ZsGr0vtalw83bKKJB2U6WqEuAyKApoNofBGnahvzLZoIKNrVWLednKYUrbeH3BbB18+5XS9+e5DvgRwXsrq85PDqI5xo5RO8N7e1m5L2toKB19ous8rPCi1zQmffU=";
		public Window1()
		{
			InitializeComponent();
		}
		
		void BtnBrowser_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openfileDlg = new OpenFileDialog();
            openfileDlg.Title = "选择需要进行注册的用户文件(*.reginfo)";
            openfileDlg.Filter = "reginfo文件|*.reginfo文件";
            if (openfileDlg.ShowDialog() == true)
            {
            	
            	string regMsg = EncryptHelper.FileDecrypt("reg.dll");
        		string[] regList = regMsg.Split(new String[]{getCpu}, StringSplitOptions.None);
            }
		}
		
		void BtnOK_Click(object sender, RoutedEventArgs e)
		{
			

		}
	}
}