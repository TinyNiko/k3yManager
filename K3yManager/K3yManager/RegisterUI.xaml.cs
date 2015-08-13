using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace K3yManager
{
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        private bool eq =false; 
        private BackgroundWorker hehe;
        public delegate int Checkall();
        public Register()
        {
            InitializeComponent();
            Thread aa = new Thread(startDelegate);
            aa.Start(); 
           
        }
        public void startBackWork()
        {
            Backinit();
            Backstart(); 
        }

        public void Backinit()
        {
            hehe = new BackgroundWorker();
            hehe.WorkerSupportsCancellation = false;
         
        }

        public void Backstart() 
        {
            hehe.RunWorkerAsync(); 
        }

        private void startDelegate()
        {
            while(true)
            {
               
                this.Dispatcher.Invoke(new Action(checkstate));
                if (eq == true)
                    break; 
            }
        }

        void   checkstate() 
        {
                if(username.Text != "" )
                {      
                    checkuser.IsChecked = true; 
                }
                if(passwd.Password != "")
                {
                    checkpwd1.IsChecked = true; 
                }
                if(passwd2.Password !="" && ( passwd2.Password == passwd.Password))
                {
                    checkpwd2.IsChecked = true; 
                }
                if (checkuser.IsChecked == true && checkpwd1.IsChecked == true && checkpwd2.IsChecked == true)
                {
                    Regbn.IsEnabled = true ;
                   
                }

             
        }
        
      
   
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("input you information", "HELP", MessageBoxButton.OK); 
        }

        private void Regbn_Click(object sender, RoutedEventArgs e)
        {
            eq = true; 
            //save message and encrypt 
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       

    
    }
}
