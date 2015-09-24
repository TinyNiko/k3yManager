using Microsoft.Win32;
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
    /// MyPywork.xaml 的交互逻辑
    /// </summary>
    public partial class MyPywork : Window
    {
        public MyPywork()
        {
            InitializeComponent();
        }

        private void pypath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "(*.*)|*.py";
            if (fileDialog.ShowDialog() == true)
            {
                string FilePath = fileDialog.FileName;
                Srctext.Text = FilePath;
            }

        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            if(Srctext.Text == "")
            {
                MessageBox.Show("path is empty");
                return; 
            }
            Pyworks pw = new Pyworks(Srctext.Text);
            string result =  pw.work();
            Destext.Text = result; 
        }
    }
}
