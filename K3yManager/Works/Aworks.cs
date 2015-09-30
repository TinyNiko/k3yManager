using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace K3yManager.Works
{
    public class Aworks
    {
        private string g_newpath;
        private string g_filepath;
        private string _serialNumber;
        private string _driveLetter;
        private string[] WAY2CRYPTO = { "AES", "DES", "RC4" };
        private Eenclass enc = new Eenclass();
        private Decry dec = new Decry();
        [DllImportAttribute("kernel32.dll")]
        public static extern int WinExec(string exename, int type);


        public string exec(string exePath, string parameters)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.FileName = exePath;
            psi.Arguments = parameters;
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
            System.IO.StreamReader outputStreamReader = process.StandardOutput;
            System.IO.StreamReader errStreamReader = process.StandardError;
            process.WaitForExit(2000);
            if (process.HasExited)
            {
                string output = outputStreamReader.ReadToEnd();
                string error = errStreamReader.ReadToEnd();

                return output;
            }
            return null;

        }

        public void Sigcreate(String name)
        {
            try
            {

                CspParameters cspParams = new CspParameters();
                cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";

                RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(name);

                SignXml(xmlDoc, rsaKey);

                xmlDoc.Save(name);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void SignXml(XmlDocument Doc, RSA Key)
        {
            // Check arguments.
            if (Doc == null)
                throw new ArgumentException("Doc");
            if (Key == null)
                throw new ArgumentException("Key");

            SignedXml signedXml = new SignedXml(Doc);
            signedXml.SigningKey = Key;
            Reference reference = new Reference();
            reference.Uri = "";
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            signedXml.AddReference(reference);

            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));

        }

        public Boolean Verify(string name)
        {
            try
            {

                CspParameters cspParams = new CspParameters();
                cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";

                RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(name);

                bool result = VerifyXml(xmlDoc, rsaKey);

                return result;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }

        }

        private static Boolean VerifyXml(XmlDocument Doc, RSA Key)
        {
            if (Doc == null)
                throw new ArgumentException("Doc");
            if (Key == null)
                throw new ArgumentException("Key");

            SignedXml signedXml = new SignedXml(Doc);

            XmlNodeList nodeList = Doc.GetElementsByTagName("Signature");

            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            signedXml.LoadXml((XmlElement)nodeList[0]);

            return signedXml.CheckSignature(Key);
        }

        public string getSerialNumberFromDriveLetter(string driveLetter)
        {
            this._driveLetter = driveLetter.ToUpper();

            if (!this._driveLetter.Contains(":"))
            {
                this._driveLetter += ":";
            }

            matchDriveLetterWithSerial();

            return this._serialNumber;
        }

        private void matchDriveLetterWithSerial()
        {

            string[] diskArray;
            string driveNumber;
            string driveLetter;

            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition");
            foreach (ManagementObject dm in searcher1.Get())
            {
                diskArray = null;
                driveLetter = getValueInQuotes(dm["Dependent"].ToString());
                diskArray = getValueInQuotes(dm["Antecedent"].ToString()).Split(',');
                driveNumber = diskArray[0].Remove(0, 6).Trim();
                if (driveLetter == this._driveLetter)
                {
                    /* This is where we get the drive serial */
                    ManagementObjectSearcher disks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                    foreach (ManagementObject disk in disks.Get())
                    {

                        if (disk["Name"].ToString() == ("\\\\.\\PHYSICALDRIVE" + driveNumber) & disk["InterfaceType"].ToString() == "USB")
                        {
                            this._serialNumber = parseSerialFromDeviceID(disk["PNPDeviceID"].ToString());
                        }
                    }
                }
            }
        }




        private string parseSerialFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('\\');
            string[] serialArray;
            string serial;
            int arrayLen = splitDeviceId.Length - 1;

            serialArray = splitDeviceId[arrayLen].Split('&');
            serial = serialArray[0];

            return serial;
        }


        private string getValueInQuotes(string inValue)
        {
            string parsedValue = "";

            int posFoundStart = 0;
            int posFoundEnd = 0;

            posFoundStart = inValue.IndexOf("\"");
            posFoundEnd = inValue.IndexOf("\"", posFoundStart + 1);

            parsedValue = inValue.Substring(posFoundStart + 1, (posFoundEnd - posFoundStart) - 1);

            return parsedValue;
        }

        public string getUname()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            /*
            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  File type: {0}", d.DriveType);
            }
            */
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    return d.Name;
                }
            }

            return null;
        }

        public Boolean EncFile(string path, string encway, byte[] key)
        {
            g_newpath = null; 
            g_filepath = path;
            return RealEncFile(encway, key);
        }
        // don't overwrite 
        public Boolean EncFile(string filepath, string newpath, string encway, byte[] key)
        {
            g_filepath = filepath;
            g_newpath = newpath;
            return RealEncFile(encway, key);

        }

        private bool test(byte[] bkey)
        {
            string src = "11111111111111111111\n";

            byte[] bsrc = Encoding.UTF8.GetBytes(src);
       
            byte[] tmp = enc.desenc(bsrc, bkey);
            MessageBox.Show(enc.hex2str(tmp));
            byte[] tmp2=dec.decdes(tmp, bkey);

            MessageBox.Show(System.Text.Encoding.Default.GetString(tmp2));
            return true; 
        }


        public byte[] generatekey(string salt, string key)
        {
            byte[] result = new byte[16];
            string src = salt + key;
            result =enc.Mymd5(src);
            return result;
        }

        public Boolean webcheckpass(string salt)
        {//TODO 
            return true;
        }

        public Boolean DecFile(string path, string encway, byte[] key)
        {
            g_filepath = path;
            g_newpath = null;
            return RealDecFile(encway, key);
        }


        // don't overwrite

        public Boolean DecFile(string filepath, string newpath, string encway, byte[] key)
        {
            g_newpath = newpath;
            g_filepath = filepath;
            return RealDecFile(encway, key);
        }


        private bool RealEncFile(string encway, byte[] key)
        {
            FileStream fs = new FileStream(g_filepath, FileMode.Open);
            byte[] tmp = null; 
            byte[] src=new byte[fs.Length];
            fs.Read(src, 0, src.Length);
            if (encway.Equals("AES"))
            {
                 tmp = enc.aesenc(src, key);
         
            }
            else if (encway.Equals("DES"))
            {
               tmp = enc.desenc(src, key);
            }
            else if (encway.Equals("RC4"))
            {
                tmp = enc.rc4enc(src, key); 
            }
            else
            {
                return false;
            }
            
            if(g_newpath==null)
            {// 
                FileStream oldfile = new FileStream(g_filepath, FileMode.Truncate);
                oldfile.Write(tmp, 0, tmp.Length);
                oldfile.Close(); 
            }
            else
            {//overwrite
              FileStream newfile =new FileStream(g_newpath,FileMode.Create) ;
              //StreamWriter sw = new StreamWriter(newfile, Encoding.UTF8);
              newfile.Write(tmp, 0, tmp.Length);
                newfile.Close();
            }

            return true;
        }

        private bool RealDecFile(string encway, byte[] key)
        {
            byte[] tmp=null;
            FileStream fs = new FileStream(g_filepath, FileMode.Open);
            byte[] src = new byte[fs.Length] ;
            fs.Read(src, 0, src.Length);
            fs.Close();
          
            if (encway.Equals("AES"))
            {
               tmp =  dec.decaes(src, key);
             
            }
            else if (encway.Equals("DES"))
            {
                tmp =dec.decdes(src, key);
           
            }
            else if (encway.Equals("RC4"))
            {
                tmp = dec.decrc4(src, key); 
            }
            else
            {
                return false;
            }

            if (g_newpath==null)
            {// 
                FileStream oldfile = new FileStream(g_filepath, FileMode.Truncate);
                oldfile.Write(tmp, 0, tmp.Length);
                oldfile.Close();
            }
            else
            {//overwrite
                FileStream newfile = new FileStream(g_newpath, FileMode.Create);
                //StreamWriter sw = new StreamWriter(newfile, Encoding.UTF8);
                newfile.Write(tmp, 0, tmp.Length);
                newfile.Close(); 
            }


            return true ;
        }


    }
}
