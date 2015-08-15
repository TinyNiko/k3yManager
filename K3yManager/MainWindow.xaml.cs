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
using System.Xml;
using System.Configuration;





namespace K3yManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        enum ERROR
        {
            CHOOSER_ERROR , PASSWD_EMPTY , PASSWD_WRONG ,FILE_MISSING 
        }
       
        public string[] error = {"FIRST REGISTER\nTHEN YOU CAN LOGIN!!\n" ,
                                 "user and passwd can't be empty!!!",
                                 "user or passwd is wrong" ,
                                 "can't find  file!!\nPlease register"}; 
        public MainWindow()
        {
           
            InitializeComponent();
            checkconfig(); 
        }

        private void checkconfig()
        {
            MyConfig con = new MyConfig(); 
            if(con.GetValue("islogin").Equals("false") )
            {
                return ;             
            }
            else
            {
                Main main = new Main();
                main.Show();
                this.Close(); 
            }
        }
        private void HELP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(error[0],"HELP" ,MessageBoxButton.OK); 
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
                MessageBox.Show(error[1], "Waring", MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            
            // use MD5 and DES encrypt  user 
            // use SHA and AES encrypt passwd  
            // and check local user if not online check 
            //TODO
            String usernum = makeuser(user.Text ,pwd.Text);
            String passwdnum = makepwd(pwd.Text); 
            int i = checkall(usernum , passwdnum) ; 
            if(i == 0)
            {
                Main main = new Main(usernum);
                main.Show();
                this.Close(); 
            }
            else if(i ==(int)ERROR.FILE_MISSING)
            {   
                //online check
                MessageBox.Show(error[i], "wrong", MessageBoxButton.OK, MessageBoxImage.Error) ; 
            }
            else
            {
                MessageBox.Show(error[i], "wrong", MessageBoxButton.OK, MessageBoxImage.Error) ; 
            }

        }

        private int checkall(string usernum, string passwdnum)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load("hehe.xml");
            }
            catch(Exception)
            {
                
                return (int)ERROR.FILE_MISSING ; 
         
            }

            XmlNode root = xmlDoc.SelectSingleNode("ALLUSER");
            // lookup the id 
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ALLUSER").ChildNodes;
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                XmlNode usernode = xe.ChildNodes[0];
                XmlNode passnode = xe.ChildNodes[1];
                XmlElement subuse = (XmlElement)usernode;
                XmlElement subpass = (XmlElement)passnode;  
                if(subuse.InnerText.Equals(usernum))
                {
                    if(subpass.InnerText.Equals(passwdnum))
                    {
                        return 0 ; 
                    }
                }
            }
            
            return (int)ERROR.PASSWD_WRONG; 
        }

        private string makepwd(string passwd)
        {
            Eenclass enc; 
            byte[] pass = Encoding.UTF8.GetBytes(passwd) ;
            enc = new Eenclass(pass);
            byte[] shapass = enc.MySHA256(); 
            enc = new Eenclass(shapass, pass);
            byte[] aaa = enc.aesenc();
            string result = enc.hex2str(aaa);

            return result; 
        }

        private string makeuser(string user,string passwd )
        {
            Eenclass enc; 
            byte[] users = Encoding.UTF8.GetBytes(user); 
            byte[] pass = Encoding.UTF8.GetBytes(passwd);
            enc = new Eenclass(users);
            byte[] md5user = enc.Mymd5();
            enc = new Eenclass(md5user, pass);  
            byte[] aaa = enc.desenc();
            string result = enc.hex2str(aaa);

            return result; 
            
        }
    }
}
