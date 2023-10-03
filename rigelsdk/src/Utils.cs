using System.Security.Cryptography;
using System.Text;

namespace rigelsdk.src
{
    public static class Utils
    {
        public static string SerializeToQuryString(object obj)
        {
            var str = new List<string>();
            foreach (dynamic property in obj.GetType().GetProperties())
            {
                if (property != null)
                {
                    str.Add(property + "=" + property.ToString());
                }
            }
            return string.Join("&", str.ToArray());
        }

        //public static string GenerateHMAC(string key, string salt, string input)
        //{
        //    var encoding = new System.Text.ASCIIEncoding();
        //    byte[] keyBytes = encoding.GetBytes(key);
        //    byte[] messageBytes = encoding.GetBytes(input + salt);
        //    using var hmacsha1 = new HMACSHA1(keyBytes);
        //    byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
        //    return Convert.ToBase64String(hashmessage);
        //}
        public static string Sign(string key, string salt, string input)
        {
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            var data = Encoding.UTF8.GetBytes(input + salt);
            var hash = hmac.ComputeHash(data);
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] data)
        {
            var base64 = Convert.ToBase64String(data);
            var base64Url = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64Url;
        }

        public static string SignQueryString(string key, string salt, string requestPath, string queryString, long expiry)
        {
            List<string> signableSlice = new()
            {
                $"request_path={requestPath}"
            };
            if (!string.IsNullOrEmpty(queryString))
            {
                var querySlice = queryString.Split('&');
                if (expiry is not 0 and not -1)
                {
                    _ = querySlice.Append($"X-ExpiresAt={expiry}");
                }
                querySlice = querySlice.OrderBy(q => q).ToList().ToArray();
                signableSlice.AddRange(querySlice);
            }

            var signableString = string.Join('&', signableSlice);
            var signature = Sign(key, salt, signableString);
            signableSlice.Add($"X-Signature={signature}");

            // Removing request_path
            signableSlice.RemoveAt(0);

            return string.Join('&', signableSlice);
        }
    }
}
