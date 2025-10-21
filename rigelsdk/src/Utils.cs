using System.Security.Cryptography;
using System.Text;
namespace rigelsdk.src
{
    public static class Utils
    {
        public static string SerializeToQuryString(Dictionary<string, string> obj)
        {
            var str = new List<string>();
            foreach (var property in obj)
            {
                Console.WriteLine($"property==>{property}");
                if (property.Value != null)
                {
                    str.Add(property.Key + "=" + property.Value.ToString());
                }
            }
            return string.Join("&", str.ToArray());
        }

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
                List<string> querySlice = queryString.Split('&').ToList();
                if (expiry is not 0 and not -1)
                {
                    querySlice.Add($"X-ExpiresAt={expiry}");
                }
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
