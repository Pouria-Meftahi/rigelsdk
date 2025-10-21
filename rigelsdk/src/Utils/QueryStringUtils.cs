using System.Web;

namespace RigelSdk.src.Utils
{

    /// <summary>
    /// Utility methods for query string operations
    /// </summary>
    public static class QueryStringUtils
    {
        /// <summary>
        /// Serializes an object to query string format
        /// </summary>
        public static string SerializeToQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(kvp =>
                $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
        }
    }
}
