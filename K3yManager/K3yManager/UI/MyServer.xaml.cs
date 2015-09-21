using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using K3yManager.Works;
namespace K3yManager.UI
{
    /// <summary>
    /// MyServer.xaml 的交互逻辑
    /// </summary>
    public partial class MyServer : Window
    {
        public MyServer()
        {
            InitializeComponent();
        }

        private void bn_Ping_Click(object sender, RoutedEventArgs e)
        {
            Destext.Text = ""; 
            ServerWork sw = new ServerWork();
            string result = sw.MyPing();
            Destext.Text += result; 
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
