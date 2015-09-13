using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace K3yManager
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {   
        
        private string username;
        private static int CHANCE = 0;
        private MyConfig con;
        private string FilePath = null;
        private bool ChangeFlag_Error = false;
        private int ChangeData_offset = 0;   
        public Main(string username)
        {
            con = new MyConfig();
            con.SetValue("username", username);
            con.SetValue("islogin", "true"); 
            this.username = username ;
            InitializeComponent();
            Thread aa = new Thread(getCombobox);
            aa.Start(); 
           
        }
        public Main()
        {
            con = new MyConfig() ; 
            this.username = con.GetValue("username") ; 
            InitializeComponent();
            Thread aa = new Thread(getCombobox);
            aa.Start(); 
        }
        enum ERROR 
        {
            CHOOSER_ERROR, PASSWD_EMPTY, PASSWD_WRONG, FILE_MISSING
        }

        const string WARNING_TAG = "WARNING"; 
        private string[] myerror = { "TAG and COUNT and KEY can't be empty!!!" ,"FILE_MISSING","PASSWD_WRONG,YOU HAVE TWO CHANCE"};
        private string[] show = { "     IMPORTANT     \nTAG:{0}\nCOUNT:{1}\nKEY:{2}\nSEC:{3}\n" ,
                                   "TAG:{0}\nCOUNT:{1}\nKEY{2}\n"}; 

        private void send2serverall(string username ,string tag , string user ,string pass)
        {

        }

        private void getCombobox()
        {
            this.Dispatcher.Invoke(new Action(getComboboxtag)); 
        }

        private void getComboboxtag()
        {
            MyConfig con = new MyConfig();
            if (!File.Exists("message.xml"))
            { 
                if (con.GetValue("querytimes") != "0")
                {
                    MessageBox.Show(myerror[1], "WARNING", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                return; 
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("message.xml");
            XmlNode root = xmlDoc.SelectSingleNode("ALLUSER");
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ALLUSER").ChildNodes;
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("Name") == username)
                {
                    foreach (XmlNode xn2 in xe.ChildNodes)
                    {
                        XmlElement xe2 = (XmlElement)xn2;
                        if (xe2.GetAttribute("tag") !="")
                        {
                            COMBOBOX.Items.Add(xe2.GetAttribute("tag")); 
                        }

                    }
                }
            }
            string a = con.GetValue("querytimes") ; 
            int b = Int32.Parse(a) + 1 ;
            con.SetValue("querytime", b.ToString());
        }
        private void add_Click(object sender, RoutedEventArgs e)
        {
            if(TAG.Text=="" || COUNT.Text=="" || KEY.Text=="" ||SEC.Text =="")
            {
                MessageBox.Show(myerror[0] , "ERROR" , MessageBoxButton.OK ,MessageBoxImage.Error) ;
                return ; 
            }
            else
            {
                 string showmessage = String.Format(show[0] , TAG.Text , COUNT.Text  ,KEY.Text ,SEC.Text) ; 
                if( MessageBox.Show(showmessage, "IMPORTANT", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    COUNT.Text = "" ;
                    KEY.Text = ""  ;
                    SEC.Text =""   ; 
                    return; 
                }
            }

            COMBOBOX.Items.Add(TAG.Text) ; 
            
            // first use DES .....
            byte[] user = Encoding.UTF8.GetBytes(COUNT.Text) ; 
            byte[] pass = Encoding.UTF8.GetBytes(KEY.Text)   ;
           
            byte[] sec = Encoding.UTF8.GetBytes(SEC.Text)    ;
            Eenclass enc = new Eenclass() ;
            string rsauser = enc.hex2str(enc.aesenc(user, sec)) ; 
            string rsapass = enc.hex2str(enc.aesenc(pass, sec)) ;
        
            
            send2serverall(username ,TAG.Text ,rsauser , rsapass ) ;
            save2xml(username,TAG.Text,rsauser,rsapass);

            COUNT.Text = "";
            SEC.Text = ""  ;
            KEY.Text = ""  ;
            TAG.Text = ""  ; 
        }

        private void save2xml(string username , string tag , string rsauser, string rsapass)
        {
            bool issub = false;
            if (File.Exists("message.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("message.xml");
                XmlNode root = xmlDoc.SelectSingleNode("ALLUSER");
                 
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("ALLUSER").ChildNodes;
                foreach (XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;
                    if (xe.GetAttribute("Name") == username)
                    {
                        issub = true;
                        XmlElement xe1 = xmlDoc.CreateElement("TAG");
                        xe1.SetAttribute("tag", tag);
                        XmlElement xe11 = xmlDoc.CreateElement("user");
                        xe11.InnerText = rsauser;
                        xe1.AppendChild(xe11);

                        XmlElement xe12 = xmlDoc.CreateElement("pass");
                        xe12.InnerText = rsapass;
                        xe1.AppendChild(xe12);
                        xe.AppendChild(xe1); 
                        root.AppendChild(xe);
                        xmlDoc.Save("message.xml");

                    }
                }
                if(!issub)
                {
                    // add new one 
                    XmlElement xe1 = xmlDoc.CreateElement("USER");
                    xe1.SetAttribute("Name", username); 
                    XmlElement xe11 = xmlDoc.CreateElement("TAG");
                    xe11.SetAttribute("tag" , tag) ;
                    xe1.AppendChild(xe11);
                    XmlElement xe111 = xmlDoc.CreateElement("user"); 
                    XmlElement xe112 = xmlDoc.CreateElement("pass");
                    xe111.InnerText = rsauser ;
                    xe112.InnerText = rsapass ;
                    xe11.AppendChild(xe111);
                    xe11.AppendChild(xe112);
                    xe1.AppendChild(xe11);
                    root.AppendChild(xe1);
                    xmlDoc.Save("message.xml");
                }
               
            }
            else
            {
                XmlTextWriter writer = new XmlTextWriter("message.xml", System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("ALLUSER");
                writer.WriteStartElement("USER");
                writer.WriteAttributeString("Name", username);
                writer.WriteStartElement("TAG");
                writer.WriteAttributeString("tag", tag); 
                writer.WriteStartElement("user");
                writer.WriteString(rsauser); 
                writer.WriteEndElement();
                writer.WriteStartElement("pass");
                writer.WriteString(rsapass);
                writer.WriteEndElement();
                writer.Close();
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;  
            if (COMBOBOX.SelectedItem != null)
            {
                Eenclass enc = new Eenclass();
                string delete = COMBOBOX.Text; 
                COMBOBOX.Items.Remove(COMBOBOX.SelectedItem);
                i = deletexml(delete);
                queryfinal(i) ;
            }
        }

        private int deletexml(string dele)
        {
            if (!File.Exists("message.xml"))
            {
                return (int)ERROR.PASSWD_WRONG;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("message.xml");
            XmlNode root = xmlDoc.SelectSingleNode("ALLUSER");

            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ALLUSER").ChildNodes;
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("Name") == username)
                {
                    foreach (XmlNode xn2 in xe.ChildNodes)
                    {
                        XmlElement xe2 = (XmlElement)xn2;
                        if (xe2.GetAttribute("tag") == dele)
                        {
                            xe.RemoveChild(xe2);
                            xmlDoc.Save("message.xml");
                            return 0 ; 
                        }

                    }
                }
            }
            return 0; 
        }

        private void query_Click(object sender, RoutedEventArgs e)
        {   
            if(SEC.Text == "")
            {
                MessageBox.Show("SEC can't be empty");
                return; 
            }
            string tag = COMBOBOX.Text; 
            byte[] sec = Encoding.UTF8.GetBytes(SEC.Text)     ; 

            int i = queryxml(username , tag ,sec);
            queryfinal(i);

        }

        private void queryfinal(int i)
        {
            if (i == 0)
            {
                return ; 
            }
            if (i == 3)
            {
                MessageBox.Show(myerror[3], WARNING_TAG, MessageBoxButton.OK);
            }
            if (i == 4)
            {
                MessageBox.Show(myerror[4], WARNING_TAG, MessageBoxButton.OK);

            }
        }

        private int queryxml(string username , string tag,byte[] sec)
        {
           
            
            if(!File.Exists("message.xml"))
            {
                return (int)ERROR.PASSWD_WRONG; 
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("message.xml");
            XmlNode root = xmlDoc.SelectSingleNode("ALLUSER");

            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ALLUSER").ChildNodes;
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("Name") == username)
                {
                   foreach(XmlNode xn2 in xe.ChildNodes)
                   {
                       XmlElement xe2 = (XmlElement)xn2; 
                       if(xe2.GetAttribute("tag") == tag)
                       {
                           XmlNode usernode = xe2.ChildNodes[0];
                           XmlNode passnode = xe2.ChildNodes[1];
                           string subuser = usernode.InnerText;
                           string subpass = passnode.InnerText;
                           decrypto(tag,subuser, subpass,sec);
                           CHANCE = 0;
                           return 0; 
                       }
                   }
                }
            }
            
            return  0 ; 
        }

        private void decrypto( string tag ,string user , string pass,byte[] sec)
        {
            Decry de = new Decry() ; 
            byte[] binuser = de.str2hex(user) ;
            byte[] binpass =  de.str2hex(pass);
            string depass = Encoding.Default.GetString(de.decaes(binuser , sec) ); 
            string deuser = Encoding.Default.GetString(de.decaes(binpass ,sec))  ; 
            string message=String.Format(show[1], tag,deuser, depass) ;
            MessageBox.Show(message, "MESSAGE", MessageBoxButton.OK); 
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyConfig con = new MyConfig();
                con.SetValue("username", "");
                con.SetValue("islogin", "False");
                MainWindow xx = new MainWindow(true);
                xx.Show();
                this.Close(); 

            }
            catch(Exception)
            { }
        }
        
        private void Do_Click(object sender, RoutedEventArgs e)
        {
            bool over = checkover.IsChecked == true ? true : false; 

            if(over)
            {
                writesth(FilePath); 
            }
            else
            {
                copyandwrite(); 
            }
            MessageBox.Show("Change OK");       
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "(*.*)|*.zip;*.apk";
            if (fileDialog.ShowDialog() == true )
            {
                FilePath = fileDialog.FileName;
                ChoosedPath.Text = FilePath; 
            }
        }


        private void copyandwrite()
        {
            int index = FilePath.IndexOf(".");
            string newpath = FilePath.Substring(0, index) + "new" + FilePath.Substring(index);
            MessageBox.Show(newpath); 
            writesth(newpath);   

        }

        private void writesth(String path)
        {
           byte[] data = new byte[4096];
            try
            {
                //TODO
                
                int size = 4096; 
                long current=0 , first; 
                int count = 0; 
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                FileStream target = new FileStream(path, FileMode.Create);

                fs.Seek(0, SeekOrigin.Begin);
                first = fs.Length;
                count = fs.Read(data, 0, 4096);
                current += count; 
                while(current < first)
                {
                    data=changedata(data);
                    target.Write(data,0,data.Length);
                    data.Initialize();
                    if(current +4096 > first)
                    {
                        size =(int)(first - current);
                        count = fs.Read(data, 0, size);
                        data = changedata(data);
                        target.Write(data, 0, size); 
                        break; 
                    }
                    count = fs.Read(data, 0, size);
                    current += count; 
                }

                fs.Close();
                target.Close(); 
                
            }
            catch (Exception)
            { }
        }



        private byte[] changedata(byte[] data)
        {
   
            byte[] final = new byte[4]{ 0x50, 0x4B ,0x01,0x02};
            for(int i =0; i <data.Length; i++)
            {   
                if(ChangeFlag_Error)
                {
                    data[i + ChangeData_offset] = 0x00;
                    ChangeFlag_Error = false;
                    ChangeData_offset = 0;
                } 

                if(data[i]==final[0] &&data[i+1]==final[1] && data[i+2] ==final[2]&& data[i+3] ==final[3])
                {
                    if (i + 8 < data.Length)
                    {
                        if (data[i + 8] != 0x00)
                        {
                            MessageBox.Show("Found");
                            data[i + 8] = 0x00;

                        }
                    }
                    else
                    {
                        ChangeFlag_Error = true;
                        ChangeData_offset = 7-data.Length +i; 
                    }
                }
            }

            return data; 

        }

    }
}
