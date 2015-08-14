using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3yManager
{
    class Decry
    {
       private string m_src;
       private string m_key; 
       public  Decry(string str ,string key)
       {
            m_src = str;
            m_key = key;
       }

       
       public string decaes()
       {
           return "niko"; 
       }

       public string decdes()
       {
           return "caf3";
       }
    }
}
