using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Utility
{
    public class Notification
    {
        private static object _PUBLICKEY="";
    
        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static bool validateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // stop communicate with unauthenticated servers.
            if (certificate == null || chain == null)
                return false;

            // stop communicate with unauthenticated servers.
            if (sslPolicyErrors != SslPolicyErrors.None)
                return false;

            // match certificate public key and allow communicate with authenticated servers.
            String publicekey = certificate.GetPublicKeyString();
            if (publicekey.Equals(_PUBLICKEY))
                return true;

            // stop communicate with unauthenticated servers.
            return false;
        }
    }
}
