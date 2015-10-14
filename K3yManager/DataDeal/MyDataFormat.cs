using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace K3yManager.DataDeal
{
   public  class MyDataFormat
    {
       public  MyDataFormat()
        {

        }
       

        //add 0xxxx , 0xxxxx , 
        public string  dealhex(string src)
        {
           
            string start = dealtrim(src);
            if (iseven(start))
                return null;

            string result = "0x";
            result += src[0]; 

            for (int i = 1; i < src.Length-2; i++)
            {
                result += start[i];
                if (i % 2 == 0)
                {
                    result += ",0x";
                }
                
            }
            result += start[start.Length - 2];
            result += start[start.Length - 1]; 

            return result;
        }


        public string dealstr2hex(string src)
        {
            Eenclass enc =new Eenclass();
            string result = null;
            byte[] aa = Encoding.UTF8.GetBytes(src);
            result = enc.hex2str(aa); 
            return result; 
        }

        public string dealpython(string src)
        {
            string start = dealtrim(src); 
            string result = null ;
            string first = "payload=\"\"\n";
            string second = "payload+=\"\\x";
            result += first+second;
            for (int i = 0; i < start.Length; i++)
            {
                if (i % 2 == 0 &&i%20 !=0)
                {
                    result += "\\x";
                }

                if(i%20 == 0 && i!=0)
                {
                    result += "\"\n" + second;
                }
                result += start[i];

            }
            //result += start[start.Length - 2];
            //result += start[start.Length - 1];
            result += "\"";

            return result; 
        }

        private bool iseven( string src)
        {
            if (src.Length % 2 != 0)
            {
                MessageBox.Show("Length is even ,wrong.", "WRONG", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;

            }
            return false; 
        }
        public string dealhexstring(string src)
        {
           
            string start = dealtrim(src);
            if (iseven(start))
                return null;
            string result = null;
            for(int i =0; i <start.Length;i++)
            {
                if(i% 2== 0 )
                {
                    result += "\\x" ;
                }
                result += start[i]; 

            }
            //result += start[start.Length - 2];
            //result += start[start.Length - 1];

            return result;
        }

        public string dealc(string src)
        {
           
            string start = dealtrim(src);
            if (iseven(start))
                return null;
            string result = null;
            string shell = "char shellcode[]=\"";
            result += shell; 
            for (int i = 0; i < start.Length; i++)
            {
                if (i % 2 == 0 &&i %20 != 0) 
                {
                    result += "\\x";
                }
                result += start[i];

                if (i % 20 == 0)
                {
                    result += "\"\n                 \"";
                }

            }
            result += "\""; 
            return result;
        }

        public string dealtrim(string src)
        {
           

            string result =null;

            CharEnumerator CEnumerator = src.GetEnumerator();

            while (CEnumerator.MoveNext())

            {

                byte[] array = new byte[1];

                array = System.Text.Encoding.ASCII.GetBytes(CEnumerator.Current.ToString());

                int asciicode = (short)(array[0]);

                if (asciicode != 32 && asciicode != 10 &&  asciicode!= 13)

                {

                    result += CEnumerator.Current.ToString();

                }

            }
            if (iseven(result))
                return null;
            return result; 
        }
    }
}
