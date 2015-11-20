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
using Microsoft.Win32;
using System.Threading;

namespace K3yManager.UI
{
    /// <summary>
    /// MyServer.xaml 的交互逻辑
    /// </summary>
    public partial class MyServer : Window
    {
        ServerWork sw = new ServerWork();
        public MyServer()
        {
            InitializeComponent();
        }

        string[] Info = { "Login Error ", "Login Ok" };  

   
        private void bn_Ping_Click(object sender, RoutedEventArgs e)
        {
            Destext.Text = ""; 
            ServerWork sw = new ServerWork();
            string result = sw.MyPing();
            Destext.Text += result; 
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            start.IsEnabled=false;
            Destext.Text = ""; 
            sw.startListen(8087 ,Destext);
            Destext.Text += "Starting Listening!!+\n";
            sw.Timer();
            Thread aa = new Thread(gg);
            aa.Start(); 
                          
        }
        private void gg()
        {
            this.Dispatcher.Invoke(new Action(gg2));
        }

        private void gg2()
        {
            while(true)
            {
                string aa = sw.waitclient();
                Destext.Text += aa;
                sw.dealclient();
            }
        }
        private void bn_file_Click(object sender, RoutedEventArgs e)
        {
            string filepath = null; 
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                 filepath = fileDialog.FileName;
                 int result = sw.sendfile(filepath);
                 switch(result)
                {
                    case -1:
                        MessageBox.Show("Error : Server not connect");
                        break;
                    case -2:
                        MessageBox.Show("Error : Send file error ");
                        break;
                    case 0:
                        MessageBox.Show("Send File ok ");
                        break;
                    default:
                        break; 
                }
            }
        }

        private void bn_stop_Click(object sender, RoutedEventArgs e)
        {
            sw.stopserver();
            Destext.Text += "Server has stopped!!\n"; 
            start.IsEnabled = true; 
        }

        private void bn_client_Click(object sender, RoutedEventArgs e)
        {// connect  ***********passwd  and user name //
            string IP = Srctext.Text; 
            int result = sw.ConnectServer(IP); 
            Destext.Text += Info[result];             

        }

        private void bn_info_Click_1(object sender, RoutedEventArgs e)
        {
            string tt = sw.ShowInfo();
            Destext.Text += tt; 
        }

        private void bn_Ins_Click(object sender, RoutedEventArgs e)
        {
            string tmp = Srctext.Text;
            Destext.Text += "Ins"+tmp ;
            Srctext.Text = "";
            string returns = sw.SendIns(tmp);
            Destext.Text += returns;
        }

        private void Srctext_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                string tmp = Srctext.Text;
                Destext.Text += tmp;
                Srctext.Text = "";
                string returns = sw.SendIns(tmp);
                Destext.Text += returns;  
            }
        }

    }
}
