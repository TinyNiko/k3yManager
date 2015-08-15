﻿using System;
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
        private byte[] m_key;
        private byte[] m_src;
 
        public Eenclass()
        {

        }
        public Eenclass(byte[] src)
        {
            m_src = src  ;
            m_key = null ;
        }

        public Eenclass(byte[] src, byte[] key)
        {
            m_key = key ;  
            m_src = src ;
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

        public byte[] MySHA256()
        {
            SHA256 sha = SHA256.Create();
            return sha.ComputeHash(m_src);  
        }
        private byte[] checkaeskey(byte[] key)
        {
            byte[] newkey = new byte[16];
            key.CopyTo(newkey, 0);
            for (int i = key.Length; i < 16; i++)
            {
                newkey[i] = 0x61;
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
            myaes.Mode = CipherMode.CFB;
            myaes.Key = newkey;
            myaes.IV = mInitializationVector; 
            ICryptoTransform trans = myaes.CreateEncryptor() ; 
            CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
            cs.Write(m_src, 0, m_src.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin); 
            return ms.ToArray();  
                   
        }
        public byte[] aesenc(byte[] src , byte[] key)
        {
            byte[] newkey = checkaeskey(key);
            byte[] mInitializationVector = { 0x01, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xf7, 0xEF, 
                                             0x12, 0x23, 0x66, 0x54 , 0x99, 0xA2, 0xB3,0xCE};
            AesCryptoServiceProvider myaes = new AesCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            myaes.Mode = CipherMode.CFB;
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
            mydes.Mode = CipherMode.CFB;
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
            mydes.Mode = CipherMode.CFB;
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
            key.CopyTo(newkey, 0);
            for (int i = key.Length; i < 8; i++)
            {
                newkey[i] = 0x61;
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
            RSA rsa = new RSACryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            byte[] result = null;
            return result; 
        }


    }
}
