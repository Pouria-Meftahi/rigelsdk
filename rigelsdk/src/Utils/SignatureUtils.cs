using System.Security.Cryptography;
using System.Text;

namespace RigelSdk.src.Utils
{
    /// <summary>
    /// Utility methods for signing requests
    /// </summary>
    public static class SignatureUtils
    {
        /// <summary>
        /// Creates HMAC-SHA1 signature
        /// </summary>
        public static string Sign(string key, string salt, string input)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(input + salt);

            using var hmac = new HMACSHA1(keyBytes);
            var hashBytes = hmac.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        /// <summary>
        /// Signs a query string for Rigel requests
        /// </summary>
        public static string SignQueryString(string key, string salt, string requestPath,
            string queryString, long expiry)
        {
            var signableList = new System.Collections.Generic.List<string>
            {
                $"request_path={requestPath}"
            };

            if (!string.IsNullOrEmpty(queryString))
            {
                var queryParts = queryString.Split('&').ToList();

                if (expiry != 0 && expiry != -1)
                {
                    queryParts.Add($"X-ExpiresAt={expiry}");
                }

                queryParts.Sort();
                signableList.AddRange(queryParts);
            }

            var signableString = string.Join("&", signableList);
            var signature = Sign(key, salt, signableString);
            signableList.Add($"X-Signature={signature}");

            // Remove request_path
            signableList.RemoveAt(0);

            return string.Join("&", signableList);
        }
    }
}
