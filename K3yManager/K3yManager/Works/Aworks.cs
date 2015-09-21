using System;
using System.Collections.Generic;
using System.Linq;
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

        public  void Sigcreate(String name)
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

    }
}
