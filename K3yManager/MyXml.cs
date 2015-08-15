using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3yManager
{
    class MyXml
    {
        private string mfilepath ;
        private string mthingstodo ;  
        private string mtag ;
        MyXml()
        {

        }
        MyXml(string mfilepath ,string mtag)
        {
            this.mtag = mtag ; 
            this.mfilepath = mfilepath; 
        }
        MyXml(string mfilepath , string tag , string mthingstodo)
        {
            this.mtag = tag   ; 
            this.mfilepath = mfilepath;
            this.mthingstodo = mthingstodo; 
        }

        public bool delete()
        {
            return true; 
        }
        public bool delete(string mfilepath , string tag ,string mthingstodo)
        {
            return true; 
        }

        public bool add()
        {
            return true;
        }
        public bool add(string mfilepath, string tag, string mthingstodo)
        {
            return true;
        }

        public bool query()
        {
            return true;
        }
        public bool query(string mfilepath, string tag, string mthingstodo)
        {
            return true;
        }
    }
}
