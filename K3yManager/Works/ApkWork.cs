using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace K3yManager.Works
{
    public class ApkWork
    {
        const int MOD_ADLER = 65521;
        private string apkfilepath = null;
        private byte[] payloadArray = null;
        private byte[] unShelldexArray = null;
        public ApkWork(string filepath)
        {
            apkfilepath = filepath;

        }

        public int encapk()
        {
            FileStream fs = new FileStream(apkfilepath, FileMode.Open);
            byte[] tmppayloadArray = new byte[fs.Length];
            fs.Read(tmppayloadArray, 0, payloadArray.Length);
            fs.Close();
            FileStream fs2 = new FileStream("D:\\unshell.dex", FileMode.Open);
            unShelldexArray = new byte[fs2.Length];
            fs.Read(unShelldexArray, 0, unShelldexArray.Length);
            fs.Close();
            try
            {

                payloadArray = myencrypt(tmppayloadArray);
                int payloadlen = payloadArray.Length;
                int unShelldexlen = unShelldexArray.Length;
                int totallen = 4 + payloadlen + unShelldexlen;
                byte[] newdex = new byte[totallen];

                Array.Copy(unShelldexArray, 0, newdex, 0, unShelldexlen);
                Array.Copy(payloadArray, 0, newdex, unShelldexlen, payloadlen);
                Array.Copy(BitConverter.GetBytes(payloadlen), 0, newdex, totallen - 4, 4);

                fixFileSizeHeader(newdex);
                fixSHA1Header(newdex);
                fixCheckSumHeader(newdex);

                String str = "g:\\classes.dex";
                FileStream fs3 = new FileStream(str, FileMode.Create);
                fs3.Write(newdex, 0, newdex.Length);
                fs3.Close(); 

            }
            catch (Exception e)
            {

            }
            return 0;
        }


        public static void fixSHA1Header(byte[] dexBytes)
        {
            SHA1 a = SHA1.Create(); 
            byte[] newdt = a.ComputeHash(dexBytes,32,dexBytes.Length-32);
            Array.Copy(newdt, 0, dexBytes, 12, 20);
        }


        public static void fixFileSizeHeader(byte[] dexBytes)
        {
            byte[] newfs = BitConverter.GetBytes( dexBytes.Length);
            byte[] refs = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                refs[i] = newfs[newfs.Length - 1 - i];

            }
           Array.Copy(refs, 0, dexBytes, 32, 4);
        } 
        public static void fixCheckSumHeader(byte[] dexbytes)
        {
 
            int va = adler32(dexbytes, 12, dexbytes.Length - 12);
            byte[] newcs = BitConverter.GetBytes(va);
            byte[] recs = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                recs[i] = newcs[newcs.Length - 1 - i];
              
            }
           Array.Copy(recs, 0, dexbytes, 8, 4);

        }

        public static int adler32(byte[] dexBytes, int offset, int len)
        {
            int  a = 1, b = 0;
            int  index=offset;

            for ( ; index < len; ++index)
            {
                a = (a + dexBytes[index]) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }
            return (b << 16) | a;
        }


        public  byte[] myencrypt(byte[] dexBytes)
        {
              byte[] tmp = new byte[dexBytes.Length];
              for (int i = 0; i < dexBytes.Length; i++)
              {
                 tmp[i]--;
              }
              return tmp;

       }



    }
}
