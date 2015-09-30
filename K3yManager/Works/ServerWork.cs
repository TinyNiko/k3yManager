using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace K3yManager.Works
{
    public class ServerWork
    {
        System.Diagnostics.Stopwatch stopwatch = null; 
        public string pingstr;
        private Socket s;
        private List<String> clientip = new List<string>(); 
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

        public void startListen(int port)
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

        public void waitclient(TextBox tt)
        {
            while (true)
            {
                Socket client = s.Accept();
                tt.Text += "[*Connection] " + client.RemoteEndPoint.ToString();
                clientip.Add(client.RemoteEndPoint.ToString()); 
              
            }
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
