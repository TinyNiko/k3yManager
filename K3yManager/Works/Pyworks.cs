using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3yManager.Works;
namespace K3yManager.Works
{
   
    public class Pyworks
    {
        private const string EXEPATH = "C:\\Python27\\python.exe" ;
        private string parameters = null;

        public Pyworks()
        {

        }
       public Pyworks(string path)
        {
            parameters = path; 
        }

        public string work()
        {
            Aworks aw = new Aworks(); 
            string result = aw.exec(EXEPATH , parameters) ;
            return result; 
            
        }
        public string work(string myparameters)
        {
            Aworks aw = new Aworks();
            string result = aw.exec(EXEPATH, myparameters);
            return result;

        }
    }
}
