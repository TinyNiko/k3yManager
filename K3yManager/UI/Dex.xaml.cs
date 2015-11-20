using K3yManager.Works;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace K3yManager.UI
{
    /// <summary>
    /// Dex.xaml 的交互逻辑
    /// </summary>
    public partial class Dex : Window
    {

        List<DexWorks> dwx =new List<DexWorks>();
        DexWorks dw= null; 
        private string filepath;
        private string[] header_tmp = null;
        
        List<string> pathlist = new List<string>();
        Hashtable allhash = new Hashtable();
        string[] string_off_str = { "index", "offset", "size", "content" };
        string[] namearray = { "Name","magic", "checksum","sig","file_size" , "header_size" , "endan_tag","link_size",
        "link_off","map_off","string_ids_size","string_off" , "type_size","type_off","proto_size","proto_off" ,
        "field_size" , "field_off" , "method_size","method_off","class_size","class_off","data_size","data_off"};
        int[] sizearray = { 0,8, 4,20,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,};
        Hashtable hash = new Hashtable(); 
        public Dex(string path)
        {
            inithash(); 
            filepath = path;
            pathlist.Add(path);
            dw = new DexWorks(path);
            dwx.Add(dw); 
            InitializeComponent();
            AddTreeView();
            AddTabPage(path);
        }

        private void inithash()
        {
            allhash.Add(0x0000, "TYPE_HEADER");
            allhash.Add(0x0001, "TYPE_STRING_ID");
            allhash.Add(0x0002, "TYPE_TYPE_ID");
            allhash.Add(0x0003, "TYPE_PROTO");
            allhash.Add(0x0004, "TYPE_FIELD_ID");
            allhash.Add(0x0005, "TYPE_METHOD");
            allhash.Add(0x0006, "TYPE_CLASSDEF");
            allhash.Add(0x2001, "TYPE_CODE_ITEM");
            allhash.Add(0x1001, "TYPE_TYPELIST");
            allhash.Add(0x1002, "TYPE_ANNO_SET_REF");
            allhash.Add(0x1003, "TYPE_ANNO_SET_ITEM");
            allhash.Add(0x2000, "TYPE_CLASS_DATA");
            allhash.Add(0x2002, "TYPE_STRING_DATA");
            allhash.Add(0x2003, "TYPE_DEBUG_INFO");
            allhash.Add(0x2004, "TYPE_ANNOTION_ITEM");
            allhash.Add(0x2005, "TYPE_ENCODED_ARRAY");
            allhash.Add(0x2006, "TYPE_ANNO_DIRECT");
            allhash.Add(0x1000, "TYPE_MAPLIST");

        }
        private void AddTreeView()
        {
            TreeViewItem node = new TreeViewItem();
            node.Header = "DexFile";
            TreeViewItem node2 = new TreeViewItem();
            node2.Header = "DexClass";
            treeView1.Items.Add(node);
            treeView1.Items.Add(node2);
            TreeViewItem node01 = new TreeViewItem();
            node01.Header = "stringoff";
            TreeViewItem node02 = new TreeViewItem();
            node02.Header = "typeoff";
            TreeViewItem node03 = new TreeViewItem();
            node03.Header = "protooff";
            TreeViewItem node04 = new TreeViewItem();
            node04.Header = "fieldoff";
            TreeViewItem node05 = new TreeViewItem();
            node05.Header = "methodoff";
            TreeViewItem node011 = new TreeViewItem();
            node011.Header = "mapoff";
   
            node.Items.Add(node01);
            node.Items.Add(node02);
            node.Items.Add(node03);
            node.Items.Add(node04);
            node.Items.Add(node05);
            node.Items.Add(node011);

            TreeViewItem cnode1 = new TreeViewItem();
            cnode1.Header = "class_defs";
            TreeViewItem cnode12 = new TreeViewItem();
            cnode12.Header = "class_data";
            TreeViewItem cnode13 = new TreeViewItem();
            cnode13.Header = "code_off";
            node2.Items.Add(cnode1); 
            node2.Items.Add(cnode12);
            node2.Items.Add(cnode13); 




        }

        private void AddTabPage(string path)
        {
            string[] xx = path.Split(new char[2] { '\\', '.' });
            TabItem page = new TabItem();
            page.Header = xx[1];
            page.Name = xx[1];
            ScrollViewer sv = new ScrollViewer(); 
            StackPanel sp = new StackPanel();
            sv.Content = sp; 
            sp.Orientation= Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;  
            CreateHeader(sp);
            hash.Add("header", sv);
            page.Content = sp; 
            tabControl1.Items.Add(page);
        }

        private void CreateHeader(StackPanel sp)
        {
            header_tmp = dw.AnaHeader();
            if(header_tmp.Length==0)
            {
                MessageBox.Show("ERROR FORMAT");
                return; 
            }
            int off = 0; 
            for(int i =0; i <= 23; i++)
            {
                StackPanel s = new StackPanel();
                s.Orientation = Orientation.Horizontal;
                s.Height = 20;  
                    Button Offset = new Button();
                    Offset.Width = 100;
                    Offset.Content = Convert.ToString(off,16);
                    Button Name = new Button();
                    Name.Width = 100;
                    Name.Content = namearray[i];
                    Button Size = new Button();
                    Size.Width = 100;
                    Size.Content = sizearray[i]; 

               if(i ==0 )
               {
                    Offset.Content = "Offset";
                    Name.Content = "Name";
                    Size.Content = "Size";
                    Button value = new Button();
                    value.Width = 100;
                    value.Content = "Value";
                    s.Children.Add(Offset);
                    s.Children.Add(Name);
                    s.Children.Add(Size);
                    s.Children.Add(value);

                }
                else
                {
                    TextBox tb = new TextBox();
                    tb.Width = 100;
                    tb.FontSize = 15;
                    tb.Text = header_tmp[i - 1];
                    s.Children.Add(Offset);
                    s.Children.Add(Name);
                    s.Children.Add(Size);
                    s.Children.Add(tb);
                    off += sizearray[i]; 
                }
              
                sp.Children.Add(s); 
            }
        }
        private void bn_About_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Version 1.0 By N1k0"); 
        }
        

        private void bn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void bn_Open_Click(object sender, RoutedEventArgs e)
        {
            string path = null;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                path = fileDialog.FileName;
                pathlist.Add(path);
            }

            AddTabPage(path);
        }

        private void bn_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello"); 
        }

        private void bn_Close_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult xx = MessageBox.Show("Save?", "WARNING", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning); 
            if(xx ==MessageBoxResult.Cancel)
            {
                return; 
            }
            else if(xx == MessageBoxResult.Yes)
            {
                //
            }
            else
            {
                //close() ; 
            }
        }

        private void bn_CloseAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult xx = MessageBox.Show("Save?", "WARNING", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (xx == MessageBoxResult.Cancel)
            {
                return;
            }
            else if (xx == MessageBoxResult.Yes)
            {
                //Save()
            }
            else
            {
                //close() ; 
            }
        }

        private void drop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            AddTabPage(fileName); 
        }

        private void dropEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;                          
            else
                e.Effects = DragDropEffects.None;
        }

        private void treeView1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem dd= (TreeViewItem)treeView1.SelectedItem;
            if(dd==null)
            {
                return; 
            }
            string cc = (string)dd.Header; 
            if (cc == "DexFile")
            {
                if(hash.Contains("header"))
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem; 
                    xx.Content = hash["header"]; 

                }

            }
            else if(cc=="mapoff")
            {
                if (!hash.Contains("mapoff"))
                {
                    AddMapoff((TabItem)tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["mapoff"]; 
                }
            }
            else if (cc == "stringoff")
            {
                if (!hash.Contains("stringoff"))
                {
                    AddStringoff(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["stringoff"];
                }
            }
            else if (cc == "typeoff")
            {
                if (!hash.Contains("typeoff"))
                {
                    AddTypeoff(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["typeoff"];
                }

            }
            else if (cc == "protooff")
            {
                if (!hash.Contains("protooff"))
                {
                    AddProtooff(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["protooff"];
                } 
            }
            else if (cc == "fieldoff")
            {
                if (!hash.Contains("fieldoff"))
                {
                    AddFieldoff(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["fieldoff"];
                }
            }
            else if (cc == "methodoff")
            {
                if (!hash.Contains("methodoff"))
                {
                    AddMethodoff(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["methodoff"];
                }
            }
            else if (cc == "class_defs")
            {
                if (!hash.Contains("class_defs"))
                {
                    AddClass_defs(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["class_defs"];
                }

            }
            else if (cc == "class_data")
            {
                if (!hash.Contains("class_data"))
                {
                    AddClass_data(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["class_data"];
                }

            }
            else if (cc == "code_off")
            {
                if (!hash.Contains("code_off"))
                {
                    AddCode_off(tabControl1.SelectedItem);
                }
                else
                {
                    TabItem xx = (TabItem)tabControl1.SelectedItem;
                    xx.Content = hash["code_off"];
                }

            }
        }



        private void CreateCode_offView(StackPanel sp)
        {
            dw.AnaCode_off(); 
        }
        private void AddCode_off(object now)
        {
            TabItem nowx = (TabItem)now;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateCode_offView(sp);
            nowx.Content = sp;
            hash.Add("code_off", sp);
        }

        private void AddClass_data(object now)
        {
            TabItem nowx = (TabItem)now;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateClass_dataView(sp);
            nowx.Content = sp;
            hash.Add("class_data", sp);
        }

        private void CreateClass_dataView( StackPanel sp)
        {
            dw.AnaClass_data(); 
        }
        private void AddMapoff(object now)
        {
            TabItem nowx = (TabItem)now;
            
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateMapoffView(sp); 
            nowx.Content = sp; 
            hash.Add("mapoff" ,sp); 

        }

        private void CreateMapoffView(StackPanel sp)
        {
            int map_off = Int32.Parse(header_tmp[8], System.Globalization.NumberStyles.HexNumber) + 4;
            dw.AnaMapoff();
            int len = (int)dw.Maplist.size;
            for (int i = 0; i <= len; i++)
            {
                StackPanel s = new StackPanel();
                s.Orientation = Orientation.Horizontal;
                s.Height = 20;
                Button Address = new Button();
                Address.Width = 100;
                Button Type = new Button();
                Type.Width = 130;
                Button Size = new Button();
                Size.Width = 100;
                Button Offset= new Button();
                Offset.Width = 100;

                if (i == 0)
                {
                    Offset.Content = "Offset";
                    Type.Content = "Type";
                    Size.Content = "Size";
                    Address.Content = "Address"; 

                }
                else
                {
                    Address.Content = Convert.ToString(map_off + (i-1) * 12, 16);
                    Type.Content = allhash[(int)dw.Maplist.list[i - 1].type];
                    Size.Content = dw.Maplist.list[i - 1].size;
                    Offset.Content = Convert.ToString(dw.Maplist.list[i - 1].offset,16);     
                }
                s.Children.Add(Address);
                s.Children.Add(Type);
                s.Children.Add(Size);
                s.Children.Add(Offset);
                sp.Children.Add(s);
            }
        }


        private void AddClass_defs(object now)
        {
            TabItem nowx = (TabItem)now;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateClass_defs(sp);
            nowx.Content = sp;
            hash.Add("class_defs", sp); 
        }
        private void CreateClass_defs(StackPanel sp)
        {
            dw.AnaClass_defs();
        }

        private void AddMethodoff(object now)
        {
            TabItem nowx = (TabItem)now;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateMethodoffView(sp);
            nowx.Content = sp;
            hash.Add("methodoff", sp);
        }
        private void  CreateMethodoffView(StackPanel sp)
        {
            dw.AnaMethodoff(); 
        }
        private void AddStringoff(object now)
        {
            TabItem nowx = (TabItem)now;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateStringoffView(sp);
            nowx.Content = sp;
            hash.Add("stringoff", sp);
        }
        private void CreateStringoffView(StackPanel sp)
        {
            dw.AnaStringoff();
            int len = (int)dw.String_item.size; 
            for (int i = 0; i <= len; i++)
            {
                StackPanel s = new StackPanel();
                s.Orientation = Orientation.Horizontal;
                s.Height = 20;
                Button Index = new Button();
                Index.Width = 100;
                Button Offset = new Button();
                Offset.Width = 130;
                Button Size = new Button();
                Size.Width = 100;
                if (i == 0)
                {
                    Offset.Content = "Offset";
                    Index.Content = "Index";
                    Size.Content = "Size";
                    Button Stringline = new Button();
                    Stringline.Width = 100;
                    Stringline.Content = "Content";
                    s.Children.Add(Index);
                    s.Children.Add(Offset);
                    s.Children.Add(Size);
                    s.Children.Add(Stringline);
                }
                else
                {
                    TextBox tb = new TextBox();
                    tb.Width = 100;
                    tb.FontSize = 15;
                    Index.Content = i;
                    Offset.Content = dw.String_item.strlist[i].offset;
                    Size.Content = dw.String_item.strlist[i].len;
                    tb.Text = dw.String_item.strlist[i].content;
                    s.Children.Add(Index);
                    s.Children.Add(Offset);
                    s.Children.Add(Size);
                    s.Children.Add(tb);
                }

                sp.Children.Add(s);
            }
        } 

        private void AddFieldoff(object now)
        {
            TabItem nowx = (TabItem)now;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateStringoffView(sp);
            nowx.Content = sp;
            hash.Add("fieldoff", sp);


        }
        private void CreateFieldoff(StackPanel sp)
        { }
        private void AddTypeoff(object now)
        {
            TabItem nowx = (TabItem)now;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateStringoffView(sp);
            nowx.Content = sp;
            hash.Add("typeof", sp);


        }
        private void CreateTypeoff(StackPanel sp)
        { }
        private void AddProtooff(object now)
        {
            TabItem nowx = (TabItem)now;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            CreateStringoffView(sp);
            nowx.Content = sp;
            hash.Add("protooff", sp);


        }

        private void CreateProtooff(StackPanel sp)
        { }
    }
}
