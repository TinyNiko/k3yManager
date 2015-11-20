using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace K3yManager.Works
{


    public class ServerWork
    {
        System.Diagnostics.Stopwatch stopwatch = null; 
        public string pingstr;
        private static Socket s;
        private Socket clientSocket; 
        private static List<Socket> clientip = new List<Socket>(); 
        public string MyPing()
        {
            string ip1 = getIP();
             for(int i = 0; i < 255; i++)
            {
                Ping myPing=new Ping();
                myPing.PingCompleted += new PingCompletedEventHandler(_myPing_PingCompleted);
                // gethost ip 
                string pingIP =ip1  + i.ToString();
                myPing.SendAsync(pingIP, 1000, null);

            }
            return pingstr; 
        }
     

    
        private string getIP()
        {
            string hostname = Dns.GetHostName();
            IPAddress localhost = Dns.GetHostAddresses(hostname)
             .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
              .First();
            return localhost.ToString();
           
        }
        private void _myPing_PingCompleted(object sender, PingCompletedEventArgs e)
        {

            if (e.Reply.Status == IPStatus.Success)
            {
                
                pingstr += (e.Reply.Address.ToString() + "  |") + '\n';
              
            }

        }

        public void startListen(int port ,TextBox ttx) 
        {
            try
            {
                string ipstr = getIP();
                IPAddress ip = IPAddress.Parse(ipstr);
                IPEndPoint ipe = new IPEndPoint(ip, port);

                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Bind(ipe);
                s.Listen(20);
              
                   
            }
            catch(Exception )
            {

            }
        }
        public  string  waitclient()
        {
            Socket client = s.Accept();
            string aa = "[*Connection] " + client.RemoteEndPoint.ToString();
            clientip.Add(client);
            return aa; 
        }

        public void dealclient()
        {
            byte[] buff = new byte[1024];
            Socket client = clientip.Last();
            client.Receive(buff);
            string aa = Encoding.UTF8.GetString(buff);
            MessageBox.Show(aa);
        }

        public int  sendfile(string filepath)
        {
            if(clientSocket==null)
            {
                return -1; 
            }
            // file +8*byte+filesrc 
            byte[] src;
            int  result = -1;
            FileStream fs = new FileStream(filepath, FileMode.Open);
            long len = fs.Length;
            src = new byte[fs.Length];
            string filehead = "file";
            string filename = getfilename(filepath); 
            int  filenamelen = filename.Length; 
            fs.Read(src, 0, src.Length);
            fs.Close();

            clientSocket.Send(Encoding.UTF8.GetBytes(filehead));
            clientSocket.Send(Encoding.UTF8.GetBytes(filenamelen.ToString()));
            clientSocket.Send(Encoding.UTF8.GetBytes(filename));
            clientSocket.Send(Encoding.UTF8.GetBytes(len.ToString())); 
            clientSocket.Send(src);
            result = 0; 
            return result; 
        }

        private string getfilename(string filepath)
        {
            string result = null;
            string[] tt = null; 
            if(filepath.Contains("/"))
                 tt = filepath.Split('/');
            if (filepath.Contains("\\"))
                tt = filepath.Split('\\');

            result = tt[tt.Length-1]; 
            return result; 
        }

        public int ConnectServer(string IPANDPORTNAME)
        {
            byte[] buff = new byte[256]; 
            string[] IPPORT = IPANDPORTNAME.Split(' '); 
            int result =0 ;
            IPAddress ip = IPAddress.Parse(IPPORT[0]);
            int PORT = Int32.Parse(IPPORT[1]); 
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip ,PORT ));
                clientSocket.Send(Encoding.UTF8.GetBytes(  IPPORT[2]));
                Thread.Sleep(1000);
                clientSocket.Send(Encoding.UTF8.GetBytes(IPPORT[3])) ;
                clientSocket.Receive(buff);
                if(buff[0] ==0x09 )
                {
                    clientSocket.Close();
                    return result;  
                } 
                
                result = 1; 
            }
            catch
            {
                return  result;
            }
            return result; 
        }

        public string SendIns(string tmp)
        {
            if (clientSocket == null)
                return null; 
            byte[] buff = new byte[1024]; 
            string send = "system" + tmp;
            clientSocket.Send(Encoding.UTF8.GetBytes(send));
            Thread.Sleep(2000); 
            clientSocket.Receive(buff) ;
            return Encoding.UTF8.GetString(buff);
        }

        public void stopserver()
        {
            s.Close();
            stopwatch.Stop();
            stopwatch.Reset();
        }

        public string ShowInfo()
        {
            // 客户端连接数 ，服务器时间
            if(s==null)
            {
                return "服务器未启动\n";
            }
            stopwatch.Stop();
            string result = "";
            result += "启动时间：" + stopwatch.Elapsed+"\n";
            result += "连接数 ：" + clientip.Count + "\n";  

            stopwatch.Start();
            return result; 
        }

        public void Timer()
        {
            stopwatch= new System.Diagnostics.Stopwatch(); 
            stopwatch.Start(); 
        }
    }
}
