using K3yManager.Works;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace K3yManager.UI
{
    /// <summary>
    /// FileScan.xaml 的交互逻辑
    /// </summary>
    public partial class FileScan : Window
    {

        private string pypath = "G:\\code\\windows\\K3yManager\\K3yManager\\pyscript\\filescan.py  ";
        private string[] argpath = { "G:\\code\\android\\modules", "G:\\code\\cpp\\modules", "G:\\code\\java\\modules",
                                    "G:\\code\\python\\modules","G:\\code\\windows\\modules","G:\\code\\shellcode"};

        private string[] pathname = { "android", "cpp", "java", "python", "windows" ,"shellcode"};

        public FileScan() 
        {
            InitializeComponent();
            RefreshTree(); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TreeView.Items.Clear(); 
            RefreshTree(); 
        }


        private void RefreshTree()
        {
            Pyworks pw = new Pyworks();
            for(int i = 0 ; i < argpath.Length; i++)
            {
                string para = pypath + argpath[i];
               
                string filename = pw.work(para);
               

                AddTreeNode(i, filename); 
            }
        }

        private void AddTreeNode(int place , string filename)
        {
            TreeViewItem item = new TreeViewItem() { Header = pathname[place] };
            string[] items = filename.Replace("\r", "").Split('\n');
            for(int i =0; i <items.Length-1; i++)
            {
                item.Items.Add(items[i]); 
            }
            TreeView.Items.Add(item);
            
        }


        private void select(object sender, RoutedEventArgs e)
        {

            string cc = TreeView.SelectedItem.ToString();
            foreach (var xx in argpath)
            {

                string filepath = xx + "\\" + cc;
                if (File.Exists(filepath))
                {
                    showfile(filepath);
                    break;
                }

            }


        }

        private void showfile(string path)
        {
            filetext.Text = ""; 
            StreamReader sr = new StreamReader(path);
            filetext.Text = sr.ReadToEnd();
            sr.Close();  

        }

   
    }
}
