﻿using K3yManager.DataDeal;
using K3yManager.UI;
using K3yManager.Weather;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using K3yManager.Works;
using System.Windows; 
using System.Xml;

namespace K3yManager
{

    public partial class Main : Window
    {

        const int UPDATE = 2333;
        const int ADD = 2334;
        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        private string username;
        private static int CHANCE = 0;
        private MyConfig con;
        private string FilePath = null;
        private bool ChangeFlag_Error = false;
        private int ChangeData_offset = 0;
        private Icon ico = new Icon("url.ico");
        Aworks aw = new Aworks();

        class runpy
        {
            public runpy(int count)
            {
                this.count = count;
            }
            private int count;
            public void run()
            {
                checktime(count);
            }
            private void checktime(int count)
            {

                Thread.Sleep(1000);
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                MyConfig con = new MyConfig();
                string oldtime = con.GetValue("time");
                if (oldtime.Equals(""))
                {
                    con.SetValue("time", now);
                    return;
                }
                DateTime told = DateTime.Parse(oldtime);
                DateTime tnow = DateTime.Parse(now);
                System.TimeSpan t3 = tnow - told;

                if (t3.TotalDays > 30)
                {
                    con.SetValue("time", now);
                    Pyworks pw = new Pyworks();
                    string pass = pw.work("..\\pyscript\\makepass " + count.ToString());
                    MessageBox.Show("Passwd Timeout" + pass, "WARNING", MessageBoxButton.OK);

                }
            }
        }

        public Main(string username)
        {

            notifyIcon.Icon = ico;
            notifyIcon.MouseDoubleClick += dclick;
            con = new MyConfig();
            con.SetValue("username", username);
            con.SetValue("islogin", "true");
            this.username = username;
            InitializeComponent();
            this.Closing += MyExit;
            Thread aa = new Thread(getCombobox);
            aa.Start();
        }
        public Main()
        {
            notifyIcon.Icon = ico;
            notifyIcon.MouseDoubleClick += dclick;
            con = new MyConfig();
            this.username = con.GetValue("username");
            InitializeComponent();
            this.Closing += MyExit;
            Thread aa = new Thread(getCombobox);
            aa.Start();

        }




        private void MyExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(" MINI", "CHOOICE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Cancel = true;
                this.Hide();
                this.notifyIcon.Visible = true;
                this.ShowInTaskbar = false;


            }
            else
            {
                e.Cancel = false;
            }
        }

