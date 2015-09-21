using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace K3yManager.Works
{
    public class ServerWork
    {
        public string pingstr; 
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
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            IPAddress ip = localhost.AddressList[0];
            return ip.ToString(); 

        }
        private void _myPing_PingCompleted(object sender, PingCompletedEventArgs e)
        {

            if (e.Reply.Status == IPStatus.Success)
            {
                
                pingstr += (e.Reply.Address.ToString() + "  |") + '\n';
              
            }

        }
    }
}
