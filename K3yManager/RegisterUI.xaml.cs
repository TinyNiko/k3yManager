using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Xml;

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
                else
                {
                    checkuser.IsChecked = false; 
                }
                if(passwd.Password != "")
                {
                    checkpwd1.IsChecked = true;
                }
                else
                {
                    checkpwd1.IsChecked = false;
                }
                if (passwd2.Password != "" && (passwd2.Password.Length == passwd.Password.Length) && (passwd.Password.Equals(passwd2.Password)))
                {
                    checkpwd2.IsChecked = true;
                }
                else
                {
                    checkpwd2.IsChecked = false;
                }

                if (checkuser.IsChecked == true && checkpwd1.IsChecked == true && checkpwd2.IsChecked == true)
                {
                    Regbn.IsEnabled = true ;
                   
                }
                else
                {
                    Regbn.IsEnabled = true;
                }

             
        }
        
      
   
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("input you information", "HELP", MessageBoxButton.OK); 
        }

        private void Regbn_Click(object sender, RoutedEventArgs e)
        {
            
            //save message and encrypt 
            string userstr = username.Text;
            if (userstr.Equals(""))
                return; 
            string pass = passwd.Password;
            if (pass.Equals(""))
                return; 
          
            bool encstream = encrypt(userstr, pass); 
            if(encstream == true)
            {
                MessageBox.Show("user is already exist","WARNING");
                username.Text = ""; 

            }
            eq = true; 
            this.Close(); 
        }

        
        public bool encrypt(string user ,string passwd)
        {

            byte[] userbyte = System.Text.Encoding.UTF8.GetBytes(user);
            byte[] pass = System.Text.Encoding.UTF8.GetBytes(passwd);
            Eenclass enc; 
            bool isexist = true ; 
            
            enc = new Eenclass(userbyte) ;
            byte[] md5user = enc.Mymd5() ; //202cb962
            enc = new Eenclass(pass); 
            byte[] shapass = enc.MySHA256() ;
            enc = new Eenclass(md5user, pass); //a665a459 
            byte[] final_user = enc.desenc();
            enc = new Eenclass(shapass, pass); // 1eaf67e9 
            byte[] final_pass = enc.aesenc(); //d63804

            isexist = send2server(final_user);
           
            if (isexist == true)
                return isexist; 
            
            createXml(final_user, final_pass);
           
            return isexist;


        }

        private bool send2server(byte[] user)
        {
            return false ; 
        }

        public string hex2str(byte[] src)
        {
            string str = BitConverter.ToString(src).Replace("-",string.Empty); 

            return str; 
        }

        private void createXml(byte[] user, byte[] pass)
        {
            int i = 0  ; 
            string userstr = hex2str(user);
            string passstr = hex2str(pass); 
            if (File.Exists("hehe.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("hehe.xml");
                XmlNode root = xmlDoc.SelectSingleNode("ALLUSER");
                // lookup the id 
                XmlNodeList nodeList=xmlDoc.SelectSingleNode("ALLUSER").ChildNodes;
                foreach (XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;
                    if(xe.GetAttribute("id") !="" )
                    {
                        i++; 
                    }
                }
                XmlElement xe1 = xmlDoc.CreateElement("USER");
                xe1.SetAttribute("id", i.ToString()) ;
                XmlElement xesub1 = xmlDoc.CreateElement("user");
                xesub1.InnerText =userstr;
                xe1.AppendChild(xesub1);
                
                XmlElement xesub2 = xmlDoc.CreateElement("pass");
                xesub2.InnerText = passstr;
                xe1.AppendChild(xesub2);
                
                root.AppendChild(xe1);
                xmlDoc.Save("hehe.xml");
            }
            else
            {   
                XmlTextWriter writer = new XmlTextWriter("hehe.xml", System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("ALLUSER");
                writer.WriteStartElement("USER");
                writer.WriteAttributeString("id", "0");
                writer.WriteStartElement("user");
                writer.WriteBinHex(user, 0, user.Length);
                writer.WriteEndElement();
                writer.WriteStartElement("pass");
                writer.WriteBinHex(pass, 0, pass.Length);
                writer.WriteEndElement();
                writer.Close(); 
            }
            
        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
