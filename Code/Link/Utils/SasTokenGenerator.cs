using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WALLE.Link.Utils
{
    public static class SasTokenGenerator
    {
        public static string CreateSasToken(string resourceUri, string keyName, string key)
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            const long tenYears = 60 * 60 * 24 * 356 * 10;

            string expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + tenYears);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

                return string.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            }
        }
    }
}