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
   
    class Eenclass
    {
        private MyConfig con; 
        private byte[] m_key;
        private byte[] m_src;
        private string pubkey; 
        public Eenclass()
        {
            creatersakey(); 
        }
        public Eenclass(byte[] src)
        {
            m_src = src  ;
            m_key = null ;
            creatersakey();
        }

        public Eenclass(byte[] src, byte[] key)
        {
            m_key = key ;  
            m_src = src ;
            creatersakey();
        }
        
        public string hex2str(byte[] src)
        {
            string str = BitConverter.ToString(src).Replace("-", string.Empty);

            return str;
        }
        public byte[] Mymd5()
        {
            MD5 md5 = MD5.Create();

            return md5.ComputeHash(m_src); 
        }
        public byte[] Mymd5(string src)
        {
            MD5 md5 = MD5.Create();
            byte[] bsrc = Encoding.UTF8.GetBytes(src); 
            return md5.ComputeHash(bsrc);
        }


        public byte[] MySHA256()
        {
            SHA256 sha = SHA256.Create();
            return sha.ComputeHash(m_src);  
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

        public byte[] aesenc()
        {
            byte[] newkey = checkaeskey(m_key); 
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF, 
                                             0x12, 0x23, 0x66, 0x54 , 0x99, 0xA2, 0xB3,0xCE};
            AesCryptoServiceProvider myaes = new AesCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            myaes.Mode = CipherMode.CBC;
            myaes.Key = newkey;
            myaes.IV = mInitializationVector; 
            ICryptoTransform trans = myaes.CreateEncryptor() ; 
            CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
            cs.Write(m_src, 0, m_src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin); 
            return ms.ToArray();  
                   
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
        public byte[] aesenc(byte[] src , byte[] key)
        {
            byte[] newkey = checkaeskey(key);
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF, 
                                             0x12, 0x23, 0x66, 0x54 , 0x99, 0xA2, 0xB3,0xCE};
            AesCryptoServiceProvider myaes = new AesCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            myaes.Mode = CipherMode.CBC;
            myaes.Key = newkey;
            myaes.IV = mInitializationVector;
            ICryptoTransform trans = myaes.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
            cs.Write(src, 0, src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();

        }

        public byte[] desenc()
        {
            byte[] newkey = checkdeskey(m_key);
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF };
            DES mydes = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            mydes.Mode = CipherMode.CBC;
            mydes.Key = newkey;
            mydes.IV = mInitializationVector;
            ICryptoTransform encryptor = mydes.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(m_src, 0, m_src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin); 
           // byte[] byteenc = new byte[512] ; 
            //ms.Read(byteenc,0, 512) ;
            return ms.ToArray();
        }

        public byte[] desenc(byte[] src, byte[] key)
        {
            byte[] newkey = checkdeskey(key);
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF };
            DES mydes = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            mydes.Mode = CipherMode.CBC;
            mydes.Key = newkey;
            mydes.IV = mInitializationVector;
            ICryptoTransform encryptor = mydes.CreateEncryptor();
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

          
        public byte[] rsaenc()
        {
            byte[] result = null  ;
            return result ; 
        }

        public byte[] rsaenc(byte[] src , byte[] key)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubkey);
            byte[] result = rsa.Encrypt(src, false);
            return result; 

        }

        private void creatersakey()
        {
            con = new MyConfig();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            if (con.GetValue("RSAP") == null)
            {
                
                con.SetValue("RSAP", rsa.ToXmlString(true));
                con.SetValue("RSAPR", rsa.ToXmlString(false));
                pubkey = con.GetValue("RSAP"); 

            }
            else
            {  
                pubkey = con.GetValue("RSAP");
            }
            

        }


        public byte[] rc4enc()
        {
            byte[] result = null;

            return result;



        }


        public byte[] rc4enc(byte[] src ,byte[] key)
        {
            byte[] result = null;

            return result;      
        }

    }
}
