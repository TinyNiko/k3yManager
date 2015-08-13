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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace K3yManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HELP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FIRST REGISTER\nTHEN YOU CAN LOGIN!!\n" ,"HELP" ,MessageBoxButton.OK); 
        }

        private void REGISTER_Click_1(object sender, RoutedEventArgs e)
        {
            Register reg = new Register() ;
            reg.ShowDialog();  
        }

        private void LOGIN_Click(object sender, RoutedEventArgs e)
        {
            if(user.Text == "" || pwd.Text == "" )
            {
                MessageBox.Show("user and passwd can't be empty!!!", "Waring", MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            
            // use SHA-256 and DES encrypt  user 
            // use MD5 and AES encrypt passwd  
            // and check local user if not online check 
            //TODO
            String usernum = makeuser(user.Text);
            String passwdnum = makepwd(pwd.Text); 
         
            if(checkall(usernum , passwdnum))
            {
                Main main = new Main();
                main.Show();
                this.Close(); 
            }
            else
            {
                MessageBox.Show("user or passwd is wrong", "wrong", MessageBoxButton.OK, MessageBoxImage.Error); 
            }

        }

        private bool checkall(string usernum, string passwdnum)
        {//TODO

            return true ; 
        }

        private string makepwd(string p)
        {
            return "niko"; 
        }

        private string makeuser(string p)
        {
            return "123" ;
        }
    }
}