        private void dclick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            this.Show();
            WindowState = WindowState.Normal;
            this.Focus();
        }

        enum ERROR
        {
            CHOOSER_ERROR, PASSWD_EMPTY, PASSWD_WRONG, FILE_MISSING
        }

        const string WARNING_TAG = "WARNING";
        private string[] myerror = { "TAG and COUNT and KEY can't be empty!!!", "FILE_MISSING", "PASSWD_WRONG,YOwaU HAVE TWO CHANCE" };
        private string[] show = { "     IMPORTANT     \nTAG:{0}\nCOUNT:{1}\nKEY:{2}\nSEC:{3}\n" ,
                                   "TAG:{0}\nCOUNT:{1}\nKEY{2}\n"};

        private string[] WAY2ENC = { "python", "c", "hex", "hexstring", "trim" };

        private string[] FILEWAY = { "AES", "DES", "RC4" };
        private void send2serverall(string username, string tag, string user, string pass)
        {

        }


        private void getCombobox()
        {
            this.Dispatcher.Invoke(new Action(getComboboxtag));
        }

        private void getComboboxtag()
        {
            int COUNT = 0;
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

            Aworks aw = new Aworks();

           /* if (con.GetValue("sig").Equals("true"))
            {
                if (!aw.Verify("message.xml"))
                {
                    this.Close();
                }
            }
            */
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
                        if (xe2.GetAttribute("tag") != "")
                        {
                            COMBOBOX.Items.Add(xe2.GetAttribute("tag"));
                            COUNT++;
                        }

                    }
                }
            }
            runpy ru = new runpy(COUNT);
            Thread bb = new Thread(ru.run);
            bb.Start();

            foreach (var astring in WAY2ENC)
            {
                Way2Enc.Items.Add(astring);
            }

            string a = con.GetValue("querytimes");
            int b = Int32.Parse(a) + 1;
            con.SetValue("querytime", b.ToString());
            foreach (var it in FILEWAY)
            {
                Com_Encway.Items.Add(it); 
            }
        }
        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (TAG.Text == "" || COUNT.Text == "" || KEY.Text == "" || SEC.Text == "")
            {
                MessageBox.Show(myerror[0], "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                string showmessage = String.Format(show[0], TAG.Text, COUNT.Text, KEY.Text, SEC.Text);
                if (MessageBox.Show(showmessage, "IMPORTANT", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    COUNT.Text = "";
                    KEY.Text = "";
                    SEC.Text = "";
                    return;
                }
            }

            COMBOBOX.Items.Add(TAG.Text);

            // first use DES .....
            byte[] user = Encoding.UTF8.GetBytes(COUNT.Text);
            byte[] pass = Encoding.UTF8.GetBytes(KEY.Text);

            byte[] sec = Encoding.UTF8.GetBytes(SEC.Text);
            Eenclass enc = new Eenclass();
            string rsauser = enc.hex2str(enc.aesenc(user, sec));
            string rsapass = enc.hex2str(enc.aesenc(pass, sec));


            send2serverall(username, TAG.Text, rsauser, rsapass);
            save2xml(username, TAG.Text, rsauser, rsapass, ADD);

            COUNT.Text = "";
            SEC.Text = "";
            KEY.Text = "";
            TAG.Text = "";
        }

        private void save2xml(string username, string tag, string rsauser, string rsapass, int choose)
        {
            MyConfig con = new MyConfig();

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

                        if (choose == ADD)
                        {
                            addexistuser(xmlDoc, root, xe, tag, rsauser, rsapass);
                        }
                        else
                        {
                            updateuser();
                        }
                    }
                }
                if (!issub)
                {
                    // add new user one 
                    addnewuser(xmlDoc, root, tag, rsapass, rsauser);

                }

            }
            else
            {
                createfile(rsauser, rsapass, tag);
            }
        }

        private void updateuser()
        {

        }
        private void addexistuser(XmlDocument xmlDoc, XmlNode root, XmlElement xe, string tag, string rsauser, string rsapass)
        {
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
            aw.Sigcreate("message.xml");
            con.SetValue("sig", "true");
        }

        private void addnewuser(XmlDocument xmlDoc, XmlNode root, string tag, string rsapass, string rsauser)
        {
            XmlElement xe1 = xmlDoc.CreateElement("USER");
            xe1.SetAttribute("Name", username);
            XmlElement xe11 = xmlDoc.CreateElement("TAG");
            xe11.SetAttribute("tag", tag);
            xe1.AppendChild(xe11);
            XmlElement xe111 = xmlDoc.CreateElement("user");
            XmlElement xe112 = xmlDoc.CreateElement("pass");
            xe111.InnerText = rsauser;
            xe112.InnerText = rsapass;
            xe11.AppendChild(xe111);
            xe11.AppendChild(xe112);
            xe1.AppendChild(xe11);
            root.AppendChild(xe1);
            xmlDoc.Save("message.xml");
            aw.Sigcreate("message.xml");
            con.SetValue("sig", "true");
        }
        private void createfile(string rsauser, string rsapass, string tag)
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
            aw.Sigcreate("message.xml");
            con.SetValue("sig", "true");
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
                queryfinal(i);
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
                            return 0;
                        }

                    }
                }
            }
            return 0;
        }

        private void query_Click(object sender, RoutedEventArgs e)
        {
            if (SEC.Text == "")
            {
                MessageBox.Show("SEC can't be empty");
                return;
            }
            string tag = COMBOBOX.Text;
            byte[] sec = Encoding.UTF8.GetBytes(SEC.Text);

            int i = queryxml(username, tag, sec);
            queryfinal(i);

        }

        private void queryfinal(int i)
        {
            if (i == 0)
            {
                return;
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

        private int queryxml(string username, string tag, byte[] sec)
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
                        if (xe2.GetAttribute("tag") == tag)
                        {
                            XmlNode usernode = xe2.ChildNodes[0];
                            XmlNode passnode = xe2.ChildNodes[1];
                            string subuser = usernode.InnerText;
                            string subpass = passnode.InnerText;
                            decrypto(tag, subuser, subpass, sec);
                            CHANCE = 0;
                            return 0;
                        }

                    }
                    CHANCE++;
                }
            }

            return 0;
        }

        private void decrypto(string tag, string user, string pass, byte[] sec)
        {
            Decry de = new Decry();
            byte[] binuser = de.str2hex(user);
            byte[] binpass = de.str2hex(pass);
            string depass = Encoding.Default.GetString(de.decaes(binuser, sec));
            string deuser = Encoding.Default.GetString(de.decaes(binpass, sec));
            string message = String.Format(show[1], tag, deuser, depass);
            COUNT.Text = deuser;
            TAG.Text = tag;
            KEY.Text = depass; 
            //MessageBox.Show(message, "MESSAGE", MessageBoxButton.OK);
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
            catch (Exception)
            { }
        }

        private void Do_Click(object sender, RoutedEventArgs e)
        {
            bool over = checkover.IsChecked == true ? true : false;

            if (over)
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
            fileDialog.Filter = "(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
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
                long current = 0, first;
                int count = 0;
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                FileStream target = new FileStream(path, FileMode.Create);

                fs.Seek(0, SeekOrigin.Begin);
                first = fs.Length;
                count = fs.Read(data, 0, 4096);
                current += count;
                while (current < first)
                {
                    data = changedata(data);
                    target.Write(data, 0, data.Length);
                    data.Initialize();
                    if (current + 4096 > first)
                    {
                        size = (int)(first - current);
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

            byte[] final = new byte[4] { 0x50, 0x4B, 0x01, 0x02 };
            for (int i = 0; i < data.Length; i++)
            {
                if (ChangeFlag_Error)
                {
                    data[i + ChangeData_offset] = 0x00;
                    ChangeFlag_Error = false;
                    ChangeData_offset = 0;
                }

                if (data[i] == final[0] && data[i + 1] == final[1] && data[i + 2] == final[2] && data[i + 3] == final[3])
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
                        ChangeData_offset = 7 - data.Length + i;
                    }
                }
            }

            return data;

        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            string src = Srctext.Text;
            string way = Way2Enc.Text;
            string des = dealstring(src, way);

            Destext.Text = des;
        }


        private string dealstring(string src, string way)
        {
            string result = null;
            MyDataFormat a = new MyDataFormat();

            if (way.Equals("hex"))
            {
                result = a.dealhex(src);
            }
            else if (way.Equals("hexstring"))
            {
                result = a.dealhexstring(src);
            }
            else if (way.Equals("python"))
            {
                result = a.dealpython(src);
            }
            else if (way.Equals("c"))
            {
                result = a.dealc(src);
            }
            else if (way.Equals("trim"))
            {
                result = a.dealtrim(src);

            }


            return result;
        }

        private void weather_Click(object sender, RoutedEventArgs e)
        {
            Myweather wea = new Myweather();
            wea.Show();
        }

        private void pywork_Click(object sender, RoutedEventArgs e)
        {
            MyPywork my = new MyPywork();
            my.Show();
        }

        private void UpDate_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("message.xml"))
            {
                MessageBox.Show("FILE MISSING");
            }
            if (COUNT.Text == "" || KEY.Text == "" || SEC.Text == "")
            {
                MessageBox.Show(myerror[0], "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                string showmessage = String.Format(show[0], COMBOBOX.Text, COUNT.Text, KEY.Text, SEC.Text);
                if (MessageBox.Show(showmessage, "IMPORTANT", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    COUNT.Text = "";
                    KEY.Text = "";
                    SEC.Text = "";
                    return;
                }
            }

            byte[] user = Encoding.UTF8.GetBytes(COUNT.Text);
            byte[] pass = Encoding.UTF8.GetBytes(KEY.Text);

            byte[] sec = Encoding.UTF8.GetBytes(SEC.Text);
            Eenclass enc = new Eenclass();

            string rsauser = enc.hex2str(enc.aesenc(user, sec));
            string rsapass = enc.hex2str(enc.aesenc(pass, sec));

            send2serverall(username, COMBOBOX.Text, rsauser, rsapass);
            save2xml(username, TAG.Text, rsauser, rsapass, UPDATE);

            COUNT.Text = "";
            SEC.Text = "";
            KEY.Text = "";
            TAG.Text = "";

        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            FileScan fs = new FileScan();
            fs.Show();

        }

        private void Server_Click(object sender, RoutedEventArgs e)
        {
            MyServer ms = new MyServer();
            ms.Show();
        }

        private void browser_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                FilePath = fileDialog.FileName;
                enc_path.Text = FilePath;
            }
        }

        private void bn_refresh_Click(object sender, RoutedEventArgs e)
        {
            
            Aworks aw = new Aworks();
            string name = aw.getUname();
            if (name==null)
            {
                MessageBox.Show("检测有误，请稍后再试！！！", "错误", MessageBoxButton.OK);
                return;
            }

            key.Text = "12345";//aw.getSerialNumberFromDriveLetter(name);

        }

        private void enc_Click(object sender, RoutedEventArgs e)
        {
            if (Salt.Text.Equals("") || key.Text.Equals("") || enc_path.Equals("") 
                ||  Com_Encway.Text.Equals("")) 
            {
                MessageBox.Show("Something wrong ,Please recheck"); 
                return; 
            }

            Aworks aw = new Aworks();
            if(aw.webcheckpass(Salt.Text)==false)
            {
                return; 
            }

            byte[] fkey = aw.generatekey(Salt.Text,key.Text);

            if (checkBox.IsChecked == false)
            {
                int index = FilePath.IndexOf(".");
                string newpath = FilePath.Substring(0, index) + "new" + FilePath.Substring(index);
                MessageBox.Show(newpath);
                checkstate(aw.EncFile( FilePath,  newpath, Com_Encway.Text,fkey)) ; 
             
            }
            else
            {
                //don't overwrite
                checkstate(aw.EncFile(FilePath, Com_Encway.Text,fkey));
             
            }

        }

        private void checkstate(bool state)
        {
            if(state)
            {
                MessageBox.Show("ok");
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void dec_Click(object sender, RoutedEventArgs e)
        {
            if (Salt.Text.Equals("") || key.Text.Equals("") || enc_path.Equals("")
               || Com_Encway.Text.Equals(""))
            {
                MessageBox.Show("Something wrong ,Please recheck");
                return;
            }

            Aworks aw = new Aworks();
            if (aw.webcheckpass(Salt.Text) == false)
            {
                return;
            }

            byte[] fkey = aw.generatekey(Salt.Text, key.Text);
            if (checkBox.IsChecked == false)
            {
                int index = FilePath.IndexOf(".");
                string newpath = FilePath.Substring(0, index-3) + "src" + FilePath.Substring(index);
                MessageBox.Show(newpath);
                checkstate(aw.DecFile(FilePath ,newpath, Com_Encway.Text,fkey));
        
            }
            else
            {
                checkstate(aw.DecFile(FilePath, Com_Encway.Text,fkey)) ;
            }

        }

        private void cloud_Click(object sender, RoutedEventArgs e)
        {

        }
        private void download_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DEX_Click(object sender, RoutedEventArgs e)
        {
            string filepath = ChoosedPath.Text;
            Dex aa = new Dex(filepath);
            aa.Show(); 
        }

        private void APK_Click(object sender, RoutedEventArgs e)
        {
            string filepath = ChoosedPath.Text;
            ApkWork apkshell = new ApkWork(filepath);
           int i =  apkshell.encapk();
            if (i == 0)
            {
                MessageBox.Show("ok"); 
            }
            else
            {
                MessageBox.Show("Error");
            }
        }
    }

  
}
