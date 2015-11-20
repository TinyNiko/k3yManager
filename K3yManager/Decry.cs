using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace K3yManager
{
    class Decry
    {
       private byte[] m_src;
       private byte[] m_key;
        private string prikey; 
       public  Decry(byte[] str ,byte[] key)
       {
            m_src = str;
            m_key = key;
            getkey();
       }

       public Decry()
       {
            getkey();
        }

       public byte[] decaes(byte[] src, byte[] key)
       {
           byte[] newkey = checkaeskey(key);
           byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF, 
                                             0x12, 0x23, 0x66, 0x54 , 0x99, 0xA2, 0xB3,0xCE};
           AesCryptoServiceProvider myaes = new AesCryptoServiceProvider();
           MemoryStream ms = new MemoryStream();
           myaes.Mode = CipherMode.CBC;
           myaes.Key = newkey;
           myaes.IV = mInitializationVector;
           ICryptoTransform trans = myaes.CreateDecryptor();
           CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
           cs.Write(src, 0, src.Length);
           cs.FlushFinalBlock();
           ms.Seek(0, SeekOrigin.Begin);
           return ms.ToArray();

       }
       
       public byte[] decaes()
       {
            byte[] newkey = checkaeskey(m_key); 
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF, 
                                             0x12, 0x23, 0x66, 0x54 , 0x99, 0xA2, 0xB3,0xCE};
            AesCryptoServiceProvider myaes = new AesCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            myaes.Mode = CipherMode.CBC;
            myaes.Key = newkey;
            myaes.IV = mInitializationVector; 
            ICryptoTransform trans = myaes.CreateDecryptor() ; 
            CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
            cs.Write(m_src, 0, m_src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin); 
            return ms.ToArray();  
       }

       
       public string hex2str(byte[] src)
       {
            string str = BitConverter.ToString(src).Replace("-", string.Empty);
            
            return str;
       }
       public byte[] str2hex(string str)
       {
           byte[] vBytes = new byte[str.Length / 2];
           for (int i = 0; i < str.Length; i += 2)
               if (!byte.TryParse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber,
                   null, out vBytes[i / 2]))
               {
                   vBytes[i / 2] = 0;
               }
           return vBytes;
       }

        private byte[] checkaeskey(byte[] key)
        {
            byte[] newkey = new byte[16];
            if (key.Length > 16)
            {
                Array.Copy(key, newkey, 16);
            }
            else
            {

                Array.Copy(key, newkey, key.Length);
                for (int i = key.Length; i < 16; i++)
                {
                    newkey[i] = 0x61;
                }
            }
            return newkey;
        }

       public byte[] decdes()
       {
            byte[] newkey = checkdeskey(m_key);
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF };
            DES mydes = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            mydes.Mode = CipherMode.CBC;
            mydes.Key = newkey;
            mydes.IV = mInitializationVector;
            ICryptoTransform encryptor = mydes.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(m_src, 0, m_src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin); 
           // byte[] byteenc = new byte[512] ; 
            //ms.Read(byteenc,0, 512) ;
            return ms.ToArray();
       }

         public byte[] decdes(byte[] src, byte[] key)
        {
            byte[] newkey = checkdeskey(key);
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF };
          
            DES mydes = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            mydes.Mode = CipherMode.CBC;
            mydes.Key = newkey;
            mydes.IV = mInitializationVector;
            ICryptoTransform encryptor = mydes.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(src, 0, src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin);
            // byte[] byteenc = new byte[512] ; 
            //ms.Read(byteenc,0, 512) ;
            return ms.ToArray();

        }


         private byte[] checkdeskey(byte[] key)
         {
             byte[] newkey = new byte[8];
            if (key.Length > 8)
            {
                Array.Copy(key, newkey, 8);
            }
            else
            {
                Array.Copy(key, newkey, key.Length);
                for (int i = key.Length; i < 8; i++)
                {
                    newkey[i] = 0x61;
                }
            }
             return newkey;
         }

        public byte[] decrsa(byte[] src)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(prikey);
            byte[] result = rsa.Decrypt(src, false);
            return result; 

        }

        public byte[] decrc4()
        {
            byte[] result = null;

            return result; 
        }


        public byte[] decrc4(byte[] src, byte[] ket)
        {
            byte[] result = null;

            return result; 
        }


        private void getkey()
        {
            MyConfig con = new MyConfig();
            if (con.GetValue("RSAPR") == null)
            {
                MessageBox.Show("There is no KEY");
                prikey = null;
                return;
            }
            else
            {
                prikey = con.GetValue("RSAPR"); 
            }
        }
    }
}
